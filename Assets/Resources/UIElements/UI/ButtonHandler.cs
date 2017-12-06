using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    Transform[] trs;
    Text UIIndicatorText;
    private GameManagerStuff GameManager;
    private GameObject TowerUI;
    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        TowerUI = GameObject.Find("UI/Tower");
        GameObject.Find("UI/Micro/Buy Gold").GetComponent<Button>().onClick.AddListener(AddGold); //addgold button func
        GameObject.Find("UI/Menu/ExitGame").GetComponent<Button>().onClick.AddListener(QuitGame); //quit button func
        foreach (Transform button in TowerUI.transform)
        {
            if (button.name != "Text")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate { TowerFunc(button.gameObject); });
            }
        }
        UIIndicatorText = GameObject.Find("UIIndicator").transform.GetChild(0).GetComponent<Text>();
        trs = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) //main selections in the menu
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
                        d.gameObject.SetActive(false); //set all to inactive to start
                    }
                }
            }
        }
        trs[2].GetComponent<Button>().onClick.Invoke(); //set memu button active on start
    }
    private void Update()
    {
        if (TowerUI.activeSelf)
        {
            foreach (Transform item in TowerUI.transform)
            {
                switch (item.name)
                {
                    case "Attack Speed":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Attack Speed : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().FireRate;
                        }
                        break;
                    case "Damage":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Damage : " + Mathf.Abs(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Damage);
                        }
                        break;
                    case "Accuracy":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Accuracy: " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Accuracy;
                        }
                        break;
                    case "Projectile Speed":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Projectile Speed : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().ProjectileSpeed;
                        }
                        break;
                    case "Kill Bonus":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Attack Speed : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().BonusGold;
                        }
                        break;
                    case "Attack Range":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Attack Speed : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Range;
                        }
                        break;
                    case "Armour Piercing":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Armour Piercing: " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().armourpiercingpc + "%";
                        }
                        break;
                }
            }
        }
    }
    private void ToggleUI() //when you click one button all others and their component parts/children are deactivates and the clicked button activates all components and children
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
    private void AddGold()
    {
        GameManagerStuff.Currency += 1000;
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    private void TowerFunc(GameObject gO)
    {
        switch (gO.name)
        {
            case "Destroy Tower":
                Destroy(TowerBehaviour.LastTowerSelected);
                break;
            case "Armour Piercing":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    if (TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().armourpiercingpc <= 90)
                    {
                        TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().armourpiercingpc += 10;
                        GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                        TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                    }
                    else
                    {
                        GameManager.PushToEventLog("Armour piercing at 100%");
                    }
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
            case "Kill Bonus":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().BonusGold += 15;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
            case "Attack Range":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Range += 10;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
            case "Projectile Speed":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().ProjectileSpeed += 1;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
            case "Accuracy":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Damage += 0.5f;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
            case "Damage":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Damage -= 0.5f;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
            case "Attack Speed":
                if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().FireRate += 0.02f;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                break;
        }
    }
}