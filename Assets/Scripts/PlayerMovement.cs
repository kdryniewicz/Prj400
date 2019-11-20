using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        if (rb2D == null)
        {
            rb2D = gameObject.GetComponent<Rigidbody2D>();
        }
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
    }
}
