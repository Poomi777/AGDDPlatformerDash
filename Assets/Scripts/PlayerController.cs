using Cinemachine;
using UnityEngine;

namespace AGDDPlatformer
{
    public class PlayerController : KinematicObject, IResettable
    {
        [Header("Movement")]
        public float maxSpeed = 7;
        public float jumpSpeed = 7;
        public float jumpDeceleration = 0.5f; // Upwards slow after releasing jump button
        public float cayoteTime = 0.1f; // Lets player jump just after leaving ground
        public float jumpBufferTime = 0.1f; // Lets the player input a jump just before becoming grounded

        [Header("Dash")]
        public float dashSpeed;
        public float dashTime;
        public float dashCooldown;
        public Color canDashColor;
        public Color cantDashColor;
        public TrailRenderer trailRenderer;
        float lastDashTime;
        Vector2 dashDirection;
        public bool isDashing;
        bool canDash;
        bool wantsToDash;

        [Header("Audio")]
        public AudioSource source;
        public AudioClip jumpSound;
        public AudioClip dashSound;

        Vector2 startPosition;
        bool startOrientation;

        float lastJumpTime;
        float lastGroundedTime;
        bool canJump;
        bool jumpReleased;
        Vector2 move;
        float defaultGravityModifier;

        SpriteRenderer spriteRenderer;

        Vector2 jumpBoost;

        Vector2 jumpPadBoost;
        bool isJumpPadBoosting;
        bool isJumpPadReleased;


        [Header("Effects")]
        public GameObject dashEffect;
        public float dashEffectDuration = 0.5f;
        public float dashOffsetDistance = 0.3f;

        public GameObject deathEffect;
        public AudioClip deathSound;

        [Header("Animation")]
        public Animator animator;
        private bool isMoving;

        private BoxCollider2D bCol;

        private bool isDead = false;

        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            bCol = GetComponentInChildren<BoxCollider2D>();
            trailRenderer.enabled = false;

            lastJumpTime = -jumpBufferTime * 2;

            startPosition = transform.position;
            startOrientation = spriteRenderer.flipX;
            

            defaultGravityModifier = gravityModifier;
        }

        new void Start()
        {
            GameManager.instance.checkPointPosition = transform.position;
            GameManager.instance.resettableGameObjects.Add(this);

        }

        void Update()
        {
            isFrozen = GameManager.instance.timeStopped;

            /* --- Read Input --- */
            if (!isDead)
            {
                move.x = Input.GetAxisRaw("Horizontal");
                if (gravityModifier < 0)
                {
                    move.x *= -1;
                }

                move.y = Input.GetAxisRaw("Vertical");

                if (Input.GetButtonDown("Jump"))
                {
                    // Store jump time so that we can buffer the input
                    lastJumpTime = Time.time;
                }

                if (Input.GetButtonUp("Jump"))
                {
                    jumpReleased = true;
                }

                // Clamp directional input to 8 directions for dash
                Vector2 desiredDashDirection = new Vector2(
                    move.x == 0 ? 0 : (move.x > 0 ? 1 : -1),
                    move.y == 0 ? 0 : (move.y > 0 ? 1 : -1));
                if (desiredDashDirection == Vector2.zero)
                {
                    // Dash in facing direction if there is no directional input;
                    desiredDashDirection = spriteRenderer.flipX ? -Vector2.right : Vector2.right;
                }
                desiredDashDirection = desiredDashDirection.normalized;
                if (Input.GetButtonDown("Dash"))
                {
                    wantsToDash = true;
                }

                /* --- Compute Velocity --- */

                if (canDash && wantsToDash)
                {
                    isDashing = true;
                    dashDirection = desiredDashDirection;
                    lastDashTime = Time.time;
                    canDash = false;
                    gravityModifier = 0;

                    Vector3 effectOffset = new Vector3(dashDirection.x, dashDirection.y, 0) * dashOffsetDistance;

                    if (dashDirection.x != 0 && dashDirection.y == 0) //side dashing
                    {
                        effectOffset += new Vector3(0, 0f, 0); //adjust the y value as needed
                    }
                    else if (dashDirection.y > 0) //dashing upwards
                    {
                        effectOffset += new Vector3(0, 0.3f, 0); //adjust the y value as needed
                    }
                    else if (dashDirection.y < 0) //dashing downwards
                    {
                        effectOffset += new Vector3(0, -0.2f, 0); //adjust the y value as needed
                    }

                    dashEffect.SetActive(true);
                    dashEffect.transform.localPosition = effectOffset; ; //how far the effect should appear from the center of the player
                    dashEffect.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg);

                    Invoke("DeactivateDashEffect", dashEffectDuration);

                    source.PlayOneShot(dashSound);
                }
                wantsToDash = false;

                if (isDashing)
                {
                    velocity = dashDirection * dashSpeed;
                    trailRenderer.enabled = true;

                    if (Time.time - lastDashTime >= dashTime)
                    {
                        isDashing = false;

                        gravityModifier = defaultGravityModifier;
                        if ((gravityModifier >= 0 && velocity.y > 0) ||
                            (gravityModifier < 0 && velocity.y < 0))
                        {
                            velocity.y *= jumpDeceleration;
                        }
                    }
                }
                else
                {
                    trailRenderer.enabled = false;

                    if (isGrounded && !isJumpPadReleased)
                    {
                        // Store grounded time to allow for late jumps
                        lastGroundedTime = Time.time;
                        canJump = true;
                        if (!isDashing && Time.time - lastDashTime >= dashCooldown)
                            canDash = true;
                    }

                    // Check time for buffered jumps and late jumps
                    float timeSinceJumpInput = Time.time - lastJumpTime;
                    float timeSinceLastGrounded = Time.time - lastGroundedTime;

                    if (canJump && timeSinceJumpInput <= jumpBufferTime && timeSinceLastGrounded <= cayoteTime)
                    {
                        velocity.y = Mathf.Sign(gravityModifier) * jumpSpeed;
                        canJump = false;
                        isGrounded = false;

                        source.PlayOneShot(jumpSound);
                    }
                    else if (jumpReleased)
                    {
                        // Decelerate upwards velocity when jump button is released
                        if ((gravityModifier >= 0 && velocity.y > 0) ||
                            (gravityModifier < 0 && velocity.y < 0))
                        {
                            velocity.y *= jumpDeceleration;
                        }
                        jumpReleased = false;
                    }

                    velocity.x = move.x * maxSpeed;

                    if (isGrounded || (velocity + jumpBoost).magnitude < velocity.magnitude)
                    {
                        jumpBoost = Vector2.zero;
                    }
                    else
                    {
                        velocity += jumpBoost;
                        jumpBoost -= jumpBoost * Mathf.Min(1f, Time.deltaTime);
                        //Debug.Log("Jumpboost: x = " + jumpBoost.x + ", y = " + jumpBoost.y);
                    }
                }

                /* --- Adjust Sprite --- */

                // Assume the sprite is facing right, flip it if moving left
                if (move.x > 0.01f)
                {
                    spriteRenderer.flipX = false;
                }
                else if (move.x < -0.01f)
                {
                    spriteRenderer.flipX = true;
                }

                if (isJumpPadBoosting)
                {
                    velocity += jumpPadBoost;
                    isJumpPadBoosting = false;
                    isJumpPadReleased = true;
                    source.PlayOneShot(jumpSound);
                    canDash = true;
                }
                if (isJumpPadReleased && isGrounded)
                {
                    isJumpPadReleased = false;
                    canJump = true;
                }

                spriteRenderer.color = canDash ? canDashColor : cantDashColor;

                isMoving = Mathf.Abs(move.x) > 0.01f;
                animator.SetBool("isMoving", isMoving);

                animator.SetBool("isDashing", isDashing);
            }
            else
            {
                velocity = Vector2.zero;
            }


        }

        public void ResetPlayer()
        {
            transform.position = GameManager.instance.checkPointPosition;
            spriteRenderer.flipX = startOrientation;

            lastJumpTime = -jumpBufferTime * 2;

            velocity = Vector2.zero;
            isDead = false;
        }

        public void resetGameObject()
        {
            transform.position = GameManager.instance.checkPointPosition;
            spriteRenderer.flipX = startOrientation;

            //lastJumpTime = -jumpBufferTime * 2;

            velocity = Vector2.zero;

            spriteRenderer.enabled = true;

            isDead = false;
            bCol.enabled = true;

        }

        public void ResetDash()
        {
            canDash = true;
        }

        //Add a short mid-air boost to the player (unrelated to dash). Will be reset upon landing.
        public void SetJumpBoost(Vector2 jumpBoost)
        {
            this.jumpBoost = jumpBoost;
        }

        public Vector2 CalculateDeflectionDirection(Collision2D collision)
        {
            // Vector2 incomingVector = -collision.relativeVelocity;
            // Vector2 normal = collision.contacts[0].normal;

            // Vector2 deflectionDirection = Vector2.Reflect(incomingVector, normal);

            return dashDirection.normalized;
        }
        public void SetJumpPadBoost(Vector2 jumpBoost)
        {
            this.jumpPadBoost = jumpBoost;
            isJumpPadBoosting = true;
            canJump = false;
            isGrounded = false;
        }

        public void Die()
        {
            if (!isDead)
            {
                Debug.Log("Player has died!");
                if (deathEffect != null)
                {
                    Instantiate(deathEffect, transform.position, Quaternion.identity);
                }

                if (deathSound != null)
                {
                    AudioSource.PlayClipAtPoint(deathSound, transform.position);
                }

                //GameObject newPlayer = Instantiate(gameObject, GameManager.instance.checkPointPosition, Quaternion.identity);

                spriteRenderer.enabled = false;
                bCol.enabled = false;
            }
            
            //Destroy(gameObject);
            isDead = true;
        }

        public Color GetPlayerColor()
        {
            return canDashColor;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (isDashing && collision.gameObject.CompareTag("Projectile"))
            {
                Vector2 deflectionDirection = CalculateDeflectionDirection(collision);
                //Color playerColor = spriteRenderer.color;
                collision.gameObject.GetComponent<Projectile>().Deflect(deflectionDirection, canDashColor);
            }
            else if (collision.gameObject.CompareTag("Damager"))
            {
                GameManager.instance.ResetLevel();
            }
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Checkpoint"))
            {
                CheckpointController checkpointController = collision.gameObject.GetComponent<CheckpointController>();
                if (checkpointController.checkpointIsEnabled)
                {
                    checkpointController.DisableCheckpoint();
                    GameManager.instance.SetCheckpointPosition(collision.gameObject.transform.position);

                }
            }
        }

        private void DeactivateDashEffect()
        {
            dashEffect.SetActive(false);
        }

        public bool isDestructible()
        {

            return false;
        }

    }
}
