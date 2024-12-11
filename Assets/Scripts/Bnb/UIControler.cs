using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControler : MonoBehaviour
{
    public GameObject panelPrefab;
    public List<GameObject> panels;
    Vector2 freePosition = new Vector2(100,-50);

    public GameObject GeneratePanel()
    { 
        GameObject panel = Instantiate(panelPrefab);
        panel.transform.SetParent(transform, false);
        panel.GetComponent<RectTransform>().anchoredPosition = freePosition;
        freePosition += new Vector2(0,-70);
        panels.Add(panel);
        return panel;
    }
}
