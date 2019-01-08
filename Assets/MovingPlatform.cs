using UnityEngine;
using System.Collections.Generic;
using Panda;

namespace Panda.Examples.Move
{
    public class MovingPlatform : MonoBehaviour
    {

        public List<Vector3> Destinations;
        int whichDest = 0;
        public Vector2 origin;

        float speed = 2.5f; // current speed
        float waitTime = 2.0f;
        
        
        



        public void Awake()
        {
            origin = transform.position;
            Destinations.Add(new Vector3(0,0,0));
        }
        /*
         * Move to the (x,y) position at the current speed.
         */
        [Task]
        void MoveTo()
        {
            Vector3 destination = new Vector2(origin.x + Destinations[whichDest].x, origin.y + Destinations[whichDest].y);
            Vector3 delta = destination - transform.position;
            Vector3 velocity = speed * delta.normalized;

            transform.position = transform.position + velocity * Time.deltaTime;

            Vector3 newDelta = (destination - transform.position);
            float d = newDelta.magnitude;
            if (Task.isInspected)
                Task.current.debugInfo = string.Format("d={0:0.000}", d);
            
            if(Vector3.Dot(delta, newDelta) <= 0.0f || d < 1e-3)
            {
                
                whichDest = (whichDest >= Destinations.Count - 1) ? 0 : whichDest+1;
                Debug.Log("whichDest changed to: " + whichDest);
                transform.position = destination;
                Task.current.Succeed();
                d = 0.0f;
                Task.current.debugInfo = "d=0.000";
            }
        }


        
        void OnDrawGizmos()
        {
            DrawLines();
            DrawSpheres(0.5f);
        }

        void DrawSpheres(float r)
        {
            Gizmos.DrawSphere(transform.position, r);//Draw a sphere where we are.
            var wp = Destinations;
            for (int i = 0; i < Destinations.Count; i++)
            {
                Gizmos.DrawSphere(wp[i] + transform.position, r);
            }
        }

        void DrawLines()
        {
            
            var wp = Destinations;

            //Draw the first one
            var p0 = transform.position;
            var p1 = Destinations[0] + transform.position;
            Gizmos.DrawLine(p0, p1);

            if(Destinations.Count > 1)
            {
                for (int i = 1; i < Destinations.Count; i++)
                {
                    p0 = wp[i] + transform.position;
                    p1 = wp[i - 1] + transform.position;
                    Gizmos.DrawLine(p0, p1);
                }
            }

            //Draw the last one
            Gizmos.DrawLine(Destinations[Destinations.Count-1] + transform.position, transform.position);
        }

    }
}
