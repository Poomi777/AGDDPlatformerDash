using AGDDPlatformer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class EnemyController : KinematicObject, IResettable
{

    [SerializeField] private float speed;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private Vector3 startPos;

    private bool isGointToEnd = true;
    private bool isWaiting = false;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bCol;

    new void Start()
    {
        GameManager.instance.resettableGameObjects.Add(this);
        startPos = transform.position;
    }

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bCol = GetComponent<BoxCollider2D>();
        spriteRenderer.flipX = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (startPoint != null && endPoint != null && !isWaiting)
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
                spriteRenderer.flipX = false;
                isWaiting = true;
                velocity = Vector2.zero;
                StartCoroutine(EnemyWait(1.0f));
            }
            else if (!isGointToEnd && Vector2.Dot(progressToStart, -startToEnd) <= 0)
            {
                isGointToEnd = true;
                spriteRenderer.flipX = true;
                isWaiting = true;
                velocity = Vector2.zero;
                StartCoroutine(EnemyWait(1.0f));
            }

        }
        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile != null && projectile.hasBeenDeflected)
            {
                
                spriteRenderer.enabled = false;
                bCol.enabled = false;
                gameObject.tag = "Untagged";
            }
        }
    }

    public void resetGameObject()
    {
        
        gameObject.transform.position = startPos;
        spriteRenderer.enabled = true;
        bCol.enabled = true;
        gameObject.tag = "Damager";

    }

    public bool isDestructible()
    {

        return false;
    }

    private IEnumerator EnemyWait(float delay)
    {
       

        yield return new WaitForSeconds(delay);

        
        isWaiting = false;
    }
}