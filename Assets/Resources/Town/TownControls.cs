using UnityEngine;
using UnityEngine.UI;

public class TownControls : MonoBehaviour {
    internal float Food, Currency, Stone, Crystal, Population = 1, Wood;
    internal float BasePopulationIncrease = 0.0001f, BaseFoodPerPop = 0.0005f, BaseStonePerPop = 0.0005f, BaseWoodPerPop = 0.0005f; 

    // Update is called once per frame
    void Update ()
    {
        Food += Population * BaseFoodPerPop;
        GameObject.Find("PlayerUI/Food").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Food).ToString();
        GameObject.Find("PlayerUI/Currency").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Currency).ToString();
        GameObject.Find("PlayerUI/Stone").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Stone).ToString();
        Stone += Population * BaseStonePerPop;
        GameObject.Find("PlayerUI/Crystal").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Crystal).ToString();
        GameObject.Find("PlayerUI/Population").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Population).ToString();
        Population += Population * BasePopulationIncrease;
        GameObject.Find("PlayerUI/Wood").transform.GetChild(0).GetComponent<Text>().text = Mathf.Round(Wood).ToString();
        Wood += Population * BaseFoodPerPop;
    }
}
