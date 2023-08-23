using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBattle : MonoBehaviour,IBattleable
{
    [SerializeField] GameObject minionBallPrefab;
    [SerializeField] Transform ballSpawnPoint;

    GameObject ball;
    IBattleable targetPlayer;

    public Bounds TargetBounds
    {
        get
        {
            Bounds _bounds = new(Vector3.up * 0.5f, new Vector3(1f, 1f));
            return _bounds;
        }
    }

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
        health -= _damage;
        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _damage.ToString(), BattleNumberType.EnemyDamage);
        if (health <= 0)
        {
            BattleManager.current.EnemyDefeated(this);
            Destroy(gameObject);
        }
    }

    public virtual void Heal(int _amount)
    {
        health += _amount;
        health = Mathf.Clamp(health, 0, 321312321);

        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _amount.ToString(), BattleNumberType.Heal);
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
        targetPlayer.TakeDamage(10);
        BattleManager.current.TurnEnded();
    }
}
