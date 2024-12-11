//���������ʣ�����̰������Ϸ���������ɣ�ע����������Ķ�ʱ��Ҫ��Ӧ�Ķ��������λ��
//�Ķ���ͷ��ʼλ��ʱ��Ҫ�������Ϊ����*innerSize��������Ϊ0.866*innerSize��������

using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class Grids : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject[] headPrefabs;
    public GameObject boundaryPrefab;
    //������Ԥ����
    public GameObject foodPrefab;
    //ʳ��Ԥ����
    public bool pairMode;
    public const int rows = 30; //�����������������ż��
    public const int cols = 50; //���������
    public float innerSize = 1f; //���������Բ�뾶
    public int noneFood = 5; //�ھ�߽�nonefood��Χ�ڽ���������ʳ��
    // Start is called before the first frame update
    public int foodNum; //���ɵ�ʳ������
    public float timeInterval = 0.12f; //��Ϸʱ����
    public float timeLeft = 300; //��Ϸʣ��ʱ��
    private float timeBefore; //˫��ģʽ����ʱ
    public GameObject canvas;
    void Awake()
    {
        timeBefore = Time.time;
        canvas = GameObject.Find("CanvasOver");
        canvas.SetActive(false);
        GenerateGrid(rows, cols);
        pairMode = (GameObject.Find("MenuManager").GetComponent<MainMenu>().mode == 1);
        if (pairMode)
        {
            foodNum = 8;
            Snake snake1 = Instantiate(headPrefabs[0], new Vector3(10, 8.66025f, 0), Quaternion.Euler(0, 0, -90)).GetComponent<Snake>();
            Snake snake2 = Instantiate(headPrefabs[1], new Vector3(40, 8.66025f, 0), Quaternion.Euler(0, 0, 90)).GetComponent<Snake>();
            snake1.Init(1, true, this);
            snake2.Init(2, true, this);
        }
        else
        {
            foodNum = 1;
            Snake snake1 = Instantiate(headPrefabs[0], new Vector3(10, 8.66025f, 0), Quaternion.Euler(0, 0, -90)).GetComponent<Snake>();
            snake1.Init(1, false, this);
            GameObject.Find("Panel").SetActive(false);
            GameObject.Find("Panel2").SetActive(false);
        }
        for (int i = 0; i < foodNum; i++)
            GenerateFood();
    }

    //�˺�������������ת��Ϊ��������,�������갴�ռ⳯�������Σ�x���ң�y�����ϣ��������������½�Ϊԭ��
    public Vector3 FromGridToWorld(int x, int y)
    {
        return new Vector3(innerSize * (x - (float)y / 2f), innerSize * y * 1.7320508f / 2,0);
    }

    public Vector2Int ToGrid(Vector3 pos)
    {
        int y = (int) (pos.y / (innerSize * 1.7320508f / 2));
        int x = (int) (pos.x / innerSize) + (int) ((float)y / 2f);
        return new Vector2Int(x, y);
    }

    //���ɾ���ƽ������
    public void GenerateGrid(int rows, int cols)
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (i == 0 || j == 0 || i == cols - 1 || j == rows - 1)
                {
                    Instantiate(boundaryPrefab, FromGridToWorld((int)Mathf.Floor(j / 2) + i, j), Quaternion.identity);
                }
                else
                {
                    Instantiate(backgroundPrefab, FromGridToWorld((int)Mathf.Floor(j / 2) + i, j), Quaternion.identity);
                }
            }
        }
    }

    //����ʳ��
    public void GenerateFood()
    {   
        int x = (int) Random.Range(noneFood, cols - noneFood);
        int y = (int) Random.Range(noneFood, rows - noneFood);
        if (GameObject.Find("HeadPrefab") != null)
        {
            foreach (Transform tile in GameObject.Find("HeadPrefab").GetComponent<Snake>().tile)
            {
                Vector2Int grid = ToGrid(tile.position);
                int x1 = grid.x - (int)Mathf.Floor(grid.y / 2);
                int y1 = grid.y;
                if (x1 == x && y1 == y)
                {
                    GenerateFood();
                    return;
                }
            }
        }
        if (GameObject.Find("HeadPrefab 2") != null)
        {
            foreach (Transform tile in GameObject.Find("HeadPrefab").GetComponent<Snake>().tile)
            {
                Vector2Int grid = ToGrid(tile.position);
                if (grid.x == x && grid.y == y)
                {
                    GenerateFood();
                    return;
                }
            }
        }
        Instantiate(foodPrefab, FromGridToWorld(x + (int)Mathf.Floor(y / 2), y), Quaternion.identity);
    }

    //������˫��ģʽ����ʱ����
    private void FixedUpdate()
    {
        if(pairMode && Time.time - timeBefore >= 1)
        {
            timeBefore = Time.time;
            timeLeft--;
            GameObject.Find("TextTime").GetComponent<TextMeshProUGUI>().text = timeLeft.ToString();
        }
        if (timeLeft <= 0)
        {
            canvas.SetActive(true);
            MainMenu.GetMenuManager().GameOver();
            if (GameObject.Find("HeadPrefab").GetComponent<Snake>().score > GameObject.Find("HeadPrefab 2").GetComponent<Snake>().score)
            {
                GameObject.Find("TextOver").GetComponent<TMPro.TextMeshProUGUI>().text = "Player 1 Wins!";
            }
            else if (GameObject.Find("HeadPrefab").GetComponent<Snake>().score < GameObject.Find("HeadPrefab 2").GetComponent<Snake>().score)
            {
                GameObject.Find("TextOver").GetComponent<TMPro.TextMeshProUGUI>().text = "Player 2 Wins!";
            }
            else
            {
                GameObject.Find("TextOver").GetComponent<TMPro.TextMeshProUGUI>().text = "It's a tie!";
            }
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

}