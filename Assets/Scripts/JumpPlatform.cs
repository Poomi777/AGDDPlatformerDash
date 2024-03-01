using AGDDPlatformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : KinematicObject
{
    [SerializeField] private float jumpSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.transform.position.y  < 
            gameObject.transform.position.y + gameObject.transform.localScale.y / 2.0f)
        { 
            return;
        }
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

        if (playerController == null) { return; }

        playerController.SetJumpPadBoost(new Vector2(0.0f, jumpSpeed));
    }
}
