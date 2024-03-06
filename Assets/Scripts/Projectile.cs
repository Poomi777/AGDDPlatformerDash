using System;
using UnityEngine;
using System.Collections.Generic;
using AGDDPlatformer;


public class Projectile : MonoBehaviour, IResettable
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
    private bool setToDestroy = false;

    void Start()
    {
        GameManager.instance.resettableGameObjects.Add(this);
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
        if (setToDestroy)
        {
            //GameManager.instance.resettableGameObjects.Remove(this);
            Destroy(gameObject);
        }
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
            // Vector2 deflectionDirection = new Vector2(0.0f, 0.0f);
            // Color playerCol = new Color(52.0f, 154.0f, 64.0f);
            // //Vector2 deflectionDirection = playerController.CalculateDeflectionDirection(collision);
            // Deflect(deflectionDirection, playerCol);

            Vector2 deflectionDirection = playerController.CalculateDeflectionDirection(collision);
            Deflect(deflectionDirection, playerController.GetPlayerColor());
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
            return;

            
        }

        if (collision.gameObject.CompareTag("Player1"))
        {
            

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
                setToDestroy = true;
            }
        }

        if (hasBeenDeflected && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            GameManager.instance.resettableGameObjects.Remove(this);
            //Destroy(gameObject);
            setToDestroy = true;
            
        }

        if (collision.gameObject.CompareTag("TilemapGround"))
        {
            setToDestroy = true;
        }

        
        
    }

    public void resetGameObject()
    {
        setToDestroy = true;
    }

    public bool isDestructible()
    {
        return true;
    }

}