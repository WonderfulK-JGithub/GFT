using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAlly : MonoBehaviour, IBattleable, IAllyable
{
    public Bounds TargetBounds
    {
        get
        {
            Bounds _bounds = new(Vector3.up * 0.75f, new Vector3(1f, 1.5f));
            return _bounds;
        }
    }


    public int BattleIndex { get; set; }

    protected int speed;
    public int Speed
    {
        get
        {
            return speed;
        }
    }

    protected int health = 40;
    public int Health
    {
        get
        {
            return health;
        }
    }
    protected int maxHealth;
    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }
    protected int energy = 100;
    public int Energy
    {
        get
        {
            return energy;
        }
    }
    protected int maxEnergy;
    public int MaxEnergy
    {
        get
        {
            return maxEnergy;
        }
    }
    protected int tilt = 0;
    public int Tilt
    {
        get
        {
            return tilt;
        }
    }

    protected int attackPower;
    protected int defense;
    protected int accuracy;

    protected Animator anim;
    protected SpriteRenderer rend;

    readonly string[] timeButtons = { "Return","Select","Menu"};

    protected string timeButton = "Return";

    protected bool timed;
    protected float timeTimer;
    protected float waitTimer;

    protected Vector3 startPos;

    GameObject itemObject;

    protected virtual void Awake()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (Input.GetButtonDown(timeButton))
        {
            if (waitTimer <= 0f && !timed)
            {
                timeTimer = 0.12f;
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
            if (timeTimer <= 0f)
            {
                timed = false;
                waitTimer = 0.12f;
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

    public void SetStats(int _battleIndex,int _allyIndex)
    {
        BattleIndex = _battleIndex;
        timeButton = timeButtons[_battleIndex];

        AllyStats _stats = AllyStatsManager.current.alliesStats[_allyIndex];
        health = _stats.currentHealth;
        maxHealth = _stats.maxHealth;
        energy = _stats.currentEnergy;
        maxEnergy = _stats.maxEnergy;
        attackPower = _stats.attackPower;
        defense = _stats.defense;
        accuracy = _stats.accuracy;
        speed = _stats.speed;
    }

    public virtual void YourTurn()
    {
        BattleManager.current.StartSelection(BattleIndex);
    }

    public virtual void TakeDamage(int _damage)
    {
        Debug.Log("Aj");
    }

    public virtual void Heal(int _amount)
    {
        health += _amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _amount.ToString(), BattleNumberType.Heal);
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }

    public virtual void RegularAttack()
    {

    }

    protected bool AccuractCheck()
    {
        return accuracy > Random.Range(0, 100);
    }

    //först item animation
    public virtual void UseItem()
    {
        Item _usedItem = BattleManager.current.selectedItem;
        itemObject = Instantiate(BattleManager.current.battleItemIconPrefab,transform.position + TargetBounds.center,Quaternion.identity);
        itemObject.GetComponent<SpriteRenderer>().sprite = _usedItem.ItemSprite;
        itemObject.LeanMoveY(itemObject.transform.position.y + 1f, 0.5f).setOnComplete(ItemUsage);
        anim.Play("Item");
    }
    //när item animationen är klar
    void ItemUsage()
    {
        anim.Play("BattleIdle");
        Destroy(itemObject);

        Item _usedItem = BattleManager.current.selectedItem;
        switch (_usedItem.TypeOfItem)
        {
            case ItemType.healing:
                HealingItem _heal = _usedItem as HealingItem;
                if (_heal.TargetAll)
                {
                    foreach (var _ally in BattleManager.current.Allies)
                    {
                        _ally.Heal(_heal.HealthGain);
                    }
                }
                else
                {
                    BattleManager.current.Allies[BattleManager.current.allyTarget].Heal(_heal.HealthGain);
                }
                Invoke(nameof(EndTurn), 1f);
                break;
        }

        AllyStatsManager.current.RemoveItem(_usedItem);
    }

    protected virtual void EndTurn()
    {
        anim.Play("BattleIdle");
        BattleManager.current.TurnEnded();
        rend.sortingOrder = 0;
    }
}
