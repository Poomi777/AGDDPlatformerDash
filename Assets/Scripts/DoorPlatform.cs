using UnityEngine;

namespace AGDDPlatformer
{
    public class DoorPlatform : MovingPlatform
    {
        
        protected override void Update()
        {

            base.Update(); 

            if (GoingTowards == Points.End && Vector2.Distance(transform.position, EndPoint.position) <= 0.1f)
            {
                StopPlatform();
                return;
            }
            else if (GoingTowards == Points.Start && Vector2.Distance(transform.position, StartPoint.position) <= 0.1f)
            {
                StopPlatform();
                return;
            } 
            
        }

        public void ActivatePlatform()
        {
            isFrozen = false;  
            

            if (AtEndPoint())
            {
                GoingTowards = Points.Start;
            }

            else if (AtStartPoint())
            {
                GoingTowards = Points.End;
            }
            

            Debug.Log($"Platform activated, moving towards {(GoingTowards == Points.Start ? "Start" : "End")} point.");
        }

        private void StopPlatform()
        {
            Debug.Log("stopping platform.");
            isFrozen = true;
            velocity = Vector2.zero; //stop platform moving
            
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
