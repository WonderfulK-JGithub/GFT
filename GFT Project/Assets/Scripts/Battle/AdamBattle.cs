using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamBattle : BattleAlly
{
    [Header("Adam")]
    [SerializeField] GameObject hockeyClubPrefab;
    [SerializeField] GameObject hockeyPuckPrefab;
    [SerializeField] GameObject thinkBubblePrefab;
    [SerializeField] GameObject thinkingEmojiPrefab;
    [SerializeField] GameObject angryEmojiPrefab;

    IBattleable targetEnemy;

    GameObject hockeyClub;
    GameObject hockeyPuck;
    GameObject thinkBubble;

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
        anim.Play("Adam_WalkRight");
    }
    void Returned()
    {
        anim.Play("BattleIdle");
        if (alliesOnTurn.Count == 0) BattleManager.current.TurnEnded();
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
            case AbilityName.HockeyTricks:
                StartHockeyTricks();
                break;
            case AbilityName.Filosofi:
                StartFilosofi();
                gustav.StartFilosofi();
                break;
        }

        energy -= _abilityToUse.Energy;
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }

    #region Abilities

    #region HockeyTricks
    void StartHockeyTricks()
    {
        targetEnemy = BattleManager.current.Enemies[BattleManager.current.enemyTarget];
        rend.sortingOrder = 1;
        anim.Play("Adam_WalkRight");
        LeanTween.move(gameObject, new Vector3(transform.position.x + 3f, -2f, 0f), 1f).setOnComplete(StartHockeyClubThrow);
    }
    void StartHockeyClubThrow()
    {
        anim.Play("ThrowUp");
        hockeyClub = Instantiate(hockeyClubPrefab, transform.position + Vector3.up, Quaternion.identity);
        LeanTween.move(hockeyClub, hockeyClub.transform.position + Vector3.up * 15f,1.5f).setOnComplete(HockeyClubFallDown);
    }
    void HockeyClubFallDown()
    {
        hockeyPuck = Instantiate(hockeyPuckPrefab, transform.position + Vector3.right, Quaternion.identity);
        hockeyPuck.LeanAlpha(1f, 1f);

        LeanTween.move(hockeyClub, hockeyClub.transform.position + Vector3.down * 15f, 1.5f).setOnComplete(CatchHockeyClub);
        Invoke(nameof(CreateCircleIndicator), 1.5f - CircleIndicator.circleTime);
    }
    void CatchHockeyClub()
    {
        if (timed)
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Nice);
            Destroy(hockeyClub);
            anim.Play("Adam_HockeySwing2");
        }
        else
        {
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Miss);
            LeanTween.move(hockeyClub, hockeyClub.transform.position + Vector3.down * 10f, 2f);
            Destroy(hockeyPuck);
            Destroy(hockeyClub, 2.5f);
            Invoke(nameof(GoBack), 2f);
        }
    }
    void HitHockeyPuck()
    {
        LeanTween.move(hockeyPuck, targetEnemy.GetGameObject().transform.position + targetEnemy.TargetBounds.center,0.8f).setOnComplete(HockeyTricksDamage);
    }
    void HockeyTricksDamage()
    {
        int _damage = attackPower * 3;
        targetEnemy.TakeDamage(_damage);
        Invoke(nameof(GoBack), 0.5f);
        Destroy(hockeyPuck);
    }
    #endregion

    #region filosofi
    public void StartFilosofi()
    {
        rend.sortingOrder = 1;
        anim.Play("Adam_WalkRight");
        LeanTween.move(gameObject, new Vector3(-2.5f, -1f), 1.5f).setOnComplete(StartThinking);
    }

    void StartThinking()
    {
        anim.Play("BattleIdle");
        thinkBubble = Instantiate(thinkBubblePrefab, transform.position + new Vector3(-2f, 3f, 0f), Quaternion.identity);
        Invoke(nameof(ThinkEmoji), 2f);
    }

    void ThinkEmoji()
    {
        GameObject _emoji = Instantiate(thinkingEmojiPrefab, thinkBubble.transform);
        _emoji.transform.localPosition = Vector3.zero;
    }

    public void FilosofiEnergyGain()
    {
        GainEnergy(10);
        Invoke(nameof(GoBack), 0.5f);
        Destroy(thinkBubble);
    }

    public void FilosofiFrustration()
    {
        Destroy(thinkBubble.transform.GetChild(1).gameObject);
        GameObject _emoji = Instantiate(angryEmojiPrefab, thinkBubble.transform);
        _emoji.transform.localPosition = Vector3.zero;
        Invoke(nameof(GoBack), 1f);
        Destroy(thinkBubble,1f);
    }
    #endregion

    #endregion
}
