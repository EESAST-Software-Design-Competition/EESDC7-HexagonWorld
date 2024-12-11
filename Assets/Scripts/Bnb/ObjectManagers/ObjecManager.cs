using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjecManager : MonoBehaviour
{
    int x;
    int y;
    GameManagerB gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManagerB").GetComponent<GameManagerB>();
        x = (int)PlayerManager.FromWorldToGrids(transform.position).x;
        y = (int)PlayerManager.FromWorldToGrids(transform.position).y;
        List<PlayerManager> players = gameManager.players;
        foreach (PlayerManager player in players)
        {
            AIController AI = player.gameObject.GetComponent<AIController>();
            if (AI != null)
            {
                AI.targets.Add(gameManager.gameGrid[x, y]);
            }
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            List<PlayerManager> players = gameManager.players;
            for (int i = 0; i < players.Count; i++)
            {
                PlayerManager player = players[i];
                if (player == null) 
                    continue;
                AIController AI = player.gameObject.GetComponent<AIController>();
                if (AI != null && AI.targets.Contains(gameManager.gameGrid[x, y]))
                {
                    AI.targets.Remove(gameManager.gameGrid[x, y]);
                }
            }
        }
    }
}
