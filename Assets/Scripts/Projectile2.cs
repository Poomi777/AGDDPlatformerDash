using System;
using UnityEngine;
using System.Collections.Generic;
using AGDDPlatformer;



// THIS FILE IS NOW REDUNDANT, ALL FUNCTIONALITY HAS BEEN MOVED TO PROJECTILE.CS















public class Projectile2 : MonoBehaviour
{
    public float speed = 5f;
    public float maxSpeed = 15f;
    public bool hasBeenDeflected = false;
    

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; //reference to player spriterenderer so we can copy their color for the deflect


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.velocity = transform.right * speed; //the projectiel will fly whereever it's x-axis right direction is facing
    }

    void Update()
    {
        RotateMovementDirection();
    }

    public void Deflect(Vector2 deflectionDirection, Color color)
    {
        hasBeenDeflected = true;
        speed = Mathf.Min(speed * 1.5f, maxSpeed);
        rb.velocity = deflectionDirection.normalized * speed;
        spriteRenderer.color = color;
        RotateMovementDirection();
    }

    private void RotateMovementDirection()
    {
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController != null && playerController.isDashing)
            {
                Vector2 deflectionDirection = playerController.CalculateDeflectionDirection(collision);
                Deflect(deflectionDirection, playerController.GetPlayerColor());
            }

            else if (!hasBeenDeflected)
            {
                GameManager.instance.ResetLevel();
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

}