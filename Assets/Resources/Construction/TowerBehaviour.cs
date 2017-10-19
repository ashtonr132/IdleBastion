using UnityEngine;
using System.Collections;
using System;

public class TowerBehaviour : MonoBehaviour {

    private int Damage = -1, Cost = 100, Range = 50, ProjectileSpeed = 5;
    private float FireRate = 0.25f, Accuracy = 1.25f;
    private GameObject EnemyController;
    private GameObject Target = null;
    private GameManagerStuff GameManager;

    //Use this for initialization
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        EnemyController = GameObject.Find("EnemyController");
        if (GameManager.CurrencyAmount - Cost < 0)
        {
            GameManager.PushToEventLog("This exceeds youre current gold.");
            Destroy(gameObject);
        }
        else
        {
            GameManager.CurrencyAmount -= Cost;
        }
        StartCoroutine(Fire());
    }
    //Update is called once per frame
    void Update()
    {
        foreach (Transform enemy in EnemyController.transform)
        {
            if (enemy != null)
            {
                if (Target != null)
                {
                    if (Vector3.Distance(enemy.position, transform.position) < Vector3.Distance(Target.transform.position, transform.position))
                    {
                        Target = enemy.gameObject;
                    }
                }
                else
                {
                    Target = enemy.gameObject;
                }
            }
        }
    }
    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(FireRate);
        if (Target != null && Vector3.Distance(Target.transform.position, transform.position) < Range)
        {
            GameObject Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Projectile.transform.name = "Projectile";
            Projectile.transform.localScale *= 3;
            Rigidbody rb = Projectile.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Projectile.GetComponent<SphereCollider>().isTrigger = true;
            Projectile.transform.position = gameObject.transform.position + new Vector3(0, gameObject.GetComponent<MeshCollider>().bounds.size.y);
            Projectile.GetComponent<Rigidbody>().velocity = (((UnityEngine.Random.onUnitSphere / Accuracy) + Target.transform.position) - Projectile.transform.position) * ProjectileSpeed;
            Projectile.transform.SetParent(gameObject.transform);
            Destroy(Projectile, ProjectileSpeed/2);
        }
        StartCoroutine(Fire());
    }
    public void SetTowerType(String Type)
    {
        switch (Type)
        {
            case "Blockade":
                break;
            case "IntermediateFireRate":
                break;
            case "IntermediateBalanced":
                break;
            case "IntermediateDamage":
                break;
            case "Marksman":
                break;
            case "AdvancedFireRate":
                break;
            case "AdvancedSpreadShotX3":
                break;
            case "AdvancedSpreadShotX5":
                break;
            case "AdvancedLargerProjectiles":
                break;
            case "Explosive":
                break;
            case "Gatling":
                break;
            case "Contagion":
                break;
            case "ElementalAffliction":
                break;
            case "BasicSupport":
                break;
            case "Agitation":
                break;
            case "VisualAcuity":
                break;
            case "Generator":
                break;
            case "Mine":
                break;
            case "Beam":
                break;
            case "Divarification":
                break;
            case "Swarm":
                break;
            case "Assimilation":
                break;
            case "Water":
                break;
            case "Fire":
                break;
            case "Earth":
                break;
            case "Air":
                break;
            case "Plasma":
                break;
            case "Ice":
                break;
            case "Magma":
                break;
            case "Terra":
                break;
            case "Lightning":
                break;
            case "BlackHole":
                break;
            case "Radiation":
                break;
            case "AreaEffect":
                break;
            default: //Basic
                Damage = -1; Cost = 100; Range = 50; ProjectileSpeed = 5; FireRate = 0.25f; 
                transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color32(100, 50, 0, 255);
                    break;
        }
    }
    public int GetDamage()
    {
        return Damage;
    }
}
