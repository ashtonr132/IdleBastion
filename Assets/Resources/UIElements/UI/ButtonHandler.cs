using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    Transform[] trs;
    Text UIIndicatorText;
    private void Start()
    {
        UIIndicatorText = GameObject.Find("UIIndicator").transform.GetChild(0).GetComponent<Text>();
        trs = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            trs[i] = transform.GetChild(i);
        }
        foreach (Transform t in trs)
        {
            if (t.GetComponent<Button>())
            {
                t.GetComponent<Button>().onClick.AddListener(ToggleUI); //Add ref to onclick function
                foreach (Transform d in t)
                {
                    if (!d.GetComponent<Text>())
                    {
                        d.gameObject.SetActive(false);
                    }
                }
            }
        }
        trs[2].GetComponent<Button>().onClick.Invoke();
    }
    private void ToggleUI()
    {
        try
        {
            UIIndicatorText.text = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
            Transform[] trs2 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform b in trs)
            {
                foreach (Transform c in b)
                {
                    if (!c.GetComponent<Text>())
                    {
                        c.gameObject.SetActive(false);
                    }
                }
            }
            foreach (Transform a in trs2)
            {
                a.gameObject.SetActive(true);
            }
        }
        catch
        {
            UIIndicatorText.text = trs[1].name;
            foreach (Transform tr in trs[1])
            {
                tr.gameObject.SetActive(true);
            }
        }
    }
}