using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamBattle : MonoBehaviour,IBattleable,IAllyable
{
    public Bounds TargetBounds
    {
        get
        {
            Bounds _bounds = new(Vector3.up * 1.5f, new Vector3(1f, 1.5f));
            return _bounds;
        }
    }

    public int Index { get; set; }

    public int Speed
    {
        get
        {
            return 10;
        }
    }

    int health = 40;
    public int Health
    {
        get
        {
            return health;
        }
    }
    public int MaxHealth
    {
        get
        {
            return 40;
        }
    }
    int energy = 100;
    public int Energy
    {
        get
        {
            return energy;
        }
    }
    public int MaxEnergy
    {
        get
        {
            return 100;
        }
    }
    int tilt = 0;
    public int Tilt
    {
        get
        {
            return tilt;
        }
    }

    IBattleable targetEnemy;
    Vector3 startPos;

    Animator anim;
    SpriteRenderer rend;

    string timeButton = "Return";

    bool timed;
    float timeTimer;
    float waitTimer;

    private void Awake()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(timeButton))
        {
            if(waitTimer <= 0f && !timed)
            {
                timeTimer = 0.15f;
                timed = true;
            }
            else
            {
                timed = false;
                waitTimer = 0.25f;
            }
        }
        if (timed)
        {
            timeTimer -= Time.deltaTime;
            if(timeTimer <= 0f)
            {
                timed = false;
                waitTimer = 0.15f;
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void TakeDamage(int _damage)
    {
        int _newDamage;
        if (timed)
        {
            _newDamage = Mathf.CeilToInt(_damage / 2f);
            tilt += Mathf.CeilToInt(_damage * 33f / MaxHealth);
            BattleManager.current.CreateBattleText(transform.position + Vector3.up, BattleTextType.Blocked);
        }
        else
        {
            _newDamage = _damage;
            tilt += Mathf.CeilToInt(_damage * 100f / MaxHealth );
        }
        health -= _newDamage;
        
        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _newDamage.ToString(), BattleNumberType.AllyDamage);
        BattleManager.current.UpdateAllyStatPanel(Index);
    }

    public void YourTurn()
    {
        BattleManager.current.StartSelection(Index);
    }

    void GoBack()
    {
        LeanTween.move(gameObject, startPos, 1f).setOnComplete(Returned);
        anim.Play("Adam_WalkRight");
    }
    void Returned()
    {
        anim.Play("Adam_BattleIdle");
        BattleManager.current.TurnEnded();
        rend.sortingOrder = 0;
    }

    public void RegularAttack()
    {
        targetEnemy = BattleManager.current.Enemies[BattleManager.current.enemyTarget];
        rend.sortingOrder = 1;
        anim.Play("Adam_WalkRight");
        Vector3 _targetPos = targetEnemy.GetGameObject().transform.position + (targetEnemy.TargetBounds.size.x / 2f + 0.5f) * Vector3.left;
        LeanTween.move(gameObject, _targetPos, 1.5f).setOnComplete(StartSwing);
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
