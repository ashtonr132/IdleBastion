using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private int alpha;
    private string str;
    internal static List<GameObject> buttons = new List<GameObject>();
    private void Start()
    {
        buttons.Add(gameObject);
        char[] chars = gameObject.name.ToCharArray(); // Name string to char array
        char[] charz = new char[chars.Length - 6];
        for (int i = 0; i < charz.Length; i++) // -Button suffix
        {
            charz[i] = chars[i];
        }
        str = new string(charz);
        GetComponent<Button>().onClick.AddListener(UIButton);
        if (GameObject.Find(str).GetComponent<CanvasRenderer>().GetAlpha() > 0)
        {
            alpha = 1;
        }
        else
        {
            alpha = 0;
        }
        UIButton();
    }
    private void UIButton()
    {
        if (alpha == 1)
        {
            alpha = 0;
        }
        else
        {
            alpha = 1;
        }
        foreach (Transform transform in GameObject.Find(str).transform)
        {
            transform.gameObject.GetComponent<CanvasRenderer>().SetAlpha(alpha);
            transform.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(alpha);
        }
        GameObject.Find(str).GetComponent<CanvasRenderer>().SetAlpha(alpha);
    }
}