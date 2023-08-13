using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBattle : MonoBehaviour,IBattleable
{
    [SerializeField] GameObject minionBallPrefab;
    [SerializeField] Transform ballSpawnPoint;

    GameObject ball;
    IBattleable targetPlayer;

    public int Speed
    {
        get
        {
            return 10;
        }
    }
    int health = 10;
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
        throw new System.NotImplementedException();
    }

    public void YourTurn()
    {
        Shoot();
    }

    void Shoot()
    {
        ball = Instantiate(minionBallPrefab, ballSpawnPoint.position, Quaternion.identity);
        targetPlayer = BattleManager.current.Allies[Random.Range(0, BattleManager.current.Allies.Count)];
        LeanTween.move(ball,targetPlayer.GetGameObject().transform.position + Vector3.up * 0.5f,2f).setOnComplete(Hit);
    }

    void Hit()
    {
        Destroy(ball);
        targetPlayer.TakeDamage(1);
        BattleManager.current.TurnEnded();
    }
}
