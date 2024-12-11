using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GameManager : MonoBehaviour
{
    public bool firstTime;
    public float innerSize = 1f;
    public int gridOuterSize = 10;
    public int mineCount = 50;
    List<TileManager> mineList = new List<TileManager>();
    public GameObject tilePrefab;
    GameObject canvas;
    //�����������
    public TileManager[,] tiles;
    float initTime = 0;
    public AudioClip boom;
    public TextMeshProUGUI textBombNum;
    int bombLeft = 0;
    //�˺�������������ת��Ϊ��������,�������갴�ռ⳯�������Σ�x���ң�y�����ϣ��������������½�Ϊԭ��
    public Vector3 FromGridToWorld(int x, int y)
    {
        return new Vector3(innerSize * (x - (float)y / 2f), innerSize * y * 1.7320508f / 2, 0);
    }

    //���ɵ���
    public void setMine(int x, int y)
    {
        int xx = Random.Range(0, 2 * gridOuterSize + 1);
        int yy = Random.Range(0, 2 * gridOuterSize + 1);
        bool isNeighbor = false;
        foreach (TileManager neighbor in tiles[x, y].neighbors)
        {
            if (tiles[xx, yy] != null && (tiles[xx, yy] == neighbor || tiles[xx,yy] == tiles[x,y]))
                isNeighbor = true;
        }
        if (tiles[xx, yy] == null || tiles[xx, yy].isMine || isNeighbor)
        {
            setMine(x, y);
        }
        else
        {
            tiles[xx, yy].isMine = true;
            mineList.Add(tiles[xx, yy]);
        }
    }

    public void GenerateMines(int x, int y)
    {
        for (int i = 0; i < mineCount; i++)
        {
            setMine(x, y);
        }
        foreach (TileManager tile in tiles)
        {
            if (tile != null)
            {
                tile.mineNeighborNum = 0;
                foreach (TileManager neighbor in tile.neighbors)
                {

                    if (neighbor != null)
                    {
                        if (neighbor.isMine)
                            tile.mineNeighborNum++;
                    }
                }
            }
        }
    }

    //��������������
    void GenerateGrid()
    {
        for (int i = 0; i <= 2 * gridOuterSize; i++)
        {
            if (i < gridOuterSize)
            {
                for (int j = 0; j <= gridOuterSize + i; j++)
                {
                    tiles[i, j] = Instantiate(tilePrefab, FromGridToWorld(i, j), Quaternion.identity).GetComponent<TileManager>();
                    tiles[i, j].x = i;
                    tiles[i, j].y = j;
                }
            }
            if (i >= gridOuterSize)
            {
                for (int j = i - gridOuterSize; j <= 2 * gridOuterSize; j++)
                {
                    tiles[i, j] = Instantiate(tilePrefab, FromGridToWorld(i, j), Quaternion.identity).GetComponent<TileManager>();
                    tiles[i, j].x = i;
                    tiles[i, j].y = j;
                }
            }
        }
        //��ʼ�������ʼ����Ҫ������Ƭ����Ϣ��������Ҫ������������
        for (int i = 0; i <= 2 * gridOuterSize; i++)
        {
            for (int j = 0; j <= 2 * gridOuterSize; j++)
            {
                if (tiles[i, j] != null)
                {
                    tiles[i, j].initiallize();
                }
            }
        }
    }

    void Awake()
    {
        gridOuterSize = MainMenu.GetMenuManager().scale - 1;
        tiles = new TileManager[2 * gridOuterSize + 1, 2 * gridOuterSize + 1];
        mineCount = MainMenu.GetMenuManager().mineNum;
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.orthographicSize = innerSize * gridOuterSize;
        camera.transform.position = new Vector3((float)gridOuterSize / 2, gridOuterSize * 0.87f, -10);
        canvas = GameObject.Find("CanvasOver");
        canvas.SetActive(false);
        initTime = Time.time;
        //��������
        GenerateGrid();
        firstTime = true;
    }

    public void GameOver()
    {
        if (GameObject.Find("Main Camera").GetComponent<AudioSource>() != null)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Pause();
        }
        GameObject menu = GameObject.Find("MenuManager");
        if (menu != null)
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(boom, menu.GetComponent<MainMenu>().volume / 100f);
        }
        else
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(boom, 1f);
        }
        foreach (TileManager tile in tiles)
        {
            if (tile != null)
            {
                if (tile.isMine)
                {
                    tile.explode();
                }
            }
        }
        MainMenu.GetMenuManager().GameOver();
        canvas.SetActive(true);
        GameObject.Find("TextWinOrLose").GetComponent<TextMeshProUGUI>().text = "Lose!";
        GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>().text = "Time: " + (Time.time - initTime).ToString("F2") + "s";
    }

    public void CheckWin()
    {
        bool win = true;
        foreach (TileManager tile in tiles)
        {
            if (tile != null && (!tile.isMine && !tile.isRevealed))
                win = false;
        }
        if (win)
        {
            MainMenu.GetMenuManager().GameOver();
            canvas.SetActive(true);
            GameObject.Find("TextWinOrLose").GetComponent<TextMeshProUGUI>().text = "Win!";
            GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>().text = "Time: " + (Time.time - initTime).ToString("F2") + "s";
        }
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

    private void Update()
    {
        int flagNum = 0;
        foreach (TileManager tile in tiles)
        {
            if (tile != null && tile.isFlag)
            {
                flagNum++;
            }
        }
        bombLeft = mineCount - flagNum;
        textBombNum.text = bombLeft.ToString();
    }
}