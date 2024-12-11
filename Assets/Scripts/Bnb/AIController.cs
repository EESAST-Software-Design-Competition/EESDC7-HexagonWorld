using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    bool haveInitialised = false;
    bool inDanger = false;
    static int width = 13;
    static int height = 13;
    GameManagerB gameManager;
    public TileManagerB[,] gameGrid;
    PlayerManager playerManager;
    List<TileManagerB> path = new List<TileManagerB>();
    public List<TileManagerB> targets = new List<TileManagerB>();
    public int targetNum = 0;
    float lastTimeUpdate = 0;
    int difficulty = 1;
    float targetCostFactor = 1.5f;
    float targetPlayerFactor = 2f;
    float targetBloodFactor = 1f;//��������Ϊblood�ĺ���
    float targetBombFactor = 1f;
    float targetSpeedFactor = 1.2f;
    float targetBoomScaleFactor = 1f;
    float boxFactor = 1.7f;//�˶��߶����������cost��
    float boomFactor = 1.1f;//�˶��߶����������cost��
    float updatePeriod = 0.5f;
    public void Initialise()
    {
        lastTimeUpdate = Time.time;
        gameManager = GameObject.Find("GameManagerB").GetComponent<GameManagerB>();
        playerManager = gameObject.GetComponent<PlayerManager>();
        width = GameManagerB.width;
        height = GameManagerB.height;
        gameGrid = gameManager.gameGrid;
        playerManager = gameObject.GetComponent<PlayerManager>();
        StartCoroutine(this.FindPathEveryPeriodOfTime());
        haveInitialised = true;
    }

    private void Update()
    {
        if (haveInitialised)
        {
            if (Time.time - lastTimeUpdate > updatePeriod)
            {
                StartCoroutine(this.FindPathEveryPeriodOfTime());
                lastTimeUpdate = Time.time;
            }
            GoAlongThePath();
            if (inDanger && gameGrid[playerManager.x, playerManager.y].goingToBoom == null)
            {
                inDanger = false;
            }
            if (difficulty == 3)
            {
                //�ı�blood��Ȩ��
                if (playerManager.blood > 3)
                {
                    targetBloodFactor = 1f;
                }
                if (playerManager.blood == 3)
                {
                    targetBloodFactor = 1.2f;
                }
                if (playerManager.blood == 2)
                {
                    targetBloodFactor = 1.4f;
                }
                if (playerManager.blood == 1)
                {
                    targetBloodFactor = 1.8f;
                }
                //�ı�player��Ȩ��
                if (playerManager.bombNum == 0)
                {
                    targetPlayerFactor = 0.8f;
                }
                else
                {
                    targetPlayerFactor = 2f;
                }
            }
        }
    }

    void GoAlongThePath()
    {
        if (path.Count == 0)
        {
            playerManager.v = 0;
            playerManager.h = 0;
        }
        if (path.Count > 0)
        {
            if (path.Count == 1)
            {
                targetNum = 0;
            }
            else if (path[1].goingToBoom.Count != 0 && path[0].goingToBoom.Count == 0 && difficulty == 3)
            {
                targetNum = 0;
            }
            //������ը���Ӳ���
            else if (path[1].tileType == TileManagerB.TileType.box && path[1].goingToBoom.Count == 0)//����Ҫը�����ӵ�����
            {
                targetNum = 0;
                if (playerManager.bombNum > 0 && playerManager.xFront == path[0].x && playerManager.yFront == path[0].y)
                {
                    playerManager.SetBomb();
                }
            }

            //else if (path.Count > 2 && path[2].tileType == TileManagerB.TileType.box && path[2].goingToBoom.Count == 0)//����Ҫը�����ӵ�����
            //{
            //    targetNum = 1;
            //    if(playerManager.bombNum > 0 && playerManager.xFront == path[1].x && playerManager.yFront == path[1].y)
            //    {
            //        playerManager.SetBomb();
            //    }
            //}
            else
            {
                targetNum = 1;
            }
            //����������Ŀ��ǰ���Ĺ���
            Vector2 target = new(path[targetNum].transform.position.x, path[targetNum].transform.position.y);
            Vector2 current = new Vector2(transform.position.x, transform.position.y);
            if (target.x - current.x > 0.1)
            {
                playerManager.h = 1;
            }
            else if (target.x - current.x < -0.1)
            {
                playerManager.h = -1;
            }
            else
            {
                playerManager.h = 0;
            }
            if (target.y - current.y > 0.1)
            {
                playerManager.v = 1;
            }
            else if (target.y - current.y < -0.1)
            {
                playerManager.v = -1;
            }
            else
            {
                playerManager.v = 0;
            }
        }
        //if (path.Count > 1 && (gameGrid[playerManager.x, playerManager.y] == path[0] || gameGrid[playerManager.x, playerManager.y] == path[1]))
        //{
        //    if (path[1].x - path[0].x == 1 && path[1].y - path[0].y == 0)
        //    {
        //        playerManager.h = 1;
        //        playerManager.v = 0;
        //    }
        //    if (path[1].x - path[0].x == -1 && path[1].y - path[0].y == 0)
        //    {
        //        playerManager.h = -1;
        //        playerManager.v = 0;
        //    }
        //    if (path[1].x - path[0].x == 0 && path[1].y - path[0].y == 1)
        //    {
        //        playerManager.h = -1;
        //        playerManager.v = 1;
        //    }
        //    if (path[1].x - path[0].x == 0 && path[1].y - path[0].y == -1)
        //    {
        //        playerManager.h = 1;
        //        playerManager.v = -1;
        //    }
        //    if (path[1].x - path[0].x == 1 && path[1].y - path[0].y == 1)
        //    {
        //        playerManager.h = 1;
        //        playerManager.v = 1;
        //    }
        //    if (path[1].x - path[0].x == -1 && path[1].y - path[0].y == -1)
        //    {
        //        playerManager.h = -1;
        //        playerManager.v = -1;
        //    }

        //}
        if (path.Count > 1 && Mathf.Abs(path[1].transform.position.x - playerManager.transform.position.x) < 0.1f && Mathf.Abs(path[1].transform.position.y - playerManager.transform.position.y) < 0.06)
        //if (gameGrid[playerManager.x,playerManager.y] == path[1])
        {
            path.Remove(path[0]);
        }
    }


    public void GetMessageOfBombUpdate()
    {
        if (gameGrid[playerManager.x, playerManager.y] != null && gameGrid[playerManager.x, playerManager.y].goingToBoom.Count > 0)
        {
            inDanger = true;
            StartCoroutine(RunAwayFromBomb());
        }
        else
        {
            inDanger = false;
        }
    }

    IEnumerator RunAwayFromBomb()
    {
        //�����ÿ�������㷨Ѱ·
        List<TileManagerB> openTiles = new List<TileManagerB>();
        List<TileManagerB> beingCheckedTiles = new List<TileManagerB>();
        List<TileManagerB> closeTiles = new List<TileManagerB>();
        openTiles.Add(gameGrid[playerManager.x, playerManager.y]);
        gameGrid[playerManager.x, playerManager.y].AddParentForRunningAwayFromBomb(gameGrid[playerManager.x, playerManager.y], this);
        while (true)
        {
            foreach (TileManagerB tile in openTiles)
            {
                foreach (TileManagerB neighbor in tile.neighbors)
                {
                    if (neighbor != null && !closeTiles.Contains(neighbor) && neighbor.tileType == TileManagerB.TileType.ground && !(neighbor.hasBomb && IsMyBomb(neighbor)) && gameManager.hasPlayer(tile, playerManager) == null)//����������ʱͨ·������
                    {

                        if (neighbor.goingToBoom.Count > 0)
                        {
                            beingCheckedTiles.Add(neighbor);
                            neighbor.AddParentForRunningAwayFromBomb(tile, this);
                        }
                        else
                        {
                            neighbor.AddParentForRunningAwayFromBomb(tile, this);
                            //����
                            List<TileManagerB> pathTemp = new List<TileManagerB>();
                            TileManagerB current = neighbor;
                            while (current.GetParentForRunningAwayFromBomb(this) != current)
                            {
                                pathTemp.Add(current);
                                TileManagerB temp = current.GetParentForRunningAwayFromBomb(this);
                                current.RemoveParentForRunningAwayFromBomb(this);
                                current = temp;
                            }
                            pathTemp.Add(gameGrid[playerManager.x, playerManager.y]);
                            pathTemp.Reverse();
                            path.Clear();
                            for (int i = 0; i < pathTemp.Count; i++)
                            {
                                path.Add(pathTemp[i]);
                            }
                            foreach (TileManagerB tile1 in openTiles)
                            {
                                tile1.RemoveParentForRunningAwayFromBomb(this);
                            }
                            foreach (TileManagerB tile1 in beingCheckedTiles)
                            {
                                tile1.RemoveParentForRunningAwayFromBomb(this);
                            }
                            foreach (TileManagerB tile1 in closeTiles)
                            {
                                tile1.RemoveParentForRunningAwayFromBomb(this);
                            }
                            yield break;
                        }
                    }
                }
            }
            if (beingCheckedTiles.Count == 0)
            {
                //��·һ��
                path.Clear();
                path.Add(gameGrid[playerManager.x, playerManager.y]);
                foreach (TileManagerB tile1 in openTiles)
                {
                    tile1.RemoveParentForRunningAwayFromBomb(this);
                }
                foreach (TileManagerB tile1 in beingCheckedTiles)
                {
                    tile1.RemoveParentForRunningAwayFromBomb(this);
                }
                foreach (TileManagerB tile1 in closeTiles)
                {
                    tile1.RemoveParentForRunningAwayFromBomb(this);
                }
                yield break;
            }
            //�����д򿪵ĸ��Ӷ�����closeTiles
            foreach (TileManagerB tile in openTiles)
            {
                closeTiles.Add(tile);
            }
            openTiles.Clear();
            //�����е�beingCheckedTiles����openTiles
            foreach (TileManagerB tile in beingCheckedTiles)
            {
                openTiles.Add(tile);
            }
            beingCheckedTiles.Clear();
            //һ������ѭ������
        }
    }

    //�ж�tile���ը���Ƿ����Լ�������
    bool IsMyBomb(TileManagerB tile)
    {
        foreach (BombManager bomb in tile.goingToBoom)
        {
            if (bomb.player == playerManager)
            {
                return true;
            }
        }
        return false;
    }
    IEnumerator FindPathEveryPeriodOfTime()
    {
        //������Ѱ·����
        if (!inDanger)
        {
            FindPathForChasing();
        }
        else
        {
            StartCoroutine(RunAwayFromBomb());
        }
        //����ը��ҵĲ���
        if (playerManager.bombNum > 0)
        {
            bool findPlayer = false;
            foreach (PlayerManager player in gameManager.players)//����ȡ���˲�ըAI���趨,������ע��Ļ���
            {
                if (!player.isAI && player != playerManager && playerManager.xFront == player.x && playerManager.yFront == player.y)
                {
                    findPlayer = true;
                }
                foreach (TileManagerB neighbor in gameGrid[playerManager.xFront, playerManager.yFront].neighbors)
                {
                    if (!player.isAI && player != playerManager && neighbor != null && neighbor.x == player.x && neighbor.y == player.y)
                    {
                        findPlayer = true;
                    }
                }
            }
            if (findPlayer)
            {
                playerManager.SetBomb();
            }
        }
        yield break;
    }

    void FindPathForChasing()
    {
        ////�ȸ���һ��Target
        //foreach(PlayerManager player in gameManager.players)
        //{
        //    /*if(player != null && !player.isAI)*/if(player != null && player != playerManager)// ֻ�ѷ�AI����ҵ���Ŀ�� //��ʱȡ�����Ҫ��
        //    {
        //        player.ReportPosition(this);
        //    }
        //}
        if (!inDanger)
        {
            //�������A*�㷨Ѱ·
            List<TileManagerB> openTiles = new List<TileManagerB>();
            List<TileManagerB> closedTiles = new List<TileManagerB>();
            openTiles.Add(gameGrid[playerManager.x, playerManager.y]);
            gameGrid[playerManager.x, playerManager.y].AddParentForChasing(gameGrid[playerManager.x, playerManager.y], this);
            gameGrid[playerManager.x, playerManager.y].AddCostForChasing(0, this);
            while (openTiles.Count > 0)
            {
                TileManagerB current = FindTheCostMin(openTiles);
                if (isTarget(current))
                {
                    //Ѱ·�������ǵ����Parent
                    List<TileManagerB> pathTemp = new List<TileManagerB>();
                    TileManagerB currentTemp = current;
                    while (currentTemp.GetParentForChasing(this) != currentTemp)
                    {
                        pathTemp.Add(currentTemp);
                        TileManagerB temp = currentTemp.GetParentForChasing(this);
                        currentTemp.RemoveParentForChasing(this);
                        currentTemp = temp;
                    }
                    currentTemp.RemoveParentForChasing(this);
                    pathTemp.Add(currentTemp);
                    if (current.objectThere == null)
                    {
                        pathTemp.Remove(current);
                    }
                    pathTemp.Reverse();
                    path.Clear();
                    for (int i = 0; i < pathTemp.Count; i++)
                    {
                        path.Add(pathTemp[i]);
                    }
                    foreach (TileManagerB tile1 in openTiles)
                    {
                        tile1.RemoveParentForChasing(this);
                        tile1.RemoveCostForChasing(this);
                    }
                    foreach (TileManagerB tile1 in closedTiles)
                    {
                        tile1.RemoveParentForChasing(this);
                        tile1.RemoveCostForChasing(this);
                    }
                    return;
                }
                else
                {
                    current.RemoveCostForChasing(this);
                    openTiles.Remove(current);
                    closedTiles.Add(current);
                    foreach (TileManagerB neighbor in current.neighbors)
                    {
                        if (neighbor != null && !closedTiles.Contains(neighbor) && !openTiles.Contains(neighbor) && neighbor.tileType != TileManagerB.TileType.wall && !(gameManager.hasPlayer(neighbor, playerManager) && !targets.Contains(neighbor)))//��������ͨ·������
                        {
                            neighbor.AddParentForChasing(current, this);
                            neighbor.AddCostForChasing(CalculateCost(neighbor), this);
                            openTiles.Add(neighbor);
                        }
                    }
                }
            }
            //Ѱ·ʧ��
            path.Clear();
            path.Add(gameGrid[playerManager.x, playerManager.y]);
            foreach (TileManagerB tile1 in openTiles)
            {
                tile1.RemoveParentForChasing(this);
                tile1.RemoveCostForChasing(this);
            }
            foreach (TileManagerB tile1 in closedTiles)
            {
                tile1.RemoveParentForChasing(this);
                tile1.RemoveCostForChasing(this);
            }
            return;
        }
    }
    TileManagerB FindTheCostMin(List<TileManagerB> openTiles)
    {
        float minCost = openTiles[0].GetCostForChasing(this);
        TileManagerB minTile = openTiles[0];
        foreach (TileManagerB tile in openTiles)
        {
            if (tile.GetCostForChasing(this) < minCost)
            {
                minCost = tile.GetCostForChasing(this);
                minTile = tile;
            }
        }
        return minTile;
    }

    float CalculateCost(TileManagerB tile)
    {
        //���ȼ��㵽���Ĵ���
        int costFromBegining = 0;
        TileManagerB current = tile;
        while (current.GetParentForChasing(this) != current)
        {
            costFromBegining += 1;
            current = current.GetParentForChasing(this);
        }
        //Ȼ����㵽Ŀ��Ĵ���
        float[] targetCost = new float[targets.Count];
        for (int i = 0; i < targets.Count; i++)
        {
            float delta1 = Mathf.Abs(tile.x - targets[i].x);
            float delta2 = Mathf.Abs(tile.y - targets[i].y);
            float delta3 = Mathf.Abs((tile.y - targets[i].y) - (tile.x - targets[i].x));
            //�ҵ������е����ֵ
            float maxDelta = delta1;
            if (delta2 > maxDelta)
            {
                maxDelta = delta2;
            }
            if (delta3 > maxDelta)
            {
                maxDelta = delta3;
            }
            //�������
            targetCost[i] = delta1 + delta2 + delta3 - maxDelta;
        }
        //����Ŀ���Ӱ������
        float[] factors = new float[targets.Count];
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].objectThere != null)
            {
                switch (targets[i].tileObject)
                {
                    case TileManagerB.TileObject.bomb:
                        factors[i] = targetBombFactor;
                        break;
                    case TileManagerB.TileObject.blood:
                        factors[i] = targetBloodFactor;
                        break;
                    case TileManagerB.TileObject.bombScale:
                        factors[i] = targetBoomScaleFactor;
                        break;
                    case TileManagerB.TileObject.speed:
                        factors[i] = targetSpeedFactor;
                        break;
                }
            }
            else
            {
                factors[i] = targetPlayerFactor;
            }
        }

        //ÿ��Ŀ����һ��·�������潫ȡ���ߵĵ���ƽ��������֤·����̵�������Ӱ�����ӣ�Ȼ���ٳ���Ŀ���Ӱ������
        //Ҳ����ѡ�����ƽ����
        double reciprocalSum = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            reciprocalSum += factors[i] / targetCost[i];
        }
        float factorSum = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            factorSum += factors[i];
        }
        reciprocalSum /= factorSum;
        float costToTarget = (float)(1 / reciprocalSum);
        float Barrierfactor;

        if (tile.tileType == TileManagerB.TileType.box)
        {
            Barrierfactor = boxFactor;
        }
        else
        {
            Barrierfactor = 1f;
        }
        if (tile.goingToBoom.Count != 0)
        {
            Barrierfactor = boomFactor;
        }
        float cost = costFromBegining + costToTarget * targetCostFactor * Barrierfactor;
        return cost;
    }

    bool isTarget(TileManagerB tile)
    {
        foreach (TileManagerB target in targets)
        {
            if (tile == target)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void SetDifficulty(int diffi)
    {
        difficulty = diffi;
        if(diffi == 1)
        {
            targetPlayerFactor = 1f;
            targetBloodFactor = 0.8f;//��������Ϊblood�ĺ���
            targetBombFactor = 1.5f;
            targetSpeedFactor = 0.8f;
            targetBoomScaleFactor = 1.5f;
            boxFactor = 1f;//�˶��߶����������cost��
            boomFactor = 1f;//�˶��߶����������cost��
            updatePeriod = 1.5f;
        }
        if (diffi == 2)
        {
            targetPlayerFactor = 2f;
            targetBloodFactor = 1.2f;//��������Ϊblood�ĺ���
            targetBombFactor = 1f;
            targetSpeedFactor = 1f;
            targetBoomScaleFactor = 1f;
            boxFactor = 1.2f;//�˶��߶����������cost��
            boomFactor = 1f;//�˶��߶����������cost��
            updatePeriod = 1f;
        }
        if (diffi == 3)
        {
            targetPlayerFactor = 2f;
            targetBloodFactor = 1f;//��������Ϊblood�ĺ���
            targetBombFactor = 1f;
            targetSpeedFactor = 1.2f;
            targetBoomScaleFactor = 1f;
            boxFactor = 1.7f;//�˶��߶����������cost��
            boomFactor = 1.1f;//�˶��߶����������cost��
            updatePeriod = 0.3f;
        }
    }
}





