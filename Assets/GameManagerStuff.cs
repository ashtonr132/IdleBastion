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
    }
}
