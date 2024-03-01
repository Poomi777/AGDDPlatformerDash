using UnityEngine;

namespace AGDDPlatformer
{
    public class DashGem : MonoBehaviour
    {
        public GameObject activeIndicator;
        public float cooldown = 2;
        public AudioSource source;
        public bool isDeflectGem;
        public MovingPlatform controlledPlatform;
        float lastCollected;
        bool isActive;

        void Awake()
        {
            lastCollected = -cooldown * 2;
        }

        void Update()
        {
            if (!isActive && Time.time - lastCollected >= cooldown)
            {
                isActive = true;
                activeIndicator.SetActive(true);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            DoorPlatform doorPlatform = controlledPlatform as DoorPlatform;
            if (!isActive)
                return;

            //DeflectGem logics
            if (isDeflectGem && other.CompareTag("Projectile"))
            {
                //Projectile projectile = other.GetComponent<Projectile>();
                

                if (doorPlatform != null)
                {
                    doorPlatform.ActivatePlatform();
                }
                else
                {
                    controlledPlatform.isFrozen = !controlledPlatform.isFrozen;
                }

                // if (controlledPlatform != null)
                // {
                //     controlledPlatform.isFrozen = !controlledPlatform.isFrozen;
                //     if (!controlledPlatform.isFrozen)
                //     {
                //         isActive = false;
                //         lastCollected = Time.time;
                //         activeIndicator.SetActive(false);
                //         source.Play();

                //     }
                // }

                isActive = false;
                lastCollected = Time.time;
                activeIndicator.SetActive(false);
                source.Play();

                Destroy(other.gameObject);
                
            }

            else if (!isDeflectGem && other.CompareTag("Player1")) //regular DashGem logic
            {
                PlayerController playerController = other.GetComponentInParent<PlayerController>();
                
                if (doorPlatform != null)
                {
                    doorPlatform.ActivatePlatform();
                }
                else
                {
                    controlledPlatform.isFrozen = !controlledPlatform.isFrozen;
                }
                
                if (playerController != null)
                {
                    playerController.ResetDash();
                    isActive = false;
                    lastCollected = Time.time;
                    activeIndicator.SetActive(false);
                    source.Play();
                }
            }
        }
    }
}
