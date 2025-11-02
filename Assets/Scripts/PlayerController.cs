using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 6;
    private Vector2 movementDirection;
    private Vector2 externalVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Move player
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");
        movementDirection.Normalize(); // Make sure directional movement isn't more
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (movementDirection * moveSpeed + externalVelocity) * Time.fixedDeltaTime); // Move player

        // Reset external velocity each frame
        externalVelocity = Vector2.zero;
    }

    public void AddExternalForce(Vector2 force)
    {
        externalVelocity += force;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Box") || collision.collider.CompareTag("Blue"))
        {
            Vector2 pushDirection = movementDirection;
            BoxController box = collision.collider.GetComponent<BoxController>();
            box.TryPush(pushDirection);
        }
    }
}
