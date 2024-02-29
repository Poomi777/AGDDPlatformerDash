using UnityEngine;

namespace AGDDPlatformer
{
    public class DoorPlatform : MovingPlatform
    {
        
        private bool activated = false;
        private bool shouldMove = false;

        protected override void Update()
        {
            if (!activated || isFrozen) 
            {
                return;
            }

            base.Update(); 

            if (shouldMove)
            {
               if (GoingTowards == Points.End && Vector2.Dot(progressToEnd, startToEnd) <= 0)
                {
                    
                    StopPlatform();
                }
                else if (GoingTowards == Points.Start && Vector2.Dot(progressToStart, -startToEnd) <= 0)
                {
                    StopPlatform();
                } 
            }
        }

        public void ActivatePlatform()
        {
            Debug.Log("Activating platform.");
            activated = !activated;
            
            if (activated)
            {
                 if (AtEndPoint())
                {
                    GoingTowards = Points.Start;
                }

                else if (AtStartPoint())
                {
                    GoingTowards = Points.End;
                }
                isFrozen = false;
            }
        }

        private void StopPlatform()
        {
            Debug.Log("stopping platform.");
            isFrozen = true;
            velocity = Vector2.zero; //stop platform moving
            shouldMove = false;
        }

        private bool AtStartPoint()
        {
            return Vector2.Distance(transform.position, StartPoint.position) < 0.1f;
        }

        private bool AtEndPoint()
        {
            return Vector2.Distance(transform.position, EndPoint.position) < 0.1f;
        }
    }
}
