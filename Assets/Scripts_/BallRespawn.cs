using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRespawn : MonoBehaviour
{
    public Rigidbody2D _rb2D; 

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "LineStart")
        {
            _rb2D.velocity = Vector2.zero;
            this.transform.position = new Vector2(transform.position.x, -4.75f);

         //Запоминание позиции следующего удара
            if (BallManager.instance.firstBallFlight)
            {
                BallManager.instance.lastBallPosition = transform.position;
                BallManager.instance.firstBallFlight = false;
            }
         //Деактивация последующих шаров
            else
            { 
                StartCoroutine(MoveToStartPosition(BallManager.instance.lastBallPosition));
            }

            BallManager.instance.countRespawnBall += 1;
        }
        if (col.gameObject.tag == "Block")
        { 
            _rb2D.AddTorque(Random.Range(-45f, 45f));
        }
    }

    IEnumerator MoveToStartPosition(Vector2 targetPos)
    { 
        float distance = Vector3.Distance(transform.position, targetPos);

        while (distance > 0.2f)
        {
            float step = BallManager.instance.speedBall * Time.smoothDeltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);

            yield return new WaitForFixedUpdate();
            distance = Vector3.Distance(transform.position, targetPos);
        }
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlusBall")
        {
            BallManager.instance.countToAddBall += 1;
            GameManager.instance.listGameObjects.Remove(col.gameObject);
            Destroy(col.gameObject);
        }

        if (col.tag == "Coint")
        {
            GameManager.instance.AddCoint();
            GameManager.instance.listGameObjects.Remove(col.gameObject);
            Destroy(col.gameObject);
        }

        if (col.tag == "Scatter")
        {
            BallManager.instance.ChangeBallDirection(this.gameObject);
        }
    }
}
