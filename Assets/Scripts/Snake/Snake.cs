using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    public bool pairMode;
    public int playerNum;
    float timeInterval;
    public GameObject bodyPrefab;
    bool ate = false;
    public List<Transform> tile = new List<Transform>();
    Vector3 direction = new Vector3(0f, 1f, 0f);
    MainMenu mainMenu;
    public int score = 0;
    GameObject canvas;
    float initTime;
    float updateTime;
    public Grids grids;
    bool hasInit = false;

    // Start is called before the first frame update
    public void Init(int playerNum, bool pairMode, Grids grids)
    {
        this.playerNum = playerNum;
        this.pairMode = pairMode;
        this.grids = grids;
        initTime = Time.time;
        updateTime = Time.time;
        mainMenu = MainMenu.GetMenuManager();
        mainMenu.ResumeGame2();
        GameObject.Find("TextScores" + playerNum.ToString()).GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString();
        grids = GameObject.Find("GridSpawner").GetComponent<Grids>();
        timeInterval = grids.timeInterval;
        ate = true;
        Move(); ate = true;
        Move(); ate = true;
        Move();
        hasInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasInit)
        {
            if (hasInit && playerNum == 1)
            {
                if (Input.GetKeyDown(KeyCode.A) && !mainMenu.isStop)
                {
                    transform.Rotate(0, 0, 60);
                }
                if (Input.GetKeyDown(KeyCode.D) && !mainMenu.isStop)
                {
                    transform.Rotate(0, 0, -60);
                }
            }
            else if (hasInit && playerNum == 2)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) && !mainMenu.isStop)
                {
                    transform.Rotate(0, 0, 60);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow) && !mainMenu.isStop)
                {
                    transform.Rotate(0, 0, -60);
                }
            }
        }
        if (hasInit && Time.time - updateTime > timeInterval)
        {
            Move();
            updateTime = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Food")
        {
            ate = true;
            Destroy(other.gameObject);
            score += 10;
            GameObject.Find("TextScores" + playerNum.ToString()).GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString();
            GameObject.Find("GridSpawner").GetComponent<Grids>().GenerateFood();
        }
        else if (pairMode)
        {
            bool isSelf = false;
            foreach (Transform t in tile)
            {
                if (t.gameObject == other.gameObject)
                {
                    isSelf = true;
                    break;
                }
            }
            if (!isSelf)
            {
                GameOver();
            }
        }
        else if (! pairMode)
        {
             GameOver();
        }
    }
    void Move()
    {
        Vector3 pos1 = transform.position;
        transform.Translate(direction);
        if (ate)
        {
            tile.Insert(0, Instantiate(bodyPrefab, pos1, Quaternion.identity).transform);
            ate = false;
        }
        else if (tile.Count > 0)
        {
            tile[tile.Count - 1].position = pos1;
            tile.Insert(0, tile[tile.Count - 1]);
            tile.RemoveAt(tile.Count - 1);
        }
    }

    void GameOver()
    {
        if (GameObject.Find("Main Camera").GetComponent<AudioSource>() != null)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Pause();
        }
        mainMenu.GameOver();
        GameObject.Find("GridSpawner").GetComponent<Grids>().canvas.SetActive(true);
        if (!pairMode)
        {
            GameObject.Find("TextOver").GetComponent<TMPro.TextMeshProUGUI>().text = "Game Over!\nScore: " + score.ToString();
        }
        if(pairMode)
        {
            GameObject.Find("TextOver").GetComponent<TMPro.TextMeshProUGUI>().text = "Player" + playerNum.ToString() + " Wins!";
        }
    }
}

