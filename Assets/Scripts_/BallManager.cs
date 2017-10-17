using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    public static BallManager instance;

    public GameObject ball;
    public Transform ballHolder;
    public Button SpeedUpBtn;
    public Button SpeedNormalizeBtn;
    public int countRespawnBall = 0;
    public float speedBall = 10;

    [HideInInspector]
    public List<GameObject> listBall = new List<GameObject>();

    Vector2 directionFiring;
    [HideInInspector]
    public Vector2 lastBallPosition;
    [HideInInspector]
    public bool firstBallFlight;

    [HideInInspector]
    public int countToAddBall;
    [HideInInspector]
    public bool ballInFlight;
    Vector2 directBall;
    float delayTime = 0.08f;
    Vector2 mouseDownPos;
    bool isPressed = false;
     
    LineRenderer _lineRender; 
    Color color = new Color(0, 150, 200, 255);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _lineRender = GetComponent<LineRenderer>(); 
        SetupBall();
    }

    void SetupBall()
    {
        //Установление начальной позиции шара
        GameObject newBall = Instantiate(ball, new Vector2(Random.Range(-3, 3), -4.75f), Quaternion.identity);
        newBall.transform.SetParent(ballHolder);

        lastBallPosition = newBall.transform.position;

        listBall.Add(newBall);
    }

    private void Update()
    {  
        if (Input.GetMouseButtonDown(0) && !ballInFlight && !CheckPositionMouse())
        {
            
            _lineRender.enabled = true;
            isPressed = true;

            mouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } 

        #region Задание направления броска (пунктир)
        if (isPressed)
        {
            Vector2 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            directionFiring = new Vector2
            (
                Mathf.Clamp(mouseDownPos.x - curMousePos.x, -7f, 7f),
                Mathf.Clamp(mouseDownPos.y - curMousePos.y, -3.5f, 3.5f)
            );
              
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(0, 1.0f) });

            _lineRender.colorGradient = gradient;

            _lineRender.SetPosition(0, lastBallPosition);
            _lineRender.SetPosition(1, directionFiring);
        }
        #endregion

        if (Input.GetMouseButtonUp(0) && !ballInFlight && !CheckPositionMouse())
        {  
            if (!ballInFlight)
            { 
                StartCoroutine(ShootBall());
            }
            firstBallFlight = true;

            _lineRender.enabled = false;

            ballInFlight = true;
            isPressed = false; 
        }

        #region Вызов следующей волны блоков (возможное добавление шара)

        if (countRespawnBall == listBall.Count)
        {
            countRespawnBall = 0; 

            if (countToAddBall > 0)
            {
                AddBall(countToAddBall);

                countToAddBall = 0;
            }

            GameManager.instance.NextWave();
        }
        #endregion
    }

    IEnumerator ShootBall()
    {
        Vector2 localLastBallPos = lastBallPosition;

        int countBallFlight = 0;
        foreach (GameObject ball in listBall)
        {
            countBallFlight++;
            ball.gameObject.SetActive(true);
            ball.transform.position = localLastBallPos;

            directBall = (localLastBallPos - directionFiring) * -1;
            ball.GetComponent<BallRespawn>()._rb2D.velocity = directBall.normalized * speedBall;

            yield return new WaitForSeconds(delayTime);

            GameManager.instance.ballsCountText.text = "BALLS: " + (listBall.Count - countBallFlight).ToString();
        } 
    }
     
    public void AddBall(int countBallToAdd)
    {
        for (int i = 0; i < countBallToAdd; i++)
        {
            GameObject newBall = Instantiate(ball);
            newBall.transform.SetParent(ballHolder);
            newBall.gameObject.SetActive(false);

            listBall.Add(newBall);
        }  
    }

    public void ChangeBallDirection(GameObject ball)
    {
        Vector2 redirect = new Vector2(Random.Range(-5, 6), Random.Range(2,6));
        ball.GetComponent<BallRespawn>()._rb2D.velocity = redirect.normalized * speedBall;
    }
     
    public void SpeedUp()
    {
        speedBall *=2;
        SetNewSpeedBalls(true);

        SpeedUpBtn.gameObject.SetActive(false);
        SpeedNormalizeBtn.gameObject.SetActive(true); 
    }
    public void NormalizeSpeed()
    {
        speedBall /=2;
        SetNewSpeedBalls(false);

        SpeedNormalizeBtn.gameObject.SetActive(false);
        SpeedUpBtn.gameObject.SetActive(true);
    }

    void SetNewSpeedBalls(bool speedChange)
    { 
        foreach (GameObject ball in listBall)
        {
            if (speedChange)
            {
                ball.GetComponent<BallRespawn>()._rb2D.velocity *= 2;
            }
            else
            {
                ball.GetComponent<BallRespawn>()._rb2D.velocity /= 2;

            }
        } 
    }

    bool CheckPositionMouse()
    {
        Vector2 posInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (posInput.x > -3f && posInput.x < -2.5f) && (posInput.y > 5.8f && posInput.y < 6.3f);
    }
}
