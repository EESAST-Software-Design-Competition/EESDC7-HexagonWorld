using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;


public class MapGenerater : MonoBehaviour
{
    public const int width = 10;
    public const int height = 20;
    public float innerSize = 1.0f;
    bool FirstTime = true;
    int nextIndex = 0;
    public Transform[,] gridsT = new Transform[width,height]; 
    public GameObject tilePrefab;
    public GameObject[] blocks;
    GameObject canvasOver;
    public Sprite[] blocksSprite;
    public int score = 0;
    int level = 0;
    int lines = 0;
    public float fallInterval = 1f;
    public float keepKeyDownTime = 0.3f;
    float[] fallTimeIntervals = new float[19] { 0.8f, 0.71f, 0.63f, 0.55f, 0.47f, 0.38f, 0.3f, 0.22f, 0.13f, 0.1f, 0.08f, 0.08f, 0.08f, 0.066f, 0.066f, 0.066f, 0.05f, 0.05f, 0.05f };
    int[] numToScore = new int[5] { 0, 40, 100, 300, 1200 };
    float initTime = 0;
    int num = 2;//用于实现前四个方块的随机无重复生成
    int[] BeginSome;
    public AudioClip sound;
    void Awake()
    {
        keepKeyDownTime = 0.5f;
        BeginSome = GenerateRandomNum(blocks.Length);
        canvasOver = GameObject.Find("CanvasOver");
        canvasOver.SetActive(false);
        generateMap();
        SpawnerNext();
        initTime = Time.time;
    }

    Vector2 fromHexToWorld(int x, int y)
    {
        float xPos = x * 0.8660254037844386f * innerSize;
        float yPos = (y - 0.5f * x) * innerSize;
        return new Vector2(xPos, yPos);
    }

    public Vector2 fromWorldToHex(Vector2 v)
    {
        int x = (int)Mathf.Round (v.x / (0.8660254037844386f * innerSize));
        int y = (int)Mathf.Round (v.y / innerSize + 0.5f * x);
        return new Vector2(x, y);
    }

    void generateMap()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y % 2 == 0)
                { //偶数行
                    Instantiate(tilePrefab, fromHexToWorld(x, y), Quaternion.identity).transform.GetComponentInChildren<SpriteRenderer>().color = new Color(0.9f,0.9f,0.9f,1);
                }
                else
                { //奇数行
                    Instantiate(tilePrefab, fromHexToWorld(x, y), Quaternion.identity);
                }
            }
        }
    }

    public void SpawnerNext()
    {

        if (FirstTime)
        {
            int i1 = BeginSome[0];
            int i2 = BeginSome[1];
            //产生方块
            Vector2 pos = fromHexToWorld(width / 2, height + 3);
            Instantiate(blocks[i1], pos, Quaternion.identity);
            nextIndex = i2;
            GameObject.Find("Image").GetComponent<Image>().sprite = blocksSprite[i2];
            FirstTime = false;
        }
        else 
        {
            Vector2 pos = fromHexToWorld(width / 2, height + 1);
            Instantiate(blocks[nextIndex], pos, Quaternion.identity);
            nextIndex = num >= blocks.Length? Random.Range(0, blocks.Length) : BeginSome[num++];
            GameObject.Find("Image").GetComponent<Image>().sprite = blocksSprite[nextIndex];
        }
        UpdateData();
    }

    //生成四个数的随机排列
    int[] GenerateRandomNum(int count)
    {
        int[] nums = new int[count];
        for (int j = 0; j < count; j++)
        {
            nums[j] = -1;
        }
        int i = 0;
        while (i < count)
        {
            int num = Random.Range(0, count);
            while (nums.Contains(num))
            {
                num = Random.Range(0, count);
            }
            nums[i] = num;
            i++;
        }
        return nums;
    }

        void UpdateData()
    {
        level = lines / 10;
        fallInterval = level < 19 ? fallTimeIntervals[level] : fallTimeIntervals[18];
        GameObject.Find("Text").GetComponent<TextMeshProUGUI>().text = "Score: " + score + "\nLevel: " + level + "\nLines: " + lines;
    }
    

    //判断一行是否填满
    public bool IsFullLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (gridsT[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }
    //删除一行
    public void DeleteLine(int y)
    {
        GameObject menu = GameObject.Find("MenuManager");
        if (menu != null)
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(sound, menu.GetComponent<MainMenu>().volume / 100f);
        }
        else
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(sound, 1f);
        }
        for (int x = 0; x < width; x++)
        {
            Destroy(gridsT[x, y].gameObject);
            gridsT[x, y] = null;
        }
        lines++;
    }
    //上面一行下降
    public void MoveLineDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (gridsT[x, y] != null)
            {
                gridsT[x, y - 1] = gridsT[x, y];
                gridsT[x, y] = null;
                gridsT[x, y - 1].position += Vector3.down;
            }
        }
    }

    //使上面所有行下降
    public void MoveAllLinesDown(int y)
    {
        for (int i = y + 1; i < MapGenerater.height; i++)
        {
            MoveLineDown(i);
        }
    }

    //删除所有填满的行
    public void DeleteFullLines()
    {
        int count = 0;
        for (int y = 0; y < MapGenerater.height; y++)
        {
            if (IsFullLine(y))
            {
                DeleteLine(y);
                count++;
                MoveAllLinesDown(y);
                y--;
            }
        }
        score += numToScore[count] * (level + 1);
    }

    public void GameOver()
    {
        if (GameObject.Find("Main Camera").GetComponent<AudioSource>() != null)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Pause();
        }
        MainMenu.GetMenuManager().GameOver();
        canvasOver.SetActive(true);
        GameObject.Find("TextOver").GetComponent<TextMeshProUGUI>().text = "Score: " + score + "\nLevel:" + level + "\nlines:" + lines + "\ntime:" + (Time.time - initTime).ToString("F2") + "s";
        GameObject.Find("Main Camera").GetComponent<AudioSource>().Stop();
    }

    // 下面是GameOverUI的按钮事件
    public void ReStartAfterOver()
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

