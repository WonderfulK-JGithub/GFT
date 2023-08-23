using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAlly : MonoBehaviour, IBattleable, IAllyable
{
    [Header("Battle ally")]
    [SerializeField] GameObject circleIndicatorPrefab;

    public Bounds TargetBounds
    {
        get
        {
            Bounds _bounds = new(Vector3.up * 0.75f, new Vector3(1f, 1.5f));
            return _bounds;
        }
    }
    public IBattleable BattleData
    {
        get
        {
            return this;
        }
    }
    public int BattleIndex { get; set; }
    public int AllyIndex { get; set; }

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

    readonly string[] timeButtons = { "Select", "Return","Menu"};

    protected string timeButton = "Return";

    protected bool timed;
    protected float timeTimer;
    protected float waitTimer;

    protected Vector3 startPos;

    protected bool dead;

    GameObject itemObject;

    #region references
    protected AdamBattle adam;
    protected GustavBattle gustav;
    #endregion

    protected virtual void Awake()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        adam = FindAnyObjectByType<AdamBattle>();
        gustav = FindAnyObjectByType<GustavBattle>();
    }
    protected virtual void Update()
    {
        if (Input.GetButtonDown(timeButton))
        {
            if (waitTimer <= 0f && !timed)
            {
                timeTimer = 0.14f;
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
                waitTimer = 0.14f;
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
        AllyIndex = _allyIndex;
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
    protected int DamageCalculation(int _damage)
    {
        return _damage;
    }

    public virtual void Die()
    {
        anim.Play("Dead");
        dead = true;
        BattleManager.current.AllyDefeated(this);
    }
    public virtual void Revive()
    {
        if (!dead) return;
        dead = false;
        anim.Play("BattleIdle");
        BattleManager.current.AllyRevived(this);
    }

    public virtual void Heal(int _amount)
    {
        health += _amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _amount.ToString(), BattleNumberType.Heal);
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }
    public virtual void GainEnergy(int _amount)
    {
        energy += _amount;
        energy = Mathf.Clamp(energy, 0, maxEnergy);

        BattleManager.current.CreateBattleNumberText(transform.position + TargetBounds.center, _amount.ToString(), BattleNumberType.Energy);
        BattleManager.current.UpdateAllyStatPanel(BattleIndex);
    }
    public virtual void RegularAttack()
    {

    }

    protected void CreateCircleIndicator()
    {
        Instantiate(circleIndicatorPrefab, transform.position + TargetBounds.center, Quaternion.identity);
    }
    protected void CreateCircleIndicator(float _time)
    {
        Invoke(nameof(CreateCircleIndicator), _time);
    }
    protected bool AccuractCheck()
    {
        return accuracy > Random.Range(0, 100);
    }

    public virtual void UseAbility()
    {
        Debug.Log("Implement!!!");
    }

    //först item animation
    public virtual void UseItem()
    {
        Item _usedItem = BattleManager.current.selectedItem;
        itemObject = Instantiate(BattleManager.current.battleItemIconPrefab,transform.position + TargetBounds.center,Quaternion.identity);
        itemObject.GetComponent<SpriteRenderer>().sprite = _usedItem.ItemSprite;
        itemObject.LeanMoveY(itemObject.transform.position.y + 1.5f, 1f).setOnComplete(ItemUsage);
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
                if (_heal.Revive)
                {
                    IAllyable _ally = BattleManager.current.AlliesData[BattleManager.current.allyTarget];
                    _ally.Revive();
                    _ally.BattleData.Heal(_heal.HealthGain);
                }
                else
                {
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
