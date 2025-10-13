using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public float moveDistance = 1f; // Distance moved when pushed
    public LayerMask collisionStop; // Layers that prevent box from moving past
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TryPush(Vector2 direction)
    {
            direction = direction.normalized;
            Vector2 posToGo = rb.position + direction * moveDistance; // Finds the location it should move to
        RaycastHit2D moveHit = Physics2D.Raycast(rb.position, direction, moveDistance, collisionStop); // Checks if its going to be moving into something it shouldn't
        if(moveHit.collider == null)
        {
            rb.MovePosition(posToGo); // Moves it
        }
    }
}
