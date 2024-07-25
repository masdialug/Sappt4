using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject reflectShield;
    public Slider reflectionBar;
    public float reflectionForceMultiplier = 2f;
    public float borderDamageRate = 1f; // Damage per second when near the border

    private int reflectionCount = 0;
    private bool isNearBorder = false;
    private float borderDamageTimer = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

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
        if (GameManager.Instance.currentHealth <= 0)
        {
            this.enabled = false; // Disable player movement
            Debug.Log("Player eliminated");
            return;
        }

        float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        // Update animations
        if (move != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            ReflectProjectiles();
        }

        // Apply border damage if near the border
        if (isNearBorder)
        {
            borderDamageTimer += Time.deltaTime;
            if (borderDamageTimer >= 1f / borderDamageRate)
            {
                GameManager.Instance.TakeDamage(1);
                animator.SetTrigger("TakeDamage");
                borderDamageTimer = 0f;
            }
        }
        else
        {
            borderDamageTimer = 0f;
        }
    }

    void ReflectProjectiles()
    {
        animator.SetTrigger("Reflect");

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Border"))
        {
            isNearBorder = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Border"))
        {
            isNearBorder = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            GameManager.Instance.TakeDamage(1);
            animator.SetTrigger("TakeDamage");
            Destroy(collision.gameObject);
        }
    }
}
