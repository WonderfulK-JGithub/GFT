using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermanBattle : BattleAlly
{
    IBattleable targetEnemy;

    public override void TakeDamage(int _damage)
    {
        int _newDamage;
        bool _wasTimed = timed;
        if (timed)
        {
            _newDamage = Mathf.CeilToInt(_damage / 2f);
            tilt += Mathf.CeilToInt(_damage * 25f / MaxHealth);
        }
        else
        {
            _newDamage = _damage;
            tilt += Mathf.CeilToInt(_damage * 100f / MaxHealth);
        }
        health -= _newDamage;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0)
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.KO);
            Die();
        }
        else if (_wasTimed)
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Blocked);
        }

        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _newDamage.ToString(), BattleNumberType.AllyDamage);
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }

    void GoBack()
    {
        if (BattleManager.current.IsCurrentAlly(BattleIndex)) LeanTween.move(gameObject, startPos, 1f).setOnComplete(Returned);
        else LeanTween.move(gameObject, startPos, 1f).setOnComplete(ReturnedFromCoOp);
        anim.Play("Herman_WalkRight");
    }
    void Returned()
    {
        anim.Play("BattleIdle");
        BattleManager.current.TurnEnded();
        rend.sortingOrder = 0;
    }
    void ReturnedFromCoOp()
    {
        anim.Play("BattleIdle");
        rend.sortingOrder = 0;
    }

    public override void RegularAttack()
    {
        targetEnemy = BattleManager.current.Enemies[BattleManager.current.enemyTarget];
        rend.sortingOrder = 1;
        anim.Play("Herman_WalkRight");
        Vector3 _targetPos = targetEnemy.GetGameObject().transform.position + (targetEnemy.TargetBounds.size.x / 2f + 0.5f) * Vector3.left;
        LeanTween.move(gameObject, _targetPos, 1.5f).setOnComplete(StartSwing);
    }
    public override void YourTurn()
    {
        base.YourTurn();
        anim.Play("Herman_IdleDown");
    }
    void StartSwing()
    {
        anim.Play("Herman_HockeySwing");

    }

    void Hit()
    {
        int _damage = attackPower;

        if (timed)
        {
            _damage *= 2;
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Nice);
        }
        targetEnemy.TakeDamage(_damage);
        Invoke(nameof(GoBack), 0.5f);
    }

    public override void UseAbility()
    {
        base.UseAbility();

        Ability _abilityToUse = BattleManager.current.selectedAbility;
        switch (_abilityToUse.AbilityName)
        {
            default:
                Debug.Log(gameObject.name + " does not have that ability...");
                break;
            case AbilityName.CrossCheck:
                StartCrossCheck();
                break;
        }

        energy -= _abilityToUse.Energy;
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }

    #region Abilities

    void StartCrossCheck()
    {
        anim.Play("Herman_Spin");
        LeanTween.alpha(gameObject, 0f, 0.5f);
        Invoke(nameof(CCFadeBack), 1.5f);
        CreateCircleIndicator(2.25f - CircleIndicator.circleTime);
        targetEnemy = BattleManager.current.Enemies[BattleManager.current.enemyTarget];
    }

    void CCFadeBack()
    {
        LeanTween.alpha(gameObject, 1f, 0.5f).setOnComplete(CCAnimation);
        transform.position = targetEnemy.GetGameObject().transform.position + (targetEnemy.TargetBounds.size.x / 2f + 0.5f) * Vector3.right;
    }

    void CCAnimation()
    {
        
        anim.Play("Herman_CrossCheck");
    }

    void CrossCheckHit()
    {
        if (timed)
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Nice);

            int _damage = Mathf.CeilToInt(attackPower * 2.5f);
            targetEnemy.TakeDamage(_damage);
        }
        else
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Miss);
        }

        Invoke(nameof(GoBack), 1f);
    }

    #endregion
}
