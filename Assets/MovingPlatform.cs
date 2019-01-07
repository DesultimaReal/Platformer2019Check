using UnityEngine;
using System.Collections;
using Panda;

namespace Panda.Examples.Move
{
    public class MovingPlatform : MonoBehaviour
    {

        float speed = 2.5f; // current speed
        float waitTime = 2.0f;
        Vector2 origin;
        

        public void Awake()
        {
            origin = transform.position;
        }
        /*
         * Move to the (x,y) position at the current speed.
         */
        [Task]
        void MoveTo(float x, float y)
        {
            Vector3 destination = new Vector2(origin.x + x, origin.y + y);
            Vector3 delta = destination - transform.position;
            Vector3 velocity = speed * delta.normalized;

            transform.position = transform.position + velocity * Time.deltaTime;

            Vector3 newDelta = (destination - transform.position);
            float d = newDelta.magnitude;
            if (Task.isInspected)
                Task.current.debugInfo = string.Format("d={0:0.000}", d);
            
            if(Vector3.Dot(delta, newDelta) <= 0.0f || d < 1e-3)
            {
                transform.position = destination;
                Task.current.Succeed();
                d = 0.0f;
                Task.current.debugInfo = "d=0.000";
            }
        }
    }
}
