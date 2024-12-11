using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedObjectManager : ObjecManager
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerManager>().speed += 0.7f;
            Destroy(gameObject);
        }
    }
}
