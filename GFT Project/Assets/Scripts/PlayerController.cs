using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Vector3 spawnPos;

    public static PlayerController current;

    [SerializeField] float walkSpeed;

    [Header("Ally followers")]
    [SerializeField] Animator[] followers;
    [SerializeField] int stepDifference;

    Rigidbody2D rb;
    Animator anim;

    List<Vector3> points = new();
    List<Vector2> directions = new();

    Vector2 walkDir;
    Vector2 lookDir;

    bool moving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        current = this;
    }

    private void Start()
    {
        transform.position = spawnPos;
        for (int i = 0; i < stepDifference * followers.Length; i++)
        {
            points.Add(transform.position);
            directions.Add(Vector2.down);
        }
    }

    private void Update()
    {
        //movement
        walkDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(walkDir == Vector2.zero) //om man inte rör sig så ändras inte lookDir
        {
            moving = false;
        }
        else
        {
            moving = true;
            if(Mathf.Abs(walkDir.x) != Mathf.Abs(walkDir.y)) lookDir = walkDir;//lookDir ändras också inte om man går diagonalt
        }

        anim.Play(GetAnimation(moving, lookDir));
    }

    private void FixedUpdate()
    {
        rb.velocity = walkSpeed * walkDir.normalized;
        if(Vector3.Distance(transform.position,points[points.Count - 1]) > walkSpeed * Time.fixedDeltaTime)
        {
            points.RemoveAt(0);
            directions.RemoveAt(0);

            points.Add(transform.position);
            directions.Add(lookDir);
        }

        for (int i = 0; i < followers.Length; i++)
        {
            Animator _follower = followers[i];
            Vector3 _lastPos = _follower.transform.position;
            _follower.transform.position = Vector3.MoveTowards(_lastPos, points[i * stepDifference], walkSpeed * Time.fixedDeltaTime);
            bool _moving = _follower.transform.position != _lastPos;
            _follower.Play(GetAnimation(_moving, directions[i]));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(typeof(ICollideable),out Component _comp)) //kollar om det den collidar med har ett script med interfacet ICollideable
        {
            (_comp as ICollideable).OnCollide();
        }
    }

    string GetAnimation(bool _moving,Vector2 _direction)
    {
        //animation
        if (_moving)
        {
            if (_direction == Vector2.down)
            {
                return "WalkDown";
            }
            else if (_direction == Vector2.right)
            {
                return "WalkRight";
            }
            else if (_direction == Vector2.up)
            {
                return "WalkUp";
            }
            else //(_direction == Vector2.left)
            {
                return"WalkLeft";
            }
        }
        else
        {
            if (_direction == Vector2.down)
            {
                return "IdleDown";
            }
            else if (_direction == Vector2.right)
            {
                return "IdleRight";
            }
            else if (_direction == Vector2.up)
            {
                return"IdleUp";
            }
            else //(_direction == Vector2.left)
            {
                return"IdleLeft";
            }
        }
    }

    public void SaveSpawnPos()
    {
        spawnPos = transform.position;
    }
}

public interface ICollideable
{
    void OnCollide();
}
