using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    Transform[] trs;
    Text UIIndicatorText;
    private GameManagerStuff GameManager;
    private GameObject TowerUI, ClickerUI;
    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        TowerUI = GameObject.Find("UI/Tower");
        ClickerUI = GameObject.Find("UI/Clicker");
        UIIndicatorText = GameObject.Find("UIIndicator").transform.GetChild(0).GetComponent<Text>();
    }
    private void Start()
    {
        GameObject.Find("UI/Micro/Buy Gold").GetComponent<Button>().onClick.AddListener(AddGold); //addgold button func
        GameObject.Find("UI/Menu/ExitGame").GetComponent<Button>().onClick.AddListener(delegate { MenuFunc(GameObject.Find("UI/Menu/ExitGame")); }); //quit button func
        GameObject.Find("UI/Menu/RestartGame").GetComponent<Button>().onClick.AddListener(delegate { MenuFunc(GameObject.Find("UI/Menu/RestartGame")); }); //menu button func
        foreach (Transform button in TowerUI.transform)
        {
            if (button.name != "Text" && button.name != "Current Cost")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate { TowerFunc(button.gameObject); }); //listeners that require a parameter must be wrapped in a delegate to pass correctly
            }
        }
        foreach (Transform button in ClickerUI.transform)
        {
            if (button.name != "Text" && button.name != "Current Cost")
            {
                button.GetComponent<Button>().onClick.AddListener(delegate { ClickerFunc(button.gameObject); });
            }
        }
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
        if (TowerUI.activeSelf) //update tower menu values
        {
            foreach (Transform item in TowerUI.transform)
            {
                switch (item.name)
                {
                    case "Current Cost":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Current Upgrade Cost : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                        }
                        break;
                    case "Attack Speed":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Attack Speed : " + decimal.Round((decimal)TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().FireRate, 8, MidpointRounding.AwayFromZero);
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
                            item.GetChild(0).GetComponent<Text>().text = "Bonus Gold : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().BonusGold;
                        }
                        break;
                    case "Attack Range":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Attack Range : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Range;
                        }
                        break;
                    case "Armour Piercing":
                        if (TowerBehaviour.LastTowerSelected != null)
                        {
                            item.GetChild(0).GetComponent<Text>().text = "Armour Piercing : " + TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().armourpiercingpc + "%";
                        }
                        break;
                }
            }
        }
        if (ClickerUI.activeSelf) // update clicker menu values
        {
            foreach (Transform item in ClickerUI.transform)
            {
                switch (item.name)
                {
                    case "Current Cost":
                        item.GetChild(0).GetComponent<Text>().text = "Current Upgrade Cost : " + GameManagerStuff.Cost.ToString();
                        break;
                    case "Kill Bonus":
                        item.GetChild(0).GetComponent<Text>().text = "Kill Bonus : " + GameManagerStuff.Bonus.ToString();
                        break;
                    case "Armour Piercing":
                        item.GetChild(0).GetComponent<Text>().text = "Armour Piercing : " + GameManagerStuff.ArmourPiercingPC.ToString() + "%";
                        break;
                    case "Damage":
                        item.GetChild(0).GetComponent<Text>().text = "Damage : " + Mathf.Abs(GameManagerStuff.Damage).ToString();
                        break;
                }
            }
        }
    }
    private void ToggleUI() //when you click one button all others and their component parts/children are deactivates and the clicked button activates all components and children
    {
        try
        {
            UIIndicatorText.text = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name; //get current event object
            Transform[] trs2 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Transform>(true); //get active and inactive children of current event object
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
        GameManagerStuff.Currency += 10000;
    }
    private void MenuFunc(GameObject gO)
    {
        switch (gO.name)
        {
            case "ExitGame":
                Application.Quit();
                break;
            case "RestartGame":
                StartCoroutine(GameManager.RestartGame());
                break;
        }

    }
    private void ClickerFunc(GameObject gO) //clicker upgrade button functionality
    {
        switch (gO.name)
        {
            case "Damage":
                if (GameManager.CanAfford(GameManagerStuff.Cost))
                {
                    GameManagerStuff.Currency -= GameManagerStuff.Cost;
                    GameManagerStuff.Cost = Mathf.Round(GameManagerStuff.Cost * 1.1f) + 10;
                    GameManagerStuff.Damage -= 0.5f;
                }
                break;
            case "Kill Bonus":
                if (GameManager.CanAfford(GameManagerStuff.Cost))
                {
                    GameManagerStuff.Currency -= GameManagerStuff.Cost;
                    GameManagerStuff.Bonus += 3;
                    GameManagerStuff.Cost += 10;
                }
                break;
            case "Armour Piercing":
                if (GameManager.CanAfford(GameManagerStuff.Cost))
                {
                    if (GameManagerStuff.ArmourPiercingPC <= 90)
                    {
                        GameManagerStuff.Currency -= GameManagerStuff.Cost;
                        GameManagerStuff.ArmourPiercingPC += 10;
                        GameManagerStuff.Cost += 10;
                    }
                    else
                    {
                        GameManager.PushToEventLog("Armour piercing at 100%");
                    }
                }
                break;
        }
    }
    private void TowerFunc(GameObject gO) //tower upgrade menu button functionality
    {
        switch (gO.name)
        {
            case "Destroy Tower":
                if (TowerBehaviour.LastTowerSelected != null)
                {
                    Destroy(TowerBehaviour.LastTowerSelected);
                    GameManagerStuff.TowersBuilt--;
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
            case "Armour Piercing":
                if (TowerBehaviour.LastTowerSelected != null)
                {
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
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
            case "Kill Bonus":
                if (TowerBehaviour.LastTowerSelected != null)
                {
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
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
            case "Attack Range":
                if (TowerBehaviour.LastTowerSelected != null)
                {
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
        }
        else
        {
            GameManager.PushToEventLog("No Tower Selected");
        }
        break;
            case "Projectile Speed":
                if (TowerBehaviour.LastTowerSelected != null)
                {
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
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
            case "Accuracy":
                if (TowerBehaviour.LastTowerSelected != null)
                {
                    if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                {
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Accuracy += 0.01f;
                    GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                    TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                }
                else
                {
                    GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                }
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
            case "Damage":
                if (TowerBehaviour.LastTowerSelected != null)
                {
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
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
            case "Attack Speed":
                if (TowerBehaviour.LastTowerSelected != null)
                {
                    if (GameManager.CanAfford(TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost))
                    {
                        if (TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().FireRate > 0.01f)
                        {
                            TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().FireRate = (TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().FireRate / 10) * 9;
                            GameManagerStuff.Currency -= TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost;
                            TowerBehaviour.LastTowerSelected.GetComponent<TowerBehaviour>().Cost += 10;
                        }
                        else
                        {
                            GameManager.PushToEventLog("Attack Speed At Max");
                        }
                    }
                    else
                    {
                        GameManager.GetComponent<GameManagerStuff>().NotEnoughGold();
                    }
                }
                else
                {
                    GameManager.PushToEventLog("No Tower Selected");
                }
                break;
        }
    }
}