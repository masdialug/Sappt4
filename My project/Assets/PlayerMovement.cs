using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject reflectShield;
    public Slider reflectionBar;
    public Slider healthBar; // Health bar for the player
    public int maxHealth = 5; // Set max health to 5
    public float reflectionForceMultiplier = 2f;

    private int currentHealth;
    private int reflectionCount = 0;

    void Start()
    {
        currentHealth = maxHealth;

        // Ensure healthBar is assigned
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // Initialize health bar max value
            healthBar.value = currentHealth; // Initialize health bar current value
        }
        else
        {
            Debug.LogError("Health bar is not assigned in the Inspector.");
        }

        // Ensure reflectionBar is assigned
        if (reflectionBar != null)
        {
            reflectionBar.maxValue = 7; // Set max value for reflection bar
            reflectionBar.value = reflectionCount; // Initialize reflection bar value
        }
        else
        {
            Debug.LogError("Reflection bar is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        if (Input.GetButtonDown("Jump"))
        {
            ReflectProjectiles();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Update health bar
        }

        if (currentHealth <= 0)
        {
            // Handle player death
            gameObject.SetActive(false);
            Debug.Log("Player eliminated");
        }
    }

    void ReflectProjectiles()
    {
        if (reflectShield == null) return;

        Collider2D[] projectiles = Physics2D.OverlapBoxAll(reflectShield.transform.position, reflectShield.transform.localScale, 0);

        foreach (Collider2D col in projectiles)
        {
            if (col.CompareTag("Projectile"))
            {
                Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 reflectionDirection = ((Vector2)(col.transform.position - transform.position)).normalized;
                    rb.velocity = reflectionDirection * rb.velocity.magnitude * reflectionForceMultiplier; // Stronger reflection force
                    reflectionCount++;

                    if (reflectionBar != null)
                    {
                        reflectionBar.value = reflectionCount; // Update reflection bar
                    }

                    if (reflectionCount >= 7)
                    {
                        reflectionCount = 0;
                        if (reflectionBar != null)
                        {
                            reflectionBar.value = 0;
                        }
                    }

                    Destroy(col.gameObject, 2f); // Destroy projectile after 2 seconds
                }
            }
        }
    }
}
