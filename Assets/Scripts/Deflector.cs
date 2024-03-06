using UnityEngine;

public class Deflector : MonoBehaviour
{
    public float deflectionStrength = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        if (projectile != null && !projectile.hasBeenDeflected)
        {
            
            Vector2 deflectionDirection = Vector2.Reflect(collision.relativeVelocity.normalized, collision.contacts[0].normal);

            
            deflectionDirection.Normalize();

            
            projectile.Deflect(deflectionDirection * deflectionStrength, this.GetComponent<SpriteRenderer>().color);
        }
    }
}
