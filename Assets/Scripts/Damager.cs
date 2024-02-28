using AGDDPlatformer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Damager : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController playerController = collision.gameObject.gameObject.GetComponent<PlayerController>();

        if (playerController == null) { return; }

        GameManager.instance.ResetLevel();
    }
}
