using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareControll : MonoBehaviour
{
    int point;

    TextMesh _pointText;
    SpriteRenderer _spriteRend;

    void Start ()
    { 
        _spriteRend = GetComponent<SpriteRenderer>();
        _pointText = GetComponentInChildren<TextMesh>();

        InitializePoints();

        _pointText.text = point.ToString();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "ball")
        {
            StartCoroutine(Blink());
            point--;
            _pointText.text = point.ToString();
        }
        if (point < 1)
        {
            GameManager.instance.listGameObjects.Remove(this.gameObject);
            GameManager.instance.PlayParticle(transform.position);
            Destroy(this.gameObject);
        }
    }

    void InitializePoints()
    {
        switch (this.name)
        {
            case "Block0":
                point = Random.Range(1, GameManager.instance.score + 1);
                break;
            case "Block1":
                point = Random.Range(10, 25);
                break;
            case "Block2":
                point = Random.Range(26, 50);
                break;
            case "Block3":
                point = Random.Range(51, 80);
                break;
            case "Block4":
                point = Random.Range(81, 101);
                break;
            default:
                point = 0;
                break;
        }
    }

    IEnumerator Blink()
    { 
        _spriteRend.color = new Color( 1f, 1f, 1f, .5f);
        yield return new WaitForSeconds(0.1f);
        _spriteRend.color = new Color(1f, 1f, 1f, 1f);

    }
}
