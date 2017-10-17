using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{ 
    public static GameManager instance;

    public const int minWidthPos = -3, maxWidthPos = 3;//-3;3
    public const int minHeightPos = -4, maxHeightPos = 5;
    [Header("GameObjects")]
    public GameObject[] blocks; 
    public GameObject[] gifts;
    public Transform gameObjectsHolder; 
    public ParticleSystem explosionBlock;
    [Space]
    [Header("UI")]
    public GameObject pausePanel;
    public Button pauseButton;
    public GameObject gameOverPanel;
    public Text scoreText;
    public int score = 1;
    public Text topText;
    public Text cointText;
    public int coints;
    public Text ballsCountText;

    [HideInInspector]
    public bool scatterWorked;
    [HideInInspector]
    public List<GameObject> listGameObjects = new List<GameObject>();
    IDictionary<Vector2, bool> listObjectsPos = new Dictionary<Vector2, bool>(); 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start ()
    {
        Time.timeScale = 1;
        FillPositionObjects();

        SetupBlock();
        SetupGifts();

        scoreText.text = "Wave: 0";
        ballsCountText.text = "BALLS: 1";
        coints = PlayerPrefs.GetInt("Coint", 0);
        cointText.text = PlayerPrefs.GetInt("Coint", 0).ToString();
        topText.text = "TOP " + PlayerPrefs.GetInt("Top", 0).ToString();
    }

    void FillPositionObjects()
    {
        for (int i = minWidthPos; i < maxWidthPos; i++)
        {
            for (int j = minHeightPos; j < maxHeightPos; j++)
            {
                listObjectsPos.Add(new Vector2(i,j), false);
            }
        }
    }

    void SetupBlock()
    { 
        for (int i = minWidthPos; i < maxWidthPos; i++)
        {
            for (int j = maxHeightPos; j > 3; j--)
            { 
                int rand = Random.Range(0, 4);
                if (rand == 1)
                {
                    Vector2 position = new Vector2(i, j);
                    GameObject newBlock = Instantiate(blocks[0], position, Quaternion.identity);
                    newBlock.name = "Block0";
                    newBlock.transform.SetParent(gameObjectsHolder);

                    listGameObjects.Add(newBlock);
                    listObjectsPos[position] = true;
                }
            }
        }
    }

    #region Смещение объектов на один вниз, а также добавление новой линии
    public void NextWave()
    { 
        foreach (GameObject item in listGameObjects)
        {  
            item.transform.position = new Vector2(item.transform.position.x, item.transform.position.y - 1);
        }

        for (int i = minWidthPos; i < maxWidthPos; i++)
        {
            int rand = Random.Range(0, 3);
            if (rand == 1)
            {
                int indexBlock = 0;
                int wave = GameManager.instance.score;
                 
                if (wave > 100) indexBlock = 4;
                else if (wave > 50) indexBlock = 3;
                else if (wave > 25) indexBlock = 2;
                else if (wave > 9) indexBlock = 1;

                GameObject newBlock = Instantiate(blocks[indexBlock], new Vector2(i, maxHeightPos), Quaternion.identity);
                newBlock.name = "Block" + indexBlock;
                newBlock.transform.SetParent(gameObjectsHolder);

                listGameObjects.Add(newBlock);
            }
        }

        CheckWorkedScatter();
        SetNewPosGameObjects();
        SetupGifts(); 
        AddScore();
         
        BallManager.instance.ballInFlight = false;
        ballsCountText.text = "BALLS: " + BallManager.instance.listBall.Count.ToString();
    }
    #endregion

    #region Добавление "способности"
    void SetupGifts() 
    { 
        for (int i = 0; i < gifts.Length - 1; i++)//PlusBall должен быть последним в списке
        {
            if (Random.Range(0, 2) == 1)
            {
                int index = Random.Range(0, gifts.Length-1);
                SetGiftOnFreePos(gifts[index]);
            }
        }

        SetupPlusBall();
    }

    void SetupPlusBall()
    {
        int wave = GameManager.instance.score;
        int rand = Random.Range(0, 5);

        if (wave > 100)
        {
            rand = Random.Range(0, 8);
            if (rand == 1) SetGiftOnFreePos(gifts[gifts.Length - 1]);
        }
        else if (wave > 50)
        {
            if (rand == 1) SetGiftOnFreePos(gifts[gifts.Length - 1]);
        }
        else if (wave > 25)
        {
            if (rand == 1 || rand == 2) SetGiftOnFreePos(gifts[gifts.Length - 1]);
        }
        else if (wave > 9)
        {
            if (rand == 1 || rand == 2 || rand == 3) SetGiftOnFreePos(gifts[gifts.Length - 1]);
        }
        else
        {
            SetGiftOnFreePos(gifts[gifts.Length - 1]);
        }
    }

    void SetGiftOnFreePos(GameObject gift)
    {
        int i, j;
        Vector2 occupiedPosition;

        do
        { 
            i = Random.Range(minWidthPos, maxWidthPos);
            j = Random.Range(0, maxHeightPos);
            occupiedPosition = new Vector2(i, j); 

        } while (listObjectsPos[occupiedPosition] == true); 

        GameObject newGift = Instantiate(gift, occupiedPosition, Quaternion.identity);
        newGift.transform.SetParent(gameObjectsHolder);

        listGameObjects.Add(newGift);
        listObjectsPos[occupiedPosition] = true;
    }
    #endregion

    void SetNewPosGameObjects()
    {
        listObjectsPos.Clear();
        FillPositionObjects();

        foreach (GameObject item in listGameObjects)
        {
            listObjectsPos[item.transform.position] = true;
        }
    }

    void CheckWorkedScatter()
    {
        if (scatterWorked)
        {
            GameObject[] scatters = GameObject.FindGameObjectsWithTag("Scatter");
            foreach (GameObject item in scatters)
            { 
                listGameObjects.Remove(item);
                Destroy(item); 
            }

            scatterWorked = false;
        }
    }

    public void AddCoint()
    {
        ++coints;
        cointText.text = coints.ToString();
        PlayerPrefs.SetInt("Coint", coints);
    }
    void AddScore()
    {
        ++score; 
        scoreText.text = "WAVE: " + score.ToString();

        if (score > PlayerPrefs.GetInt("Top", 0))
        {
            PlayerPrefs.SetInt("Top", score);
            topText.text = "TOP " + score.ToString();
        }
    }

    public void PlayParticle(Vector2 position)
    {
        ParticleSystem curExplosion = Instantiate(explosionBlock, position, Quaternion.identity);

        StartCoroutine(DestroyParticles(curExplosion));
    }
    IEnumerator DestroyParticles(ParticleSystem expParticle)
    { 
            yield return new WaitForSeconds(2f);
            Destroy(expParticle.gameObject);
    }

    public void GamePause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }
    public void GamePlay()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }
    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        SceneManager.LoadScene(1);
    }
    public void OnMain()
    {
       SceneManager.LoadScene(0);
    }
}
