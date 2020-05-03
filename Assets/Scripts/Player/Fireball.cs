using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public Animator animator;
    public Vector2 Target;
    public Statistics projStats;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
            //Quaternion rot = Quaternion.LookRotation(new Vector2(rb2d.position.x - Target.x, rb2d.position.y - Target.y), Vector3.forward); // set rotation to mouse position        }
           // rb2d.SetRotation(rot);

    }

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if(other.gameObject.tag == "Wall")
    //    {
            
    //    }
    //}


   public void OnHit()
    {
        animator.SetTrigger("hitSomething");
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, time);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       // Debug.Log("Fireball is at: " + transform.position);
        if (Target != null)
        {

            rb2d.MovePosition(Vector2.MoveTowards(rb2d.transform.position, Target, (projStats.Speed * Time.deltaTime)));
            //rb2d.MoveRotation(Vector3.Angle(transform.position, Target));

            //b2d.SetRotation(Vector3.Angle(transform.position, Target));
        }
    }
}
