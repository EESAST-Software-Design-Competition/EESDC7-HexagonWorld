using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManagerB : MonoBehaviour
{
    bool conditionBoom = false;
    bool boxBoomCondition = false;
    public PictureManagerB pictureManager;
    public TileManagerB[] neighbors = new TileManagerB[6];
    public int x;
    public int y;
    float boomTime;
    public enum TileType
    {
        ground, wall, box
    }
    public enum TileObject
    {
        none, bomb, blood, speed, bombScale
    }
    public TileType tileType;
    public TileObject tileObject;
    public bool boxable;
    public bool hasBomb = false;
    public List<BombManager> bombThere;
    public List<BombManager> goingToBoom;
    public GameObject objectThere;

    //以下的变量和AI寻路有关
    List<TileManagerB> parentForRunningAwayFromBomb = new List<TileManagerB>();
    List<AIController> AIOfparentForRunningAwayFromBomb = new List<AIController>();
    List<TileManagerB> parentForChasing = new List<TileManagerB>();
    List<AIController> AIOfparentForChasing = new List<AIController>();
    List<float> costForChasing = new List<float>();
    List<AIController> AIofcostForChasing = new List<AIController>();

    private void Awake()
    {
        pictureManager = PictureManagerB.Get();
    }
    public void ChangeToGround(bool box)
    {
        tileType = TileType.ground;
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pictureManager.ground;
        if (box)
        {
            boxable = true;
        }
        else
        {
            boxable = false;
        }
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public void ChangeToWall()
    {
        tileType = TileType.wall;
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pictureManager.wall;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void ChangeToBox()
    {
        tileType = TileType.box;
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pictureManager.box;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void ChangeToBomb()
    {
        hasBomb = true;
    }

    public void ChangeBackFromBomb()
    {
        hasBomb = false;
    }

    public void GoingToBomb(BombManager bomb)
    {
        goingToBoom.Add(bomb);
    }

    public void NotGoingToBomb(BombManager bomb)
    {
        goingToBoom.Remove(bomb);
    }

    public void Boom(BombManager bomb)
    {
        if (hasBomb)
        {
            for(int i = 0; i < bombThere.Count;i++)
            {
                if (bombThere[i] != null)
                    bombThere[i].Boom();
            }
        }
        NotGoingToBomb(bomb);
        if (tileType == TileType.wall)
        {
            return;
        }
        boomTime = Time.time;
        if (tileType == TileType.box)
        {
            ChangeToGround(true);
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pictureManager.boom;
            conditionBoom = true;
            boxBoomCondition = true;
        }
        else if (tileType == TileType.ground)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pictureManager.boom;
            conditionBoom = true;
        }
        Destroy(objectThere);
        List<PlayerManager> players = GameObject.Find("GameManagerB").GetComponent<GameManagerB>().players;
        for (int i = 0; i < players.Count; i++)
        {
            PlayerManager player = players[i];
            if (player != null && player.x == x && player.y == y)
            {
                player.Boom();
            }
        }
    }
    //这里是炸弹痕迹消失的动画
    void ChangeBack()
    {
        if (Time.time - boomTime >= 0.49f)
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = pictureManager.ground;
    }

    void ObjectGenerate()
    {
        if (tileObject == TileObject.speed)
        {
            objectThere = Instantiate(pictureManager.speedPrefab, transform.position, Quaternion.identity);
        }
        else if (tileObject == TileObject.blood)
        {
            objectThere = Instantiate(pictureManager.bloodPrefab, transform.position, Quaternion.identity);
        }
        else if (tileObject == TileObject.bomb)
        {
            objectThere = Instantiate(pictureManager.bombPrefab, transform.position, Quaternion.identity);
        }
        else if (tileObject == TileObject.bombScale)
        {
            objectThere = Instantiate(pictureManager.bombScalePrefab, transform.position, Quaternion.identity);
        }
    }
    //下列函数和AI寻路有关
    public void AddParentForRunningAwayFromBomb(TileManagerB parent, AIController AI)
    {
        parentForRunningAwayFromBomb.Add(parent);
        AIOfparentForRunningAwayFromBomb.Add(AI);
    }
    public void RemoveParentForRunningAwayFromBomb(AIController AI)
    {
        if (!AIOfparentForRunningAwayFromBomb.Contains(AI))
            return;
        int index = AIOfparentForRunningAwayFromBomb.IndexOf(AI);
        AIOfparentForRunningAwayFromBomb.RemoveAt(index);
        parentForRunningAwayFromBomb.RemoveAt(index);
    }
    public TileManagerB GetParentForRunningAwayFromBomb(AIController AI)
    {
        if (!AIOfparentForRunningAwayFromBomb.Contains(AI))
            return null;
        int index = AIOfparentForRunningAwayFromBomb.IndexOf(AI);
        return parentForRunningAwayFromBomb[index];
    }
    public void AddParentForChasing(TileManagerB parent, AIController AI)
    {
        parentForChasing.Add(parent);
        AIOfparentForChasing.Add(AI);
    }
    public void RemoveParentForChasing(AIController AI)
    {
        if (!AIOfparentForChasing.Contains(AI))
            return;
        int index = AIOfparentForChasing.IndexOf(AI);
        AIOfparentForChasing.RemoveAt(index);
        parentForChasing.RemoveAt(index);
    }
    public TileManagerB GetParentForChasing(AIController AI)
    {
        if (!AIOfparentForChasing.Contains(AI))
            return null;
        int index = AIOfparentForChasing.IndexOf(AI);
        return parentForChasing[index];
    }
    public void AddCostForChasing(float cost, AIController AI)
    {
        costForChasing.Add(cost);
        AIofcostForChasing.Add(AI);
    }
    public void RemoveCostForChasing(AIController AI)
    {
        if (!AIofcostForChasing.Contains(AI))
            return;
        int index = AIofcostForChasing.IndexOf(AI);
        AIofcostForChasing.RemoveAt(index);
        costForChasing.RemoveAt(index);
    }
    public float GetCostForChasing(AIController AI)
    {
        if (!AIofcostForChasing.Contains(AI))
            return 0f;
        int index = AIofcostForChasing.IndexOf(AI);
        return costForChasing[index];
    }

    private void Update()
    {
        if (conditionBoom && Time.time - boomTime > 0.5)
        {
            ChangeBack();
            conditionBoom = false;
            if (boxBoomCondition)
            {
                ObjectGenerate();
                boxBoomCondition = false;
            }
        }
        for (int i = 0; i < goingToBoom.Count; i++)
        {
            BombManager bomb = goingToBoom[i];
            if (goingToBoom[i] == null)
            {
                goingToBoom.RemoveAt(i);
            }
        }
    }
}
