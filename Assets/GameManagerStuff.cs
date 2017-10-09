using UnityEngine;
using UnityEngine.UI;

public class GameManagerStuff : MonoBehaviour
{
    public int CurrencyAmount = 0, TotalLifeTimeClicks = 0, EnemiesKilled = 0;
    public int Population = 1, DamageDealt = -1;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TotalLifeTimeClicks++;
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit; //ray stuff for damage text popups
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if ((Physics.Raycast(ray, out hit)))
            {
                doInfo(hit);
            }
        }
    }
    private void doInfo(RaycastHit hit)
    {
        print(GameObject.Find("GameText").GetComponent<Text>().text);
        // add info info
    }
}
