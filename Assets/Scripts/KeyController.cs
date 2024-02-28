using UnityEngine;

namespace AGDDPlatformer
{
    public class KeyController : MonoBehaviour
    {
        public GameObject activeIndicator;
        public float cooldown = 2;
        public GameObject controlledObject;
        public AudioSource source;
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
            if (!isActive)
                return;

            PlayerController playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                controlledObject.GetComponent<MovingPlatform>().isFrozen = false;
                isActive = false;
                lastCollected = Time.time;
                activeIndicator.SetActive(false);
                source.Play();
            }
        }
    }
}
