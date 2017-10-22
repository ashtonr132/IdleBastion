using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TownControls : MonoBehaviour {
    internal float Food, Currency, Stone, Crystal, Population = 1, Wood;
	
	// Update is called once per frame
	void Update ()
    {
        Food += Population * 0.0015f;
        GameObject.Find("Food").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Food).ToString();
        GameObject.Find("Currency").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Currency).ToString();
        GameObject.Find("Stone").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Stone).ToString();
        Stone += Population * 0.0005f;
        GameObject.Find("Crystal").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Crystal).ToString();
        GameObject.Find("Population").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Population).ToString();
        Population += Population * 0.0001f;
        GameObject.Find("Wood").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Wood).ToString();
        Wood += Population * 0.00075f;
    }
}
