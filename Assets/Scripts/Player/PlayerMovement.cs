using ProceduralGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb2D;
    private Animator animator; //Used to store a reference to the Player's animator component.
    public PlayerDir playerDir;
    public Vector2 prevPos;
    public bool playerIdle;
    public bool playerAttacking = false;
    public bool playerLock = false;
    float prevSpeed;

    //Fireball shooting
    public Transform fbSpawn;
    public GameObject Fireball;
    public float DelayBetweenShots = 1f;

    // Start is called before the first frame update
    void Start()
    {
        playerLock = false;
        if (rb2D == null)
        {
            rb2D = gameObject.GetComponent<Rigidbody2D>();
        }
        //Get a component reference to the Player's animator component
        animator = GetComponentInChildren<Animator>();

        prevPos = rb2D.position;
        playerDir = PlayerDir.South;
        animator.SetTrigger("playerIdle");
        animator.SetTrigger("playerDirDown");
        prevSpeed = GetComponent<Player>().PlayerStats.Speed;

    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void OnLevelWasLoaded(int level)
    {
        
    }

    private void FixedUpdate()
    {
        if (!playerLock)
        {
            rb2D.MovePosition(rb2D.position + movement * (GetComponent<Player>().PlayerStats.Speed * Time.fixedDeltaTime));

  
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

            if (Input.GetButtonUp("Fire1"))
            {
                animator.SetTrigger("playerAttack");
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                StartCoroutine(CreateFireball());
            }

            if (Input.GetButtonDown("Jump"))
            {
                GetComponent<Player>().PlayerStats.Speed = GetComponent<Player>().PlayerStats.Speed * 2;
            }
            else
            {
                GetComponent<Player>().PlayerStats.Speed = prevSpeed;
            }
        }

    }


    IEnumerator CreateFireball()
    {
        if (GetComponent<Player>().PlayerStats.Mana > 0)
        {
            GameObject go = Instantiate(Fireball, fbSpawn.position, Quaternion.identity);
            go.GetComponentInChildren<Fireball>().Target = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(string.Format("Fireball created at: {0}, going towards {1}", go.transform.position, go.GetComponentInChildren<Fireball>().Target));
            GetComponent<Player>().PlayerStats.Mana -= 1;
            yield return new WaitForSeconds(DelayBetweenShots);
        }
    }
    private void SetPlayerDir()
    {
        float dist = 0.01f;
        //Debug.Log("Distance between PrevPos & Pos is: " + Vector2.Distance(rb2D.position, prevPos));
        if ((rb2D.position.x > prevPos.x) && (Vector2.Distance(rb2D.position, prevPos) > dist)) // Check if player is going right
        {
            playerDir = PlayerDir.East;
        }
        else if ((rb2D.position.y > prevPos.y) && (Vector2.Distance(rb2D.position, prevPos) > dist)) //check if player is going up
        {
            playerDir = PlayerDir.North;
        }
        else if ((rb2D.position.x < prevPos.x) && (Vector2.Distance(rb2D.position, prevPos) > dist)) //Check if player is going left
        {
            playerDir = PlayerDir.West;
        }
        else if ((rb2D.position.y < prevPos.y) && (Vector2.Distance(rb2D.position, prevPos) > dist)) //Check if player is going down
        {
            playerDir = PlayerDir.South;
        }
       
    }

    void UpdateAnimation()
    {
        switch (playerDir)
        {
            case PlayerDir.North:
                animator.SetTrigger("playerDirUp");
                //Debug.Log("Player facing up!");
                break;
            case PlayerDir.South:
                animator.SetTrigger("playerDirDown");
                //Debug.Log("Player facing down!");
                break;
            case PlayerDir.East:
                animator.SetTrigger("playerDirRight");
                //Debug.Log("Player facing right!");
                break;
            case PlayerDir.West:
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
        if (GetComponent<Player>().PlayerStats.Health <= 0)
        {
            playerLock = true;
            animator.SetTrigger("playerDead");
            GetComponent<Player>().Invoke("CheckIfGameOver", 0.9f);

        }
    }

}


