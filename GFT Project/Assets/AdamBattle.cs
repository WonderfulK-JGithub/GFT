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

    public void TakeDamage(int _damage)
    {
        throw new System.NotImplementedException();
    }

    public void YourTurn()
    {
        print("Adam time");
    }
}
