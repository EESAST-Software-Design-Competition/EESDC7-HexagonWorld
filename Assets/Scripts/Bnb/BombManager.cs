using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileManagerB;

public class BombManager : MonoBehaviour
{
    float initTime;
    public int x;
    public int y;
    public int scale = 1;
    float timeGap = 4f;
    //List<Collider2D> triggerStay = new List<Collider2D>();
    bool triggerStay = false;
    Collider2D isMoving = null;
    public PlayerManager player;
    Vector2 direction = new(0, 0);
    public float speed = 10f;
    GameManagerB gameManager;
    bool hasUpdate = false;
    bool hasBoom = false;
    public AudioClip sound;
    private void Awake()
    {
        initTime = Time.time;
        gameManager = GameObject.Find("GameManagerB").GetComponent<GameManagerB>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == player.gameObject)
            triggerStay = true;
        //if(!triggerStay.Contains(other))
        //{
        //    triggerStay.Add(other);
        //}
    }
    void OnTriggerExit2D(Collider2D other)
    {
        //if (/*triggerStay.Contains(other)*/triggerStay)
        //    triggerStay.Remove(other);
        //if (!TriggerTrapSomeone())
        //{
        //    UpdateState();
        //}
        if (other.gameObject == player.gameObject)
        {
            triggerStay = false;
            UpdateState();
        }
    }
    

    //bool TriggerTrapSomeone()
    //{
    //    if (!triggerStay)
    //    {
    //        return false;
    //    }
    //    for(int i = 0; i < triggerStay.Count; i++)
    //    {
    //        if (triggerStay[i] != null)
    //        {
    //            PlayerManager player = triggerStay[i].gameObject.GetComponent<PlayerManager>();
                
    //        }
    //    }
    //    return false;
    //}
    void UpdateState()
    {
        if (!triggerStay/*!TriggerTrapSomeone*/)
        {
            this.gameObject.GetComponent<CircleCollider2D>().isTrigger = false;
            this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        gameManager.gameGrid[x, y].hasBomb = true;
        gameManager.gameGrid[x, y].bombThere.Add(this);
        gameManager.gameGrid[x, y].GoingToBomb(this);
        for (int i = 1; i <= scale; i++)
        {
            if (x > i && gameManager.gameGrid[x - i, y] != null)
            {
                gameManager.gameGrid[x - i, y].GoingToBomb(this);
            }
            if (x > i && gameManager.gameGrid[x - i, y].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (x < GameManagerB.width - i && gameManager.gameGrid[x + i, y] != null)
            {
                gameManager.gameGrid[x + i, y].GoingToBomb(this);
            }
            if (x < GameManagerB.width - i && gameManager.gameGrid[x + i, y].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (x < GameManagerB.width - i && y < GameManagerB.height - i && gameManager.gameGrid[x + i, y + i] != null)
            {
                gameManager.gameGrid[x + i, y + i].GoingToBomb(this);
            }
            if (x < GameManagerB.width - i && y < GameManagerB.height - i && gameManager.gameGrid[x + i, y + i].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (x > i && y > i && gameManager.gameGrid[x - i, y - i] != null)
            {
                gameManager.gameGrid[x - i, y - i].GoingToBomb(this);
            }
            if (x > i && y > i && gameManager.gameGrid[x - i, y - i].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (y > i && gameManager.gameGrid[x, y - i] != null)
            {
                gameManager.gameGrid[x, y - i].GoingToBomb(this);
            }
            if (y > i && gameManager.gameGrid[x, y - i].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (y < GameManagerB.height - i && gameManager.gameGrid[x, y + i] != null)
            {
                gameManager.gameGrid[x, y + i].GoingToBomb(this);
            }
            if (y < GameManagerB.height - i && gameManager.gameGrid[x, y + i].tileType == TileType.wall)
            {
                break;
            }
        }
        SendMessageOfBoomUpdate();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isMoving != null && isMoving != collision.collider)
        {
            //停下
            isMoving = null;
            x = (int)PlayerManager.FromWorldToGrids(transform.position - 0.25f * (Vector3)direction).x;
            y = (int)PlayerManager.FromWorldToGrids(transform.position - 0.25f * (Vector3)direction).y;
            direction = new(0, 0);
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            transform.position = GameManagerB.FromGridToWorld(x, y);
            UpdateState();
            return;
        }
        if (collision.gameObject.GetComponent<PlayerManager>() != null && collision.gameObject.GetComponent<PlayerManager>() == player)
        {
            //被自己碰到
            x = (int)PlayerManager.FromWorldToGrids(transform.position - 0.25f * (Vector3)direction).x;
            y = (int)PlayerManager.FromWorldToGrids(transform.position - 0.25f * (Vector3)direction).y;
            transform.position = GameManagerB.FromGridToWorld(x, y);
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }
        if (collision.gameObject.GetComponent<PlayerManager>() != null && collision.gameObject.GetComponent<PlayerManager>() != player && collision.gameObject.GetComponent<PlayerManager>().direction != new Vector2(0, 0) && isMoving == null)
        {
            //开始移动
            isMoving = collision.collider;
            ///下面是把所有原本的GoingToBomb去掉
            gameManager.gameGrid[x, y].hasBomb = false;
            gameManager.gameGrid[x, y].bombThere.Remove(this);
            gameManager.gameGrid[x, y].NotGoingToBomb(this);
            for (int i = 1; i <= scale; i++)
            {
                if (x > i && gameManager.gameGrid[x - i, y] != null)
                {
                    gameManager.gameGrid[x - i, y].NotGoingToBomb(this);
                }
                if (x > i && gameManager.gameGrid[x - i, y].tileType == TileType.wall)
                {
                    break;
                }
            }
            for (int i = 1; i <= scale; i++)
            {
                if (x < GameManagerB.width - i && gameManager.gameGrid[x + i, y] != null)
                {
                    gameManager.gameGrid[x + i, y].NotGoingToBomb(this);
                }
                if (x < GameManagerB.width - i && gameManager.gameGrid[x + i, y].tileType == TileType.wall)
                {
                    break;
                }
            }
            for (int i = 1; i <= scale; i++)
            {
                if (x < GameManagerB.width - i && y < GameManagerB.height - i && gameManager.gameGrid[x + i, y + i] != null)
                {
                    gameManager.gameGrid[x + i, y + i].NotGoingToBomb(this);
                }
                if (x < GameManagerB.width - i && y < GameManagerB.height - i && gameManager.gameGrid[x + i, y + i].tileType == TileType.wall)
                {
                    break;
                }
            }
            for (int i = 1; i <= scale; i++)
            {
                if (x > i && y > i && gameManager.gameGrid[x - i, y - i] != null)
                {
                    gameManager.gameGrid[x - i, y - i].NotGoingToBomb(this);
                }
                if (x > i && y > i && gameManager.gameGrid[x - i, y - i].tileType == TileType.wall)
                {
                    break;
                }
            }
            for (int i = 1; i <= scale; i++)
            {
                if (y > i && gameManager.gameGrid[x, y - i] != null)
                {
                    gameManager.gameGrid[x, y - i].NotGoingToBomb(this);
                }
                if (y > i && gameManager.gameGrid[x, y - i].tileType == TileType.wall)
                {
                    break;
                }
            }
            for (int i = 1; i <= scale; i++)
            {
                if (y < GameManagerB.height - i && gameManager.gameGrid[x, y + i] != null)
                {
                    gameManager.gameGrid[x, y + i].NotGoingToBomb(this);
                }
                if (y < GameManagerB.height - i && gameManager.gameGrid[x, y + i].tileType == TileType.wall)
                {
                    break;
                }
            }
            ///去掉完毕
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            Vector2 directionTemp = -(GameManagerB.FromGridToWorld(collision.gameObject.GetComponent<PlayerManager>().x, collision.gameObject.GetComponent<PlayerManager>().y) - GameManagerB.FromGridToWorld(x, y));
            if ((directionTemp.x) * (directionTemp.x) + (directionTemp.y) * (directionTemp.y) > 0.1f)
            {
                direction = directionTemp.normalized;
            }
            else
            {
                direction = player.direction;
            }
            return;
        }
    }
    void Update()
    {
        if(Time.time - initTime > 0.3 && ! hasUpdate)
        {
            UpdateState();
            hasUpdate = true;
        }
        if (Time.time - initTime > timeGap && !hasBoom)
        {
            Boom();
        }
        if (isMoving != null)
        {
            transform.position += speed * Time.deltaTime * (Vector3)direction;
            int xFront = (int)PlayerManager.FromWorldToGrids(transform.position + 0.7f * (Vector3)direction).x;
            int yFront = (int)PlayerManager.FromWorldToGrids(transform.position + 0.7f * (Vector3)direction).y;
            if (gameManager.hasPlayer(gameManager.gameGrid[xFront,yFront],isMoving.gameObject.GetComponent<PlayerManager>()) != null || gameManager.gameGrid[xFront, yFront].hasBomb || gameManager.gameGrid[xFront, yFront].tileType != TileType.ground || direction == new Vector2(0,0))
            {
                //停下
                isMoving = null;
                x = (int)PlayerManager.FromWorldToGrids(transform.position).x;
                y = (int)PlayerManager.FromWorldToGrids(transform.position).y;
                direction = new(0, 0);
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                transform.position = GameManagerB.FromGridToWorld(x, y);
                UpdateState();
            }
        }
    }

    public void Boom()
    {
        GameObject menu = GameObject.Find("MenuManager");
        if (menu != null)
        {
            gameManager.gameObject.GetComponent<AudioSource>().PlayOneShot(sound, menu.GetComponent<MainMenu>().volume / 100f);
        }
        else
        {
            gameManager.gameObject.GetComponent<AudioSource>().PlayOneShot(sound, 1f);
        }
        if (hasBoom)
            return;
        hasBoom = true;
        x = (int)Mathf.Round(PlayerManager.FromWorldToGrids(transform.position).x - 0.25f * direction.x);
        y = (int)Mathf.Round(PlayerManager.FromWorldToGrids(transform.position).y - 0.25f * direction.y);
        player.bombNum++;
        player.UpdatePanel();
        gameManager.gameGrid[x, y].bombThere.Remove(this);
        gameManager.gameGrid[x, y].ChangeBackFromBomb();
        gameManager.gameGrid[x, y].Boom(this);
        for (int i = 1; i <= scale; i++)
        {
            if (x > i && gameManager.gameGrid[x - i, y] != null)
            {
                gameManager.gameGrid[x - i, y].Boom(this);
            }
            if (x > i && gameManager.gameGrid[x - i, y].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (x < GameManagerB.width - i && gameManager.gameGrid[x + i, y] != null)
            {
                gameManager.gameGrid[x + i, y].Boom(this);
            }
            if (x < GameManagerB.width - i && gameManager.gameGrid[x + i, y].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (x < GameManagerB.width - i && y < GameManagerB.height - i && gameManager.gameGrid[x + i, y + i] != null)
            {
                gameManager.gameGrid[x + i, y + i].Boom(this);
            }
            if (x < GameManagerB.width - i && y < GameManagerB.height - i && gameManager.gameGrid[x + i, y + i].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (x > i && y > i && gameManager.gameGrid[x - i, y - i] != null)
            {
                gameManager.gameGrid[x - i, y - i].Boom(this);
            }
            if (x > i && y > i && gameManager.gameGrid[x - i, y - i].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (y > i && gameManager.gameGrid[x, y - i] != null)
            {
                gameManager.gameGrid[x, y - i].Boom(this);
            }
            if (y > i && gameManager.gameGrid[x, y - i].tileType == TileType.wall)
            {
                break;
            }
        }
        for (int i = 1; i <= scale; i++)
        {
            if (y < GameManagerB.height - i && gameManager.gameGrid[x, y + i] != null)
            {
                gameManager.gameGrid[x, y + i].Boom(this);
            }
            if (y < GameManagerB.height - i && gameManager.gameGrid[x, y + i].tileType == TileType.wall)
            {
                break;
            }
        }
        SendMessageOfBoomUpdate();
        Destroy(gameObject);
    }

    void SendMessageOfBoomUpdate()
    {
        foreach (PlayerManager player in gameManager.players)
        {
            if (player != null && player.isAI)
            {
                player.gameObject.GetComponent<AIController>().GetMessageOfBombUpdate();
            }
        }
    }
}
