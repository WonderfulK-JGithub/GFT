using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,ICollideable
{
    public void OnCollide()
    {
        BattleManager.alliesToSpawn.Add(0);
        BattleManager.enemiesToSpawn.Add(0);

        SceneTransition.current.EnterBattleScene();
    }

    
}
