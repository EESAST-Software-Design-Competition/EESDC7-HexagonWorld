using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TileManagerB;

public class GameManagerB : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject playerPrefab;
    public static int width = 13;
    public static int height = 13;
    public TileManagerB[,] gameGrid;
    public static float innerSize = 1.0f;
    public List<PlayerManager> players;
    public int boxNum = 6;
    public int bombObjNum = 6;
    public int speedObjNum = 6;
    public int bloodObjNum = 6;
    public int bombScaleObjNum = 6;
    int AICountInit = 0;
    float initTime = 0;
    bool init = false;
    GameObject canvas;

    //�˺�������������ת��Ϊ��������,�������갴�ռ⳯�������Σ�x���ң�y�����ϣ��������������½�Ϊԭ��
    public static Vector2 FromGridToWorld(int x, int y)
    {
        return new Vector2(innerSize * (x - (float)y / 2f), innerSize * y * 1.7320508f / 2);
    }

    void MapGenerater(int mode)
    {
        
        string filePath = Path.Combine(Application.streamingAssetsPath, "map" + mode.ToString() + ".txt");
        StreamReader streamReader = new(filePath);
        string line1 = streamReader.ReadLine();
        width = int.Parse(line1);//��һ�п��

        string line2 = streamReader.ReadLine();//
        height = int.Parse(line2);//�ڶ��и߶�
        gameGrid = new TileManagerB[width, height];
        string line3 = streamReader.ReadLine();//���������������λ��
        string line4 = streamReader.ReadLine();//���������������λ��
        string line5 = streamReader.ReadLine();//���������������
        string line6 = streamReader.ReadLine();//��������������
        string line7 = streamReader.ReadLine();//��������ը����Ʒ��
        string line8 = streamReader.ReadLine();//�ڰ������ٶ���Ʒ��
        string line9 = streamReader.ReadLine();//�ھ�����Ѫ��Ʒ��
        string line10 = streamReader.ReadLine();//��ʮ�����׵�scale��Ʒ��
        string line11 = streamReader.ReadLine();//��ʮ������ҵ�Ѫ��
        string line12 = streamReader.ReadLine();//��ʮһ����AIˢ��ʱ�䣨�Ѷȣ�
        GameObject.Find("Main Camera").transform.position = new Vector3(float.Parse(line3), float.Parse(line4), -10);
        GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = float.Parse(line5);
        boxNum = int.Parse(line6);
        bombObjNum = int.Parse(line7);
        speedObjNum = int.Parse(line8);
        bloodObjNum = int.Parse(line9);
        bombScaleObjNum = int.Parse(line10);
        string line;
        for (int x = 0; streamReader.Peek() >= 0; x++)
        {
            line = streamReader.ReadLine();
            for (int y = 0; y < line.Length; y++)
            {
                if (line[y] != '0')
                {
                    gameGrid[x, y] = Instantiate(tilePrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<TileManagerB>();
                    gameGrid[x, y].x = x;
                    gameGrid[x, y].y = y;
                    if (line[y] == '1')
                    {
                        gameGrid[x, y].ChangeToGround(true);
                    }
                    if (line[y] == '2')
                    {
                        gameGrid[x, y].ChangeToWall();
                    }
                    if (line[y] == '3')
                    {
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == '4')//���0
                    {
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 0;
                        temp.isAI = false;
                        temp.Init(0);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == '5')//���1
                    {
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 1;
                        temp.isAI = false;
                        temp.Init(1);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == '6')//AI1
                    {
                        AICountInit++;
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 2;
                        temp.isAI = true;
                        temp.Init(2);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == '7')//AI2
                    {
                        AICountInit++;
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 3;
                        temp.isAI = true;
                        temp.Init(3);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == '8')//AI3
                    {
                        AICountInit++;
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 4;
                        temp.isAI = true;
                        temp.Init(4);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == '9')//AI4
                    {
                        AICountInit++;
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 5;
                        temp.isAI = true;
                        temp.Init(5);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                    if (line[y] == 'A')//AI5
                    {
                        AICountInit++;
                        PlayerManager temp = Instantiate(playerPrefab, FromGridToWorld(x, y), Quaternion.identity).GetComponent<PlayerManager>();
                        temp.playerNum = 6;
                        temp.isAI = true;
                        temp.Init(6);
                        players.Add(temp);
                        gameGrid[x, y].ChangeToGround(false);
                    }
                }
            }
        }

        streamReader.Close();
        foreach (TileManagerB tile in gameGrid)
        {
            if (tile != null)
            {
                int x = tile.x;
                int y = tile.y;
                if (x > 1 && gameGrid[x - 1, y] != null)
                {
                    tile.neighbors[0] = gameGrid[x - 1, y];
                }
                if (x < width - 1 && gameGrid[x + 1, y] != null)
                {
                    tile.neighbors[1] = gameGrid[x + 1, y];
                }
                if (y > 1 && gameGrid[x, y - 1] != null)
                {
                    tile.neighbors[2] = gameGrid[x, y - 1];
                }
                if (y < height - 1 && gameGrid[x, y + 1] != null)
                {
                    tile.neighbors[3] = gameGrid[x, y + 1];
                }
                if (x > 1 && y > 1 && gameGrid[x - 1, y - 1] != null)
                {
                    tile.neighbors[4] = gameGrid[x - 1, y - 1];
                }
                if (x < width - 1 && y < height - 1 && gameGrid[x + 1, y + 1] != null)
                {
                    tile.neighbors[5] = gameGrid[x + 1, y + 1];
                }
            }
        }
        foreach (PlayerManager player in players)
        {
            if (player != null && !player.isAI)
                player.blood = int.Parse(line11);
            else if (player != null && player.isAI)
            {
                int diffi = int.Parse(line12);
                player.GetComponent<AIController>().SetDifficulty(diffi);
                //player.GetComponent<AIController>().updatePeriod = float.Parse(line12);
            }
        }
        BoxGenerater();
        ObjectGenerater();
        init = true;
    }

    //��������
    void BoxGenerater()
    {
        for (int i = 0; i < boxNum; i++)
        {
            GenerateOneBox();
        }
    }

    void GenerateOneBox()
    {
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);
        if (gameGrid[x, y] != null && gameGrid[x, y].tileType == TileManagerB.TileType.ground && gameGrid[x,y].boxable)
        {
            gameGrid[x, y].ChangeToBox();
        }
        else
        {
            GenerateOneBox();
        }
    }

    void ObjectGenerater()
    {
        for (int i = 0; i < bombObjNum; i++)
        {
            GenerateOneObject(TileObject.bomb);
        }
        for (int i = 0; i < speedObjNum; i++)
        {
            GenerateOneObject(TileObject.speed);
        }
        for (int i = 0; i < bloodObjNum; i++)
        {
            GenerateOneObject(TileObject.blood);
        }
        for (int i = 0; i < bombScaleObjNum; i++)
        {
            GenerateOneObject(TileObject.bombScale);
        }
    }
    
    void GenerateOneObject(TileObject tileObject)
    {
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);
        if (gameGrid[x, y] != null && gameGrid[x, y].tileType == TileManagerB.TileType.box)
        {
            gameGrid[x, y].tileObject = tileObject;
        }
        else
        {
            GenerateOneObject(tileObject);
        }
    }


    void Awake()
    {
        Physics.simulationMode = SimulationMode.Update;
        MapGenerater(GameObject.Find("MenuManager").GetComponent<MainMenu>().mode);
        //����ì�ܵ���Ҫ����Panel����ʼ����Ҫʱ�䣩����playerNum����������panel��Sprite
        canvas = GameObject.Find("CanvasOver");
        canvas.SetActive(false);
        initTime = Time.time;
    }

    private void Update()
    {
        if (init)
        {
            if (players.Count == 1)
            {
                GameOver(players[0].playerNum);
                return;
            }
            else
            {
                int countPlayer = 0;
                int countAI = 0;
                foreach (PlayerManager player in players)
                {
                    if (player != null && player.isAI == false)
                        countPlayer++;
                    else if (player != null && player.isAI)
                        countAI++;
                }
                if (countAI == 0 && AICountInit != 0)
                {
                    GameOver(-2);//-2��ʾ���ʤ��
                }
                else if (countPlayer == 0)
                {
                    GameOver(-1);//-1��ʾAIʤ��
                }
            }
        }
    }

    public PlayerManager hasPlayer(TileManagerB tile, PlayerManager player1)//���������AI��Ŀ��ص��
    {
        foreach (PlayerManager player in players)
        {
            AIController AI = player.gameObject.GetComponent<AIController>();
            if (player != null && player != player1 && player.x == tile.x && player.y == tile.y || (player.isAI && AI.targetNum < AI.targets.Count && AI.targets[AI.targetNum] == tile))
                return player;
        }
        return null;
    }

    public void GameOver(int winerNum)
    {
        MainMenu.GetMenuManager().GameOver();
        canvas.SetActive(true);
        if (GameObject.Find("Main Camera").GetComponent<AudioSource>() != null)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Pause();
        }
        if (winerNum >= 0)
            GameObject.Find("WinerHead").GetComponent<Image>().sprite = PictureManagerB.Get().player[winerNum];
        else if (winerNum == -2)
        {
            GameObject.Find("WinerHead").GetComponent<Image>().sprite = PictureManagerB.Get().players;
        }
        else if (winerNum == -1)
        {
            GameObject.Find("WinerHead").GetComponent<Image>().sprite = PictureManagerB.Get().AIs;
        }
        GameObject.Find("TextTime").GetComponent<TMPro.TextMeshProUGUI>().text = "Time: " + (Time.time - initTime).ToString("F2") + "s";
    }

    //������GameOverUI����
    public void RestartAfterOver()
    {
        MainMenu.GetMenuManager().isOver = false;
        MainMenu.GetMenuManager().isStop = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void BackAfterOver()
    {

        MainMenu.GetMenuManager().isOver = false;
        MainMenu.GetMenuManager().isStop = false;
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}

