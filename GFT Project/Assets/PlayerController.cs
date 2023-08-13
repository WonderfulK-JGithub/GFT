using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed;

    Rigidbody2D rb;
    Animator anim;

    Vector2 walkDir;
    Vector2 lookDir;

    bool moving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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

        //animation
        if (moving)
        {
            if(lookDir == Vector2.down)
            {
                anim.Play("WalkDown");
            }
            else if(lookDir == Vector2.right)
            {
                anim.Play("WalkRight");
            }
            else if(lookDir == Vector2.up)
            {
                anim.Play("WalkUp");
            }
            else //(lookDir == Vector2.left)
            {
                anim.Play("WalkLeft");
            }
        }
        else
        {
            if (lookDir == Vector2.down)
            {
                anim.Play("IdleDown");
            }
            else if (lookDir == Vector2.right)
            {
                anim.Play("IdleRight");
            }
            else if (lookDir == Vector2.up)
            {
                anim.Play("IdleUp");
            }
            else //(lookDir == Vector2.left)
            {
                anim.Play("IdleLeft");
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = walkSpeed * walkDir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(typeof(ICollideable),out Component _comp)) //kollar om det den collidar med har ett script med interfacet ICollideable
        {
            (_comp as ICollideable).OnCollide();
        }
    }
}

public interface ICollideable
{
    void OnCollide();
}
