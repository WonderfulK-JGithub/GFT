using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustavBattle : BattleAlly
{
    [Header("Gustav")]
    [SerializeField] GameObject thinkBubblePrefab;
    [SerializeField] GameObject lightBulbPrefab;
    [SerializeField] GameObject questionMarkPrefab;
    [SerializeField] GameObject chefHatPrefab;
    [SerializeField] GameObject[] chefExplotionParticles;
    [SerializeField] Item kladdkaka;

    IBattleable targetEnemy;

    GameObject thinkBubble;
    GameObject chefHat;

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
        anim.Play("Gustav_WalkRight");
    }
    void Returned()
    {
        anim.Play("BattleIdle");
        if(alliesOnTurn.Count == 0) BattleManager.current.TurnEnded();
        else
        {
            BattleManager.current.TurnsEnded(alliesOnTurn);
            alliesOnTurn.Clear();
        }
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
        anim.Play("Gustav_WalkRight");
        Vector3 _targetPos = targetEnemy.GetGameObject().transform.position + (targetEnemy.TargetBounds.size.x / 2f + 0.3f) * Vector3.left;
        LeanTween.move(gameObject, _targetPos, 1.5f).setOnComplete(StartSwing);
    }
    public override void YourTurn()
    {
        base.YourTurn();
        anim.Play("Gustav_IdleDown");
    }

    void StartSwing()
    {
        anim.Play("Gustav_MarimbaSwing");
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
            case AbilityName.Filosofi:
                StartFilosofi();
                adam.StartFilosofi();
                break;
            case AbilityName.GustavKladdkaka:
                StartGustavKladdkaka();
                break;
        }

        energy -= _abilityToUse.Energy;
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }

    #region Abilities
    void StartGustavKladdkaka()
    {
        rend.sortingOrder = 1;
        anim.Play("Gustav_WalkRight");
        LeanTween.move(gameObject, Vector3.zero, 1.5f).setOnComplete(ChefHatDecend);
        targetEnemy = BattleManager.current.Enemies[BattleManager.current.enemyTarget];
    }

    void ChefHatDecend()
    {
        anim.Play("Item2");
        chefHat = Instantiate(chefHatPrefab, Vector3.up * 7f, Quaternion.identity);
        LeanTween.move(chefHat, new Vector3(0f, 1.75f, 0f), 3f).setOnComplete(GustavChef);
        CreateCircleIndicator(3f - CircleIndicator.circleTime);
    }

    void GustavChef()
    {
        if (timed)
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Nice);
            Invoke(nameof(ChefDamage), 1f);
        }
        else
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Miss);
            Invoke(nameof(GoBack), 1.5f);
            Destroy(chefHat, 1.5f);
        }
    }

    void ChefDamage()
    {
        int _damage = attackPower * 2;
        targetEnemy.TakeDamage(_damage);
        if (BattleManager.current.Enemies.Contains(targetEnemy))//den dog inte
        {
            Destroy(Instantiate(chefExplotionParticles[0], targetEnemy.GetGameObject().transform.position + targetEnemy.TargetBounds.center, Quaternion.identity),1f);
            Invoke(nameof(GoBack), 1.5f);
            Destroy(chefHat, 1.5f);
        }
        else //den dog
        {
            Destroy(Instantiate(chefExplotionParticles[1], targetEnemy.GetGameObject().transform.position + targetEnemy.TargetBounds.center, Quaternion.identity),2.2f);
            AllyStatsManager.current.AddItem(kladdkaka);
            Invoke(nameof(GoBack), 2.5f);
            Destroy(chefHat, 2.5f);
        }
    }

    #region Filosofi
    public void StartFilosofi()
    {
        rend.sortingOrder = 1;
        anim.Play("Gustav_WalkRight");
        LeanTween.move(gameObject, new Vector3(-0.5f,-1f), 1.5f).setOnComplete(StartThinking);
    }

    void StartThinking()
    {
        anim.Play("Gustav_IdleLeft");
        thinkBubble = Instantiate(thinkBubblePrefab, transform.position + new Vector3(2f, 3f, 0f), Quaternion.identity);
        Invoke(nameof(Realisation), 4f);
        Invoke(nameof(CreateCircleIndicator), 4f - CircleIndicator.circleTime);
    }

    void Realisation()
    {
        if (timed)
        {
            GameObject _light = Instantiate(lightBulbPrefab, thinkBubble.transform);
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Nice);
            _light.transform.localPosition = Vector3.zero;
            Invoke(nameof(FilosofiEnergyGain), 1f);
        }
        else
        {
            GameObject _questionMark = Instantiate(questionMarkPrefab, thinkBubble.transform);
            _questionMark.transform.localPosition = Vector3.zero;
            adam.Invoke(nameof(adam.FilosofiFrustration), 1f);
            Invoke(nameof(GoBack), 2f);
            Destroy(thinkBubble, 2f);
        }
        
    }

    public void FilosofiEnergyGain()
    {
        adam.FilosofiEnergyGain();
        GainEnergy(10);
        Invoke(nameof(GoBack), 0.5f);
        Destroy(thinkBubble);
    }
    #endregion

    #endregion
}
