    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public Sprite grass;
    public Sprite dirt;
    public Sprite flag;
    public Sprite grassWithMouseOn;
    public Sprite explosion;
    public Sprite[] numberSprites; 
    public static PictureManager instance;

    public static PictureManager get()
    {
        if (instance == null)
        {
            instance = (PictureManager)(GameObject.FindObjectOfType<PictureManager>());
        }
        return instance;
    }
}