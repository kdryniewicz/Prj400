using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb2D;
    private Animator animator;                  //Used to store a reference to the Player's animator component.
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
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if(rb2D.position != prevPos)
        {
            prevPos = rb2D.position;
        }
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
                break;
            case Direction.South:
                animator.SetTrigger("playerDirDown");
                break;
            case Direction.East:
                animator.SetTrigger("playerDirRight");
                break;
            case Direction.West:
                animator.SetTrigger("playerDirLeft");
                break;
        }
        if (playerIdle)
        {
            animator.SetTrigger("playerIdle");
        }
        else
        {
            animator.SetTrigger("playerWalk");
        }
    }

}


