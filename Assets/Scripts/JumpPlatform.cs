using AGDDPlatformer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
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

    void OnTriggerExit2D(Collider2D other)
    {
        
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

        if (playerController == null) { return; }

        playerController.SetJumpPadBoost(new Vector2(0.0f, jumpSpeed));
    }
}
