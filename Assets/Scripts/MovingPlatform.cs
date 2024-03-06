using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AGDDPlatformer
{
    public class MovingPlatform : KinematicObject, IResettable
    {
        public float Speed; //Probably should be the same as player max speed
        public Transform StartPoint;
        public Transform EndPoint;
        public Vector2 startPos;

        protected enum Points
        {
            Start, End
        }

        protected Points GoingTowards = Points.End;
        protected Vector2 startToEnd;
        protected Vector2 progressToEnd;
        protected Vector2 progressToStart;
        
        new void Start()
        {
            GameManager.instance.resettableGameObjects.Add(this);
            startPos = transform.position;
        }

        protected virtual void Update()
        {
            if (!isFrozen)
            {

            //Move the platform back and forth between the start and end points
            Vector2 startToEnd = EndPoint.position - StartPoint.position;
            Vector2 progressToEnd = EndPoint.position - transform.position;
            Vector2 progressToStart = StartPoint.position - transform.position;
            

                if (GoingTowards == Points.End)
                {
                    velocity = progressToEnd.normalized * Speed;
                }
                else
                {
                    velocity = progressToStart.normalized * Speed;
                }

                if (GoingTowards == Points.End && Vector2.Dot(progressToEnd, startToEnd) <= 0)
                {
                    GoingTowards = Points.Start;
                }
                else if (GoingTowards == Points.Start && Vector2.Dot(progressToStart, -startToEnd) <= 0)
                {
                    GoingTowards = Points.End;
                }
            }
        }

        void OnCollisionStay2D(Collision2D other)
        {
            var otherBody = other.gameObject.GetComponent<KinematicObject>();
            if (otherBody == null) { return; }

            //Attatch if something is grounded on the platform
            if (otherBody.GetGroundedOnObject() == gameObject)
            {
                otherBody.AttatchTo(this);
            }
            else
            {
                //If it is not grounded we can detatch. 
                otherBody.Detatch();
                //If it is a player, give them a small boost to simulate inertia.
                otherBody.GetComponent<PlayerController>()?.SetJumpBoost(new Vector2(velocity.x, 0));
            }
        }

        void OnCollisionExit2D(Collision2D other)
        {
            var otherBody = other.gameObject.GetComponent<KinematicObject>();
            if (otherBody == null) { return; }

            //We can detatch if the object exits the collision.
            otherBody.Detatch();
            //If it is a player, give them a small boost to simulate inertia.
            if (!isFrozen)
            otherBody.GetComponent<PlayerController>()?.SetJumpBoost(new Vector2(velocity.x, 0));
        }

        public void StartMoving()
        {
            isFrozen = false;
        }

        public void resetGameObject()
        {

            gameObject.transform.position = startPos;
            isFrozen = true;
        }

        public bool isDestructible()
        {

            return false;
        }

    }
}