using UnityEngine;

namespace AGDDPlatformer
{
    public class DoorPlatform : MovingPlatform
    {
        protected override void Update()
        {
            if (isFrozen) 
                return;

            base.Update(); 

            
            if (GoingTowards == Points.End && Vector2.Dot(progressToEnd, startToEnd) <= 0)
            {
                
                StopPlatform();
            }
            else if (GoingTowards == Points.Start && Vector2.Dot(progressToStart, -startToEnd) <= 0)
            {
                StopPlatform();
            }
        }

        public void ActivatePlatform()
        {
            
            if (isFrozen)
            {
                GoingTowards = GoingTowards == Points.Start ? Points.End : Points.Start;
                isFrozen = false;
            }
            else
            {
                //if platform is already moving, do nothing for now
            }
        }

        private void StopPlatform()
        {
            isFrozen = true;
            velocity = Vector2.zero; //stop platform moving
        }
    }
}
