using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Desired AI
//The enemy moves forward each frame and checks for a wall or cliff close to them.
//If it sees one it turns around. If it does not see one, it checks farther in front for the player.
//If it sees the player in its sight, it will move faster while this is true this happens until the player leaves sight.
//Once it gets into an attack range of the player, it swipes its arm to damage the player.
//It then waits one second, if the player is still in range it repeats the swipe. If they are not in range it charges the player. If they are not in sight it slows down.
namespace Panda.Examples.Move
{
    public class PatrollingEnemy : Enemy
    {
        public float ourDirection;
        
        public Rigidbody2D ourBody;

        RaycastHit2D ourhit;
        public float TurnAroundDistance;
        public float AggroDistance;

        public void Start()
        {
            ourDirection = UnityEngine.Random.value;
            ourDirection = ourDirection > 0.5 ? 1 : -1;
            if (ourDirection == -1) { transform.Rotate(0, 180, 0); }
            Debug.Log(ourDirection + "Dir");
            ourBody = GetComponent<Rigidbody2D>();
        }

        public void Flip()
        {
            ourDirection = ourDirection * -1;
            transform.Rotate(0, 180, 0);
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
        public bool CheckForPlayer()
        {
            ourhit = Physics2D.Raycast(
               new Vector2(transform.position.x, transform.position.y),
               new Vector2(ourDirection, 0),
               AggroDistance,
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


        [Task]
        void Patrol()
        {
            if(CheckForWalls() || CheckForCliff())
            {

            }
            else if (CheckForPlayer())
            {

            }
            
        }

    }
}

