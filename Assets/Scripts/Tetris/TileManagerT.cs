using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class TileManagerT : MonoBehaviour
{
    MapGenerater gameManager;
    float lastFallTime;
    MainMenu mainMenu;
    float SDownTime = 0;
    float ADownTime = 0;
    float DDownTime = 0;
    float LastMoveLeft = 0;
    float LastMoveRight = 0;
    float lastSDownTime = 0;
    float lastADownTime = 0;
    float lastDDownTime = 0;
    float timeInterval = 1f;
    void Awake()
    {
        mainMenu = MainMenu.GetMenuManager();
        mainMenu.ResumeGame2();
        gameManager = GameObject.Find("GameManager").GetComponent<MapGenerater>();
        timeInterval = gameManager.fallInterval;
    }
    void Update()
    {
        //向左移动
        if (Input.GetKeyDown(KeyCode.A) && !mainMenu.isStop)
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.A))
        {
            ADownTime += (Time.time - lastADownTime);
            lastADownTime = Time.time;
        }
        else if (Time.time - lastADownTime > 0.001f)
        {
            ADownTime = 0;
            lastADownTime = Time.time;
        }
        if (ADownTime > 0.2f)
        {
            for (;Time.time - LastMoveLeft > 0.05f; LastMoveLeft = Time.time)
            {
                MoveLeft();
            }
        }

        //向右移动
        if (Input.GetKeyDown(KeyCode.D) && !mainMenu.isStop)
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.D))
        {
            DDownTime += (Time.time - lastDDownTime);
            lastDDownTime = Time.time;
        }
        else if (Time.time - lastDDownTime > 0.001f)
        {
            DDownTime = 0;
            lastDDownTime = Time.time;
        }
        if (DDownTime > 0.2f)
        {
            for (; Time.time - LastMoveRight > 0.05f; LastMoveRight = Time.time)
            {
                MoveRight();
            }
        }

        //旋转
        if (Input.GetKeyDown(KeyCode.W) && !mainMenu.isStop)
        {
            transform.Rotate(0, 0, 60);
            Vector3 position = transform.position;
            while (toWall() == 1)
            {
                transform.position += new Vector3(-0.8660254037844386f, 0.5f, 0);
            }
            while (toWall() == 2)
            {
                transform.position += new Vector3(0.8660254037844386f, -0.5f, 0);
            }
            if (IsValidPosition())
            {
                UpdateGrid();
            }
            else
            {
                transform.position = position;
                transform.Rotate(0, 0, -60);
            }
        }
        //下落
        if ((Input.GetKeyDown(KeyCode.S) || Time.time - lastFallTime > timeInterval)&& !mainMenu.isStop
             && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D)//这是为了防止在左右移动遇到障碍时判定到顶
            )
        {
            GoDown();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            for (; GoDown();) { }
        }
        if(Input.GetKey(KeyCode.S))
        {
            SDownTime += (Time.time - lastSDownTime);
            lastSDownTime = Time.time;
        }
        else if(Time.time - lastSDownTime > 0.001f)
        {
            SDownTime = 0;
            lastSDownTime = Time.time;
        }
        if(SDownTime > (gameManager.keepKeyDownTime))
        {
            timeInterval = gameManager.fallInterval * 0.05f;
        }
        else
        {
            timeInterval = gameManager.fallInterval;
        }
    }
    //左右移动
    void MoveLeft()
    {
        transform.position += new Vector3(-0.8660254037844386f, -0.5f, 0);
        if (IsValidPosition())
        {
            UpdateGrid();
        }
        else
        {
            transform.position += new Vector3(0.8660254037844386f, 0.5f, 0);
        }
    }
    void MoveRight()
    {
        transform.position += new Vector3(0.8660254037844386f, -0.5f, 0);
        if (IsValidPosition())
        {
            UpdateGrid();
        }
        else
        {
            transform.position += new Vector3(-0.8660254037844386f, 0.5f, 0);
        }
    }
bool GoDown()
    {
        transform.position += new Vector3(0, -1, 0);
        if (IsValidPosition())
        {
            UpdateGrid();
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            gameManager.DeleteFullLines();
            //检测是否到顶
            bool isTop = false;
            for (int x = 0; x < MapGenerater.width; x++)
            {
                if (gameManager.gridsT[x, MapGenerater.height - 1] != null)
                {
                    isTop = true;
                }
            }
            if (isTop)
            {
                gameManager.GameOver();
                enabled = false;
                return false;
            }
            else
            {
                enabled = false;
                gameManager.SpawnerNext();
                return false;
            }
        }
        lastFallTime = Time.time;
        return true;
    }
    //获取位置
    Vector2 fromWorldToHex(Vector2 v)
    {
        int x = (int)Mathf.Round(v.x / (0.8660254037844386f * gameManager.innerSize));
        int y = (int)Mathf.Round((v.y / gameManager.innerSize) + 0.5f * x);
        return new Vector2(x, y);
    }
    public Vector2[] GetPositions()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        Vector2[] positions = new Vector2[children.Length - 1];
        for (int i = 1; i < children.Length; i++)
        {
            positions[i - 1] = gameManager.fromWorldToHex(children[i].position);
        }
        return positions;
    }

    public void UpdateGrid()
    {
        for (int y = 0; y < MapGenerater.height; y++)
        {
            for (int x = 0; x < MapGenerater.width; x++)
            {
                if (gameManager.gridsT[x, y] != null)
                {
                    //检测某一方块是否是该方块的一部分
                    if (gameManager.gridsT[x, y].parent == transform || gameManager.gridsT[x, y] == transform)
                    {
                        //移除旧的子方块
                        gameManager.gridsT[x, y] = null;
                    }
                }
            }
        }
        //更新网格
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        for (int j = 1; j < children.Length; j++)
        {
            Vector2 pos = fromWorldToHex(children[j].position);
            if (pos.y < MapGenerater.height)
            {
                gameManager.gridsT[(int)pos.x, (int)pos.y] = children[j];
            }
        }
    }
    //检测位置是否合理
    bool IsValidPosition()
    {
        Vector2[] positions = GetPositions();
        for (int i = 0; i < positions.Length; i++)
        {
            if (IsInGrid(positions[i]) == false)
            {
                return false;
            }
            if (positions[i].y < MapGenerater.height)
            {
                if (gameManager.gridsT[(int)positions[i].x, (int)positions[i].y] != null && gameManager.gridsT[(int)positions[i].x, (int)positions[i].y].parent != transform)
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool IsInGrid(Vector2 pos)
    {
        if (pos.x >= 0 && pos.x < MapGenerater.width && pos.y >= 0)
            return true;
        else
        {
            return false;
        }
    }

    int toWall()
    {
        Vector2[] positions = GetPositions();
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].x >= MapGenerater.width)
            {
                return 1;
            }
            if (positions[i].x < 0)
            {
                return 2;
            }
        }
        return 0;
    }
}
