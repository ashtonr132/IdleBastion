using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Button>().onClick.AddListener(ToggleUI); //Add ref to onclick function
            foreach (Transform butt in GetRelativeUI(child.gameObject).transform) //Dont Render any open ui on start
            {
                butt.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
                butt.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(0);
            }
            GetRelativeUI(child.gameObject).GetComponent<CanvasRenderer>().SetAlpha(0);
        }
    }
    private void ToggleUI()
    {
        foreach (Transform child in gameObject.transform) //Each button in ui list
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name == child.name) //This button
            {
                if (GetRelativeUI(child.gameObject).GetComponent<CanvasRenderer>().GetAlpha() == 0)
                {
                    foreach (Transform butt in GetRelativeUI(child.gameObject).transform) //Render this ui
                    {
                        butt.gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);
                        butt.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(1);
                    }
                    GetRelativeUI(child.gameObject).GetComponent<CanvasRenderer>().SetAlpha(1);
                }
                else
                {
                    foreach (Transform butt in GetRelativeUI(child.gameObject).transform) //Unless its already being rendered in which case dont
                    {
                        butt.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
                        butt.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(0);
                    }
                    GetRelativeUI(child.gameObject).GetComponent<CanvasRenderer>().SetAlpha(0);
                }
            }
            else
            {
                foreach (Transform butt in GetRelativeUI(child.gameObject).transform) //Dont render the other ui'
                {
                    butt.gameObject.GetComponent<CanvasRenderer>().SetAlpha(0);
                    butt.GetChild(0).GetComponent<CanvasRenderer>().SetAlpha(0);
                }
                GetRelativeUI(child.gameObject).GetComponent<CanvasRenderer>().SetAlpha(0);
            }
        }
    }
    private GameObject GetRelativeUI(GameObject go)
    {
        char[] chars = go.name.ToCharArray(); //Name string to char array
        char[] charz = new char[chars.Length - 6];
        for (int i = 0; i < charz.Length; i++) //-Button suffix in the string
        {
            charz[i] = chars[i];
        }
        return GameObject.Find(new string(charz));
    }
}