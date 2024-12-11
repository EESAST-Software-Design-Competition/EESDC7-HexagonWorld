using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static TileManagerB;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerManager : MonoBehaviour
{
    //创建玩家时注意初始化玩家编号
    public int playerNum;
    public int blood;
    public int score;
    public int bombNum = 1;
    public Vector2 direction;
    public float speed = 3;
    public GameObject bulletPrefab;
    public int bombScale = 1;
    float bombTime = 0;
    public int x, y;
    public int xFront, yFront;
    public int h = 0, v = 0;
    Vector2 initDirection = new Vector2(1,0);
    GameObject panel;
    MainMenu mainMenu;
    public bool isAI = true;
    bool isRed = false;
    Vector2Int positionReported;
    float lastTimeReport;
    GameManagerB gameManager;
/// <summary>
/// 下面是基本控制和玩家的控制
/// </summary>
    void Awake()
    {
        lastTimeReport = Time.time + 10;
        mainMenu = MainMenu.GetMenuManager();
        mainMenu.ResumeGame2();
        GetPosition();
        positionReported = new Vector2Int(x, y);
    }
    public void Init(int index)
    {
        gameManager = GameObject.Find("GameManagerB").GetComponent<GameManagerB>();
        blood = 2;
        panel = GameObject.Find("Canvas").GetComponent<UIControler>().GeneratePanel();
        UpdatePanel();
        playerNum = index;
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = PictureManagerB.Get().player[index];
        panel.GetComponentsInChildren<Image>()[1].sprite = PictureManagerB.Get().player[index];
        if (!isAI)
        {
            gameObject.GetComponent<AIController>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<AIController>().Initialise();
        }
        lastTimeReport = Time.time;
    }

    void Update()
    {
        //private void FixedUpdate()
        //{
        
        if (playerNum == 0 && !mainMenu.isStop && !isAI)
        {
            if(Input.GetKey(KeyCode.W)) {v = 1;}
            if(Input.GetKey(KeyCode.S)) {v = -1;}
            if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
            { v = 0; }
            if(Input.GetKey(KeyCode.A)) {h = -1;}
            if(Input.GetKey(KeyCode.D)) {h = 1;}
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            { h = 0; }
        }
        if (playerNum == 1 && !mainMenu.isStop && !isAI)
        {
            if (Input.GetKey(KeyCode.UpArrow)) { v = 1; }
            if (Input.GetKey(KeyCode.DownArrow)) { v = -1; }
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
            { v = 0; }
            if (Input.GetKey(KeyCode.LeftArrow)) { h = -1; }
            if (Input.GetKey(KeyCode.RightArrow)) { h = 1; }
            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
            { h = 0; }
        }
        Move();
        //下面是投掷炸弹部分
        if (Input.GetKeyDown(KeyCode.Space ) && playerNum == 0 && !mainMenu.isStop || Input.GetKeyDown(KeyCode.KeypadEnter) && playerNum == 1 && !mainMenu.isStop)
        {
            SetBomb();
        }
        //下面是汇报位置的函数//其中涉及到是否把AI作为自己的敌人
        if(!isAI && Time.time - lastTimeReport > 0.5)
        {
            for (int i = 0; i < gameManager.players.Count; i++)
            {
                PlayerManager player = gameManager.players[i];
                if (player != this && player.isAI)
                {
                    AIController AI = player.gameObject.GetComponent<AIController>();
                    if (AI.targets.Contains(AI.gameGrid[positionReported.x, positionReported.y]))
                    {
                        AI.targets.Remove(AI.gameGrid[positionReported.x, positionReported.y]);
                    }
                    AI.targets.Add(AI.gameGrid[x, y]);
                }
            }
            positionReported = new Vector2Int(x, y);
            lastTimeReport = Time.time;
        }
        //下面是从红色转变回来的过程
        if (isRed && Time.time - bombTime > 0.49f)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            isRed = false;
        }

    }

    private void Move()
    {

        if (h == 1)
        {
            if (v == 0)
            {
                direction = new Vector2(1, 0);
            }
            if (v == 1)
            {
                direction = new Vector2(0.5f, 0.866f);
            }
            if (v == -1)
            {
                direction = new Vector2(0.5f, -0.866f);
            }
        }
        if (h == -1)
        {
            if (v == 0)
            {
                direction = new Vector2(-1, 0);
            }
            if (v == 1)
            {
                direction = new Vector2(-0.5f, 0.866f);
            }
            if (v == -1)
            {
                direction = new Vector2(-0.5f, -0.866f);
            }
        }
        if (h == 0)
        {
            if (v == 1)
            {
                direction = new Vector2(0, 1);
            }
            if (v == -1)
            {
                direction = new Vector2(0, -1);
            }
            if (v == 0)
            {
                direction = new Vector2(0, 0);
            }
        }
        //transform.position += (Vector3)direction * speed * Time.deltaTime;
        gameObject.GetComponent<Rigidbody2D>().velocity = direction * speed;
        if (direction != new Vector2(0, 0))
            transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(initDirection, direction));
        //}

        //void Update()
        //{
        GetPosition();
    }

    public void SetBomb()
    {
        if (bombNum > 0)
        {
            GameManagerB gameManager = GameObject.Find("GameManagerB").GetComponent<GameManagerB>();
            if (gameManager.gameGrid[xFront, yFront] != null && gameManager.gameGrid[xFront, yFront].tileType == TileType.ground && !gameManager.gameGrid[xFront, yFront].hasBomb)
            {
                GameObject boom = Instantiate(bulletPrefab, GameManagerB.FromGridToWorld(xFront, yFront), Quaternion.identity);
                bombNum--;
                UpdatePanel();
                gameManager.gameGrid[xFront, yFront].ChangeToBomb();
                boom.GetComponent<BombManager>().player = this;
                boom.GetComponent<BombManager>().scale = bombScale;
                boom.GetComponent<BombManager>().x = xFront;
                boom.GetComponent<BombManager>().y = yFront;
            }
        }
    }
    public static Vector2 FromWorldToGrids(Vector3 v)
    {
        int yy = (int)Mathf.Round(v.y * 2 / 1.7320508f / GameManagerB.innerSize);
        int xx = (int)Mathf.Round(v.x / GameManagerB.innerSize + yy / 2);
        float x1 = (GameManagerB.FromGridToWorld(xx, yy).x + GameManagerB.FromGridToWorld(xx - 1, yy).x) / 2f;
        float x2 = (GameManagerB.FromGridToWorld(xx, yy).x + GameManagerB.FromGridToWorld(xx + 1, yy).x) / 2f;
        if (v.x + v.y < GameManagerB.FromGridToWorld(xx, yy).x + GameManagerB.FromGridToWorld(xx, yy).y && v.x < x1)
            return new Vector2(xx - 1, yy);
        if (v.x + v.y > GameManagerB.FromGridToWorld(xx, yy).x + GameManagerB.FromGridToWorld(xx, yy).y && v.x > x2)
            return new Vector2(xx + 1, yy);
        if (v.x + v.y > GameManagerB.FromGridToWorld(xx, yy).x + GameManagerB.FromGridToWorld(xx, yy).y && 1.7320508f * v.y - v.x > 1.7320508f * GameManagerB.FromGridToWorld(xx - 1, yy).y - GameManagerB.FromGridToWorld(xx - 1, yy).x)
            return new Vector2(xx, yy + 1);
        if (v.x + v.y < GameManagerB.FromGridToWorld(xx, yy).x + GameManagerB.FromGridToWorld(xx, yy).y && 1.7320508f * v.y - v.x < 1.7320508f * GameManagerB.FromGridToWorld(xx - 1, yy - 1).y - GameManagerB.FromGridToWorld(xx - 1, yy - 1).x)
            return new Vector2(xx, yy - 1);
        return new Vector2(xx, yy);
    }

    void GetPosition()
    {
        x = (int)FromWorldToGrids(transform.position).x;
        y = (int)FromWorldToGrids(transform.position).y;
        xFront = (int)FromWorldToGrids(transform.position + 0.1f * (Vector3)direction).x;
        yFront = (int)FromWorldToGrids(transform.position + 0.1f * (Vector3)direction).y;
    }

    public void Boom()
    {
        bombTime = Time.time;
        blood -= 1;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        isRed = true;
        if (blood <= 0)
        {
            //GameObject.Find("GameManagerB").GetComponent<GameManagerB>().GameOver(1 - playerNum);
            for (int i = 0; i < gameManager.players.Count; i++)
            {
                PlayerManager player = gameManager.players[i];
                if (player != this && player.isAI)
                {
                    AIController AI = player.gameObject.GetComponent<AIController>();
                    if (AI.targets.Contains(AI.gameGrid[positionReported.x, positionReported.y]))
                    {
                        AI.targets.Remove(AI.gameGrid[positionReported.x, positionReported.y]);
                    }
                }
            }
            GameObject.Find("GameManagerB").GetComponent<GameManagerB>().players.Remove(this);
            this.UpdatePanel();
            Destroy(gameObject);
        }
        else
        {
            this.UpdatePanel();
        }
    }
    void ResetColor()
    {

    }

    public void UpdatePanel()
    {
        TextMeshProUGUI[] children = panel.GetComponentsInChildren<TextMeshProUGUI>();
         children[0].text = blood.ToString();
         children[1].text = bombNum.ToString();
         children[2].text = bombScale.ToString();
    }
    //下面是向AI汇报位置的部分[doge]

}

