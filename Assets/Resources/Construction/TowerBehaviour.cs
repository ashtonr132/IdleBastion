using UnityEngine;
using System.Collections;
using System;

public class TowerBehaviour : MonoBehaviour {

    internal int Damage = -1, Cost = 100, Range = 50, ProjectileSpeed = 5;
    internal float FireRate = 0.25f, Accuracy = 1.25f;
    private GameObject EnemyController;
    private GameObject Target = null;
    private GameManagerStuff GameManager;
    internal enum TowerID
    {
        Blockade, IntermediateFireRate, IntermediateBalanced,
        IntermediateDamage, Marksman, AdvancedFireRate,
        AdvancedSpreadShotX3, AdvancedSpreadShotX5, AdvancedLargerProjectiles,
        Explosive, Gatling, Contagion, ElementalAffliction, BasicSupport,
        Agitation, VisualAcuity, Generator, Mine, Beam, Divarification,
        Swarm, Assimilation, Water, Fire, Earth, Air, Plasma, Ice, Magma,
        Terra, Lightning, BlackHole, Radiation, AreaEffect, Default
    }
    internal TowerID CurrentTowerID;
    void Start() //Use this for initialization
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManagerStuff>();
        EnemyController = GameObject.Find("EnemyController");
        if (GameManagerStuff.Currency - Cost < 0)
        {
            GameManager.PushToEventLog("This exceeds youre current gold.");
            Destroy(gameObject);
        }
        else
        {
            GameManagerStuff.Currency -= Cost;
        }
        StartCoroutine(Fire());
    }
    void Update()//Update is called once per frame
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
    internal void SetTowerType(TowerID ID)
    {
        switch (ID)
        {
            case TowerID.Blockade:
                break;
            case TowerID.IntermediateFireRate:
                break;
            case TowerID.IntermediateBalanced:
                break;
            case TowerID.IntermediateDamage:
                break;
            case TowerID.Marksman:
                break;
            case TowerID.AdvancedFireRate:
                break;
            case TowerID.AdvancedSpreadShotX3:
                break;
            case TowerID.AdvancedSpreadShotX5:
                break;
            case TowerID.AdvancedLargerProjectiles:
                break;
            case TowerID.Explosive:
                break;
            case TowerID.Gatling:
                break;
            case TowerID.Contagion:
                break;
            case TowerID.ElementalAffliction:
                break;
            case TowerID.BasicSupport:
                break;
            case TowerID.Agitation:
                break;
            case TowerID.VisualAcuity:
                break;
            case TowerID.Generator:
                break;
            case TowerID.Mine:
                break;
            case TowerID.Beam:
                break;
            case TowerID.Divarification:
                break;
            case TowerID.Swarm:
                break;
            case TowerID.Assimilation:
                break;
            case TowerID.Water:
                break;
            case TowerID.Fire:
                break;
            case TowerID.Earth:
                break;
            case TowerID.Air:
                break;
            case TowerID.Plasma:
                break;
            case TowerID.Ice:
                break;
            case TowerID.Magma:
                break;
            case TowerID.Terra:
                break;
            case TowerID.Lightning:
                break;
            case TowerID.BlackHole:
                break;
            case TowerID.Radiation:
                break;
            case TowerID.AreaEffect:
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
