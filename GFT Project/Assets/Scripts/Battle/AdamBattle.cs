using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamBattle : BattleAlly
{
    IBattleable targetEnemy;

    public override void TakeDamage(int _damage)
    {
        int _newDamage;
        if (timed)
        {
            _newDamage = Mathf.CeilToInt(_damage / 2f);
            tilt += Mathf.CeilToInt(_damage * 25f / MaxHealth);
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Blocked);
        }
        else
        {
            _newDamage = _damage;
            tilt += Mathf.CeilToInt(_damage * 100f / MaxHealth );
        }
        health -= _newDamage;
        
        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _newDamage.ToString(), BattleNumberType.AllyDamage);
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }

    void GoBack()
    {
        LeanTween.move(gameObject, startPos, 1f).setOnComplete(Returned);
        anim.Play("Adam_WalkRight");
    }
    void Returned()
    {
        anim.Play("BattleIdle");
        BattleManager.current.TurnEnded();
        rend.sortingOrder = 0;
    }

    public override void RegularAttack()
    {
        targetEnemy = BattleManager.current.Enemies[BattleManager.current.enemyTarget];
        rend.sortingOrder = 1;
        anim.Play("Adam_WalkRight");
        Vector3 _targetPos = targetEnemy.GetGameObject().transform.position + (targetEnemy.TargetBounds.size.x / 2f + 0.5f) * Vector3.left;
        LeanTween.move(gameObject, _targetPos, 1.5f).setOnComplete(StartSwing);
    }
    public override void YourTurn()
    {
        base.YourTurn();
        anim.Play("Adam_IdleDown");
    }
    void StartSwing()
    {
        anim.Play("Adam_HockeySwing");
        
    }

    void Hit()
    {
        int _damage = 3;

        if (timed)
        {
            _damage *= 2;
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Nice);
        }
        targetEnemy.TakeDamage(_damage);
        Invoke(nameof(GoBack), 0.5f);
    }
}
