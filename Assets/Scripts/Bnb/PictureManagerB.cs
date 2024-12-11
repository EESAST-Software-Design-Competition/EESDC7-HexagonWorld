using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManagerB : MonoBehaviour
{
    private static PictureManagerB instance;
    public Sprite ground;
    public Sprite wall;
    public Sprite box;
    public Sprite[] player;
    public Sprite boom;
    //下方一个为测试  sprite
    public Sprite test;
    public Sprite players;
    public Sprite AIs;
    public GameObject bloodPrefab;
    public GameObject speedPrefab;
    public GameObject bombPrefab;
    public GameObject bombScalePrefab;

    public static PictureManagerB Get()
    {
        if(instance == null)
        {
            instance = GameObject.FindObjectOfType<PictureManagerB>();
        }
        return instance;
    }
}
