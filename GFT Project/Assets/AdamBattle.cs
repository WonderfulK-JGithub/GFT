using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamBattle : MonoBehaviour,IBattleable
{
    public int Speed
    {
        get
        {
            return 10;
        }
    }

    int health = 100;
    public int Health
    {
        get
        {
            return health;
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;
        print("aj");
    }

    public void YourTurn()
    {
        BattleManager.current.StartSelection();
    }
}
