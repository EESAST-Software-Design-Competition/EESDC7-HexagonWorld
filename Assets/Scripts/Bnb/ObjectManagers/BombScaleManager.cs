using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScaleManager : ObjecManager
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerManager>().bombScale += 1;
            collision.gameObject.GetComponent<PlayerManager>().UpdatePanel();
            Destroy(gameObject);
        }
    }
}
