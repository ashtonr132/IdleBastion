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
            DisplayUi(GetRelativeUI(child.gameObject), 0);
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
                    DisplayUi(GetRelativeUI(child.gameObject), 1);
                }
                else
                {
                    DisplayUi(GetRelativeUI(child.gameObject), 0);
                    GetRelativeUI(child.gameObject).GetComponent<CanvasRenderer>().SetAlpha(0);
                }
            }
            else
            {
                DisplayUi(GetRelativeUI(child.gameObject), 0);
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
    private void DisplayUi(GameObject obj, int alpha)
    {
        obj.GetComponent<CanvasRenderer>().SetAlpha(alpha);
        if(obj.transform.childCount > 0)
        {
            foreach(Transform trans in obj.transform)
            {
                DisplayUi(trans.gameObject, alpha);
            }
        }
    }
    internal bool UIIsActive()
    { 
        foreach(CanvasRenderer canvasrenderer in gameObject.GetComponentsInChildren<CanvasRenderer>())
        {
            if (canvasrenderer.GetAlpha() > 0)
            {
                return true;
            }
        }
        return false;
    }
}