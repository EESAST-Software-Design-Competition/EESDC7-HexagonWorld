using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class MainMenu : MonoBehaviour
{
    public bool isOver = false;
    public bool isStop = false;
    public List<GameObject> canvasList = new List<GameObject>();
    int[] fathers = new int[8] { 0, 0, 1, 0, 1, 1, 1, 1};
    int openCanvasIndex = 0;
    public int mode = 0;
    //下面是关于倒计时的一些变量
    string scene;
    bool isTiming1 = false;
    bool isTiming2 = false;
    float beginTiming = 0;
    int time = 0;
    //下面是关于扫雷的一些参数
    UnityEngine.UI.Slider sliderScale;
    UnityEngine.UI.Slider sliderMineNum;
    TMPro.TextMeshProUGUI textValueScale;
    TMPro.TextMeshProUGUI textValueMineNum;
    public int scale = 6;
    public int mineNum = 15;
    public GameObject sliderVolume;
    //下面是关于音量控制的一些参数
    public int volume = 100;
    public AudioClip click;
    //下面是单例模式，需要继承的都要在这里面写上
    private static MainMenu _instance;
    public static MainMenu Instance { get { return _instance;}}

    public static MainMenu GetMenuManager()
    {
        return _instance;
    }

    void Awake()
    {
        if (_instance != null)
        {
            this.volume = _instance.volume;
            Destroy(_instance.gameObject);
            _instance = this;
        }
        else
        {
            _instance = this;
        }
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);
        DontDestroyOnLoad(gameObject);
        //下面是控制音量的部分
        GameObject gameObj = GameObject.Find("Main Camera");
        if (gameObj.GetComponent<AudioSource>() != null)
        {
            gameObj.GetComponent<AudioSource>().volume = (float)volume / 100f;
        }
        //canvasList.Add(GameObject.Find("CanvasBegin0"));
        //canvasList.Add(GameObject.Find("CanvasSelect1"));
        //canvasList.Add(GameObject.Find("CanvasTime2"));
        //canvasList.Add(GameObject.Find("CanvasStop3"));
        //canvasList.Add(GameObject.Find("CanvasBnb4"));
        //canvasList.Add(GameObject.Find("CanvasSnake5"));
        //canvasList.Add(GameObject.Find("CanvasMine6"));
        canvasList[0].SetActive(true);
        canvasList[1].SetActive(false);
        canvasList[2].SetActive(false);
        canvasList[4].SetActive(false);
        canvasList[5].SetActive(false);
        sliderMineNum = GameObject.Find("SliderNum").GetComponent<UnityEngine.UI.Slider>();
        sliderScale = GameObject.Find("SliderScale").GetComponent<UnityEngine.UI.Slider>();
        textValueScale = GameObject.Find("TextValueScale").GetComponent<TMPro.TextMeshProUGUI>();
        textValueMineNum = GameObject.Find("TextValueNum").GetComponent<TMPro.TextMeshProUGUI>();
        SetPara1();
        canvasList[6].SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Begin()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);
        canvasList[0].SetActive(false);
        canvasList[1].SetActive(true);
        openCanvasIndex = 1;
    }

    public void Back()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);
        canvasList[openCanvasIndex].SetActive(false);
        canvasList[fathers[openCanvasIndex]].SetActive(true);
        openCanvasIndex = fathers[openCanvasIndex];
    }

//按下ESC键暂停游戏
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                Back();
                isTiming1 = false;
                isTiming2 = false;
                time = 0;
            }
            else if (!isStop)
            {
                StopGame();
            }
            else if (!isOver)
            {
                ResumeGame2();
            }
        }
        Select();
    }
    public void StopGame()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);
        if (GameObject.Find("Main Camera").GetComponent<AudioSource>() != null)
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Pause();
        }
        Time.timeScale = 0;
        isStop = true;
        canvasList[3].SetActive(true);
        sliderVolume.GetComponent<UnityEngine.UI.Slider>().value = volume;
        sliderVolume.GetComponentInChildren<TextMeshProUGUI>().text = volume.ToString();
    }

    public void ResumeGame()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);
        GameObject gameObj = GameObject.Find("Main Camera");
        if (gameObj.GetComponent<AudioSource>() != null)
        {
            gameObj.GetComponent<AudioSource>().volume = (float)volume / 100f;
        }
        if (gameObj.GetComponent<AudioSource>() != null)
        {
            gameObj.GetComponent<AudioSource>().UnPause();
        }
        Time.timeScale = 1;
        isStop = false;
        canvasList[3].SetActive(false);
    }
    public void ResumeGame2()
    {
        GameObject gameObj = GameObject.Find("Main Camera");
        if (gameObj.GetComponent<AudioSource>() != null)
        {
            gameObj.GetComponent<AudioSource>().volume = (float)volume / 100f;
        }
        if (gameObj.GetComponent<AudioSource>() != null)
        {
            gameObj.GetComponent<AudioSource>().UnPause();
        }
        Time.timeScale = 1;
        isStop = false;
        canvasList[3].SetActive(false);
    }

    public void RestartGame()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);

        Time.timeScale = 1;
        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        isStop = false;
        canvasList[3].SetActive(false);
    }

    public void ExitGame()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);

        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    //倒计时三秒
    void Select()
    {
        if (isTiming1 && Time.time - beginTiming > time)
        {
            time++;
            canvasList[2].GetComponentsInChildren<TMPro.TextMeshProUGUI>()[0].text = (4 - time).ToString();

        }
        if (isTiming1 && isTiming1 && time == 3)
        {
            SceneManager.LoadScene(scene);
            time = 0;
            isTiming1 = false;
        }
        if (isTiming2 && Time.time - beginTiming > 3.3)
        {
            //下面是控制音量的部分
            GameObject gameObj = GameObject.Find("Main Camera");
            if (gameObj.GetComponent<AudioSource>() != null)
            {
                gameObj.GetComponent<AudioSource>().volume = (float)volume / 100f;
            }
            isTiming2 = false;
            time = 0;
        }
    }
    public void SelectCanvas(int index)
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);

        canvasList[openCanvasIndex].SetActive(false);
        openCanvasIndex = index;
        canvasList[index].SetActive(true);
    }

    public void SelectScene(string str)
    {
        scene = str;
        canvasList[openCanvasIndex].SetActive(false);
        canvasList[2].SetActive(true);
        openCanvasIndex = 2;
        gameObject.GetComponent<AudioSource>().PlayOneShot(click, volume / 100f);
        beginTiming = Time.time;
        time = 0;
        isTiming1 = true;
        isTiming2 = true;
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }

    public void GameOver()
    {
        isStop = true;
        isOver = true;
        Time.timeScale = 0;
    }

    //下面是关于扫雷的一些函数
    public void SetPara1()
    {
        sliderMineNum.maxValue = (int)(((int)sliderScale.value * (int)sliderScale.value * 3 - 3 * (int)sliderScale.value + 1 )* 0.5f);
        textValueMineNum.text = ((int)sliderMineNum.value).ToString();
        textValueScale.text = ((int)sliderScale.value).ToString();
    }
    public void SetPara2()
    {
        scale = (int)sliderScale.value;
        mineNum = (int)sliderMineNum.value;
    }
    public void SetScale(int scale)
    {
        this.scale = scale;
    }
    public void SetMineNum(int num)
    {
        this.mineNum = num;
    }
    //下面是关于音量控制的函数
    public void SetVolume()
    {
        volume = (int)sliderVolume.GetComponent<UnityEngine.UI.Slider>().value;
        sliderVolume.GetComponentInChildren<TextMeshProUGUI>().text = volume.ToString();
        GameObject gameObj = GameObject.Find("Main Camera");
        if (gameObj.GetComponent<AudioSource>() != null)
        {
            gameObj.GetComponent<AudioSource>().volume = (float)volume / 100f;
        }
    }
}
