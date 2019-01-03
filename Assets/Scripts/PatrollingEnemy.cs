using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : Enemy
{
    public float ourDirection;
    public float TurnAroundDistance;
    public Rigidbody2D ourBody;
    RaycastHit2D ourhit;

    public void Start()
    {
        ourDirection = UnityEngine.Random.value;
        ourDirection = ourDirection > 0.5 ? 1 : -1;
        if(ourDirection == -1) { transform.Rotate(0, 180, 0); }
        Debug.Log(ourDirection + "Dir");
        ourBody = GetComponent<Rigidbody2D>();
    }
    public void Flip()
    {
        ourDirection = ourDirection * -1;
        transform.Rotate(0,180,0);
    }

    public bool CheckForWalls()
    {
         ourhit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y), 
            new Vector2(ourDirection, 0), 
            TurnAroundDistance, 
            1 << LayerMask.NameToLayer("Walls")
            );
        return (ourhit.collider != null);
    }

    public void FixedUpdate()
    {
        WalkDirection(speed);
        if (CheckForWalls() || !CheckForCliff())
        {
            Flip();
        }
    }

    private bool CheckForCliff()
    {
        ourhit = Physics2D.Raycast(
               new Vector2(transform.position.x + ourDirection, transform.position.y),
               new Vector2(0, -1),
               2,
               1 << LayerMask.NameToLayer("Walls")
               );
        return (ourhit.collider != null);
    }

    private void WalkDirection(float speed)
    {
        ourBody.velocity = (new Vector2(ourDirection * speed, ourBody.velocity.y));
    }
}
