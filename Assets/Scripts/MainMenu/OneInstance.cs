using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneInstance : MonoBehaviour
{
    private static OneInstance _instance;
    public static OneInstance Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = this;
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);

    }
}
