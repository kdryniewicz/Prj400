using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb2D;
    private Animator animator; //Used to store a reference to the Player's animator component.
    public Direction playerDir;
    public Vector2 prevPos;
    public bool playerIdle;

    // Start is called before the first frame update
    void Start()
    {
        if (rb2D == null)
        {
            rb2D = gameObject.GetComponent<Rigidbody2D>();
        }
        //Get a component reference to the Player's animator component
        animator = GetComponentInChildren<Animator>();

        prevPos = rb2D.position;
        playerDir = Direction.South;
        animator.SetTrigger("playerIdle");
        animator.SetTrigger("playerDirDown");
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
       
        rb2D.MovePosition(rb2D.position + movement * (speed * Time.fixedDeltaTime));

        if (!(rb2D.position == prevPos))
        {
            playerIdle = false;
        }
        else
        {
            playerIdle = true;
        }
        SetPlayerDir();
        UpdateAnimation();
        if (rb2D.position != prevPos)
        {
            prevPos = rb2D.position;
        }
    }

    private void SetPlayerDir()
    {
        if (rb2D.position.x > prevPos.x) // Check if player is going right
        {
            playerDir = Direction.East;
        }
        else if (rb2D.position.y > prevPos.y) //check if player is going up
        {
            playerDir = Direction.North;
        }
        else if (rb2D.position.x < prevPos.x) //Check if player is going left
        {
            playerDir = Direction.West;
        }
        else if (rb2D.position.y < prevPos.y) //Check if player is going down
        {
            playerDir = Direction.South;
        }
       
    }

    void UpdateAnimation()
    {
        switch (playerDir)
        {
            case Direction.North:
                animator.SetTrigger("playerDirUp");
                //Debug.Log("Player facing up!");
                break;
            case Direction.South:
                animator.SetTrigger("playerDirDown");
                //Debug.Log("Player facing down!");
                break;
            case Direction.East:
                animator.SetTrigger("playerDirRight");
                //Debug.Log("Player facing right!");
                break;
            case Direction.West:
                animator.SetTrigger("playerDirLeft");
                //Debug.Log("Player facing left!");
                break;
        }
        if (playerIdle)
        {
            animator.SetTrigger("playerIdle");
            //Debug.Log("Player is idle!");
        }
        else
        {
            animator.SetTrigger("playerWalk");
            //Debug.Log("Player is walking!");
        }
    }

}


