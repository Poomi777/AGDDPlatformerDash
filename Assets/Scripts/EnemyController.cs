using AGDDPlatformer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class EnemyController : KinematicObject
{

    [SerializeField] private float speed;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private bool isGointToEnd = true;
    
   

    // Update is called once per frame
    void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            //
            Vector2 startToEnd = endPoint.position - startPoint.position;
            Vector2 progressToEnd = endPoint.position - transform.position;
            Vector2 progressToStart = startPoint.position - transform.position;
            if (isGointToEnd)
            {
                velocity = progressToEnd.normalized * speed;
            }
            else
            {
                velocity = progressToStart.normalized * speed;
            }

            if (isGointToEnd && Vector2.Dot(progressToEnd, startToEnd) <= 0)
            {
                isGointToEnd = false;
            }
            else if (!isGointToEnd && Vector2.Dot(progressToStart, -startToEnd) <= 0)
            {
                isGointToEnd = true;
            }




        }
        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

        if (playerController == null) { return; }

        playerController.ResetPlayer();
    }
}
