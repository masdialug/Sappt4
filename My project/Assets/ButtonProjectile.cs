using UnityEngine;

public class ButtonProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int maxBounces = 16; // Maximum number of bounces before disappearing
    public PhysicsMaterial2D bounceMaterial; // Physics Material for bouncing

    private int bounceCount = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Assign the bounce material to the Rigidbody2D
        if (bounceMaterial != null)
        {
            rb.sharedMaterial = bounceMaterial;
        }
        else
        {
            Debug.LogError("Bounce material is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        // Destroy the projectile if it exceeds the maximum number of bounces
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            bounceCount++;

            // Ensure the projectile doesn't hit the top of the screen by reversing its vertical velocity if necessary
            if (collision.contacts[0].normal.y > 0 && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            GameManager.Instance.TakeDamage(2); // Double damage
            Destroy(gameObject); // Destroy projectile on hit
        }
    }

    public void SetUp(int bounces)
    {
        maxBounces = bounces;
    }
}
