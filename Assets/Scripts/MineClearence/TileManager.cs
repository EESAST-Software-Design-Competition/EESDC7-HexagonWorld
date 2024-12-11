using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.Windows.WebCam;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TileManager : MonoBehaviour
{
    bool mouseOver;
    public List<TileManager> neighbors;
    public bool isMine;
    public bool isFlag;
    public bool isRevealed;
    public int x;
    public int y;
    public int mineNeighborNum;
    SpriteRenderer rendererSprite;
    GameManager gameManager;
    MainMenu mainMenu;
    public void initiallize()
    {
        mainMenu = MainMenu.GetMenuManager();
        mainMenu.ResumeGame2();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rendererSprite = GetComponent<SpriteRenderer>();
        isMine = false;
        isFlag = false;
        isRevealed = false;
        rendererSprite.sprite = PictureManager.get().grass;
        TileManager[,] tiles = gameManager.tiles;
        neighbors = new List<TileManager>();
        if (x != gameManager.gridOuterSize * 2 && tiles[x + 1, y] != null)
            neighbors.Add(tiles[x + 1, y]);
        if (x != 0 && tiles[x - 1, y] != null)
            neighbors.Add(tiles[x - 1, y]);
        if (y != gameManager.gridOuterSize * 2 && tiles[x, y + 1] != null)
            neighbors.Add(tiles[x, y + 1]);
        if (y != 0 && tiles[x, y - 1] != null)
            neighbors.Add(tiles[x, y - 1]);
        if (x != gameManager.gridOuterSize * 2 && y != gameManager.gridOuterSize * 2 && tiles[x + 1, y + 1] != null)
            neighbors.Add(tiles[x + 1, y + 1]);
        if (x != 0 && y != 0 && tiles[x - 1, y - 1] != null)
            neighbors.Add(tiles[x - 1, y - 1]);
    }


    void Update()
    {
        if (mouseOver && UnityEngine.Input.GetMouseButtonDown(0) && !mainMenu.isStop)
        {
            if (!isRevealed && !isFlag)
            {
                if (gameManager.firstTime)
                {
                    gameManager.GenerateMines(x, y);
                    gameManager.firstTime = false;
                }
                if (isMine)
                {
                    gameManager.GameOver();
                }
                else
                {
                    reveal();
                }
                gameManager.CheckWin();
            }
        }
        if (mouseOver && UnityEngine.Input.GetMouseButtonDown(1) && !isRevealed && !mainMenu.isStop)
        {
            if (!isFlag)
            {
                isFlag = true;
                rendererSprite.sprite = PictureManager.get().flag;
            }
            else
            {
                isFlag = false;
                rendererSprite.sprite = PictureManager.get().grass;
            }
        }
    }
    private void OnMouseEnter()
    {
        mouseOver = true;
        if (!isRevealed && !isFlag)
        {
            rendererSprite.sprite = PictureManager.get().grassWithMouseOn;
        }
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        if (!isRevealed && !isFlag)
        { 
            rendererSprite.sprite = PictureManager.get().grass;
        }
    }

    public void reveal()
    {
        if (!isRevealed && !isMine)
        {
            isRevealed = true;
            if (mineNeighborNum == 0)
            {
                foreach (TileManager neighbor in neighbors)
                {
                    if (!neighbor.isRevealed)
                    {
                        neighbor.reveal();
                    }
                }
                rendererSprite.sprite = PictureManager.get().dirt;
            }
            else
            {
                rendererSprite.sprite = PictureManager.get().numberSprites[mineNeighborNum - 1];
            }
        }
    }

    public void explode()
    {
        isRevealed = true;
        rendererSprite.sprite = PictureManager.get().explosion;
    }
}
