using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAlly : MonoBehaviour, IBattleable, IAllyable
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

    protected int health = 40;
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
    protected int energy = 100;
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
    protected int tilt = 0;
    public int Tilt
    {
        get
        {
            return tilt;
        }
    }

    protected Animator anim;
    protected SpriteRenderer rend;

    readonly string[] timeButtons = { "Return","Select","Menu"};

    protected string timeButton = "Return";

    protected bool timed;
    protected float timeTimer;
    protected float waitTimer;

    protected Vector3 startPos;

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

    public void SetStats(int _index)
    {
        Index = _index;
        timeButton = timeButtons[_index];
    }

    public virtual void YourTurn()
    {
        BattleManager.current.StartSelection(Index);
    }

    public virtual void TakeDamage(int _damage)
    {
        Debug.Log("Aj");
    }

    public virtual void RegularAttack()
    {

    }

    
}
