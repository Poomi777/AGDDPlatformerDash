using System;
using UnityEngine;
using System.Collections.Generic;
using AGDDPlatformer;


public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float maxSpeed = 15f;
    public bool hasBeenDeflected = false;
    public bool isStraightMovingProjectile = false;

    public GameObject deflectionEffectPrefab;
    public AudioClip deflectionSound;
    

    private Rigidbody2D rb;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer; //reference to player spriterenderer so we can copy their color for the deflect


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //GameObject player = GameObject.FindGameObjectWithTag("Player1");

        if (isStraightMovingProjectile)
        {
            rb.velocity = transform.right * speed;
        }

        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player1");
            if (player != null)
            {
                playerTransform = player.transform;
                MoveTowardsPlayer();
            }

            else
            {
                Debug.LogError("Player not found: Check the player tag.");
            }
        }
    }

    void Update()
    {
        RotateMovementDirection();
    }

    void MoveTowardsPlayer()
    {
        if (!isStraightMovingProjectile && playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            rb.velocity = direction * speed;
        }
    }

    public void Deflect(Vector2 deflectionDirection, Color color)
    {
        hasBeenDeflected = true;
        speed = Mathf.Min(speed * 1.8f, maxSpeed);
        rb.velocity = deflectionDirection.normalized * speed;
        spriteRenderer.color = color;

        if(deflectionEffectPrefab != null)
        {
            Instantiate(deflectionEffectPrefab, transform.position, Quaternion.identity);
        }

        if(deflectionSound != null)
        {
            AudioSource.PlayClipAtPoint(deflectionSound, transform.position);
        }

        RotateMovementDirection();
        gameObject.layer = LayerMask.NameToLayer("DeflectedProjectile");
    }

    private void RotateMovementDirection()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        
        if (collision.gameObject.CompareTag("Deflector"))
        {
            Vector2 deflectionDirection = playerController.CalculateDeflectionDirection(collision);
            Deflect(deflectionDirection, playerController.GetPlayerColor());
        }

        if (collision.gameObject.CompareTag("Player1"))
        {
            // if (hasBeenDeflected)
            // {
            //     Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
            // }

            if (playerController != null && playerController.isDashing)
            {
                Vector2 deflectionDirection = playerController.CalculateDeflectionDirection(collision);
                Deflect(deflectionDirection, playerController.GetPlayerColor());

            }

            if (hasBeenDeflected)
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
                return;
            }

            else if (!hasBeenDeflected)
            {
                
                GameManager.instance.ResetLevel();
                Destroy(gameObject);
            }
        }

        if (hasBeenDeflected && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        
        Destroy(gameObject);
    }

}