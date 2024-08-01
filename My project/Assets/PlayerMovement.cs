using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f; // Add jump force for reflecting
    public GameObject reflectShield;
    public Slider reflectionBar;
    public float reflectionForceMultiplier = 2f;
    public float borderDamageRate = 1f; // Damage per second when near the border
    public int maxReflections = 7; // Maximum number of reflections
    public float reflectionCooldown = 5f; // Cooldown duration in seconds

    private int currentReflections;
    private bool isNearBorder = false;
    private float borderDamageTimer = 0f;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isReflecting = false; // Flag to indicate if the player is reflecting
    private bool inCooldown = false; // Flag to indicate if the player is in cooldown
    private float cooldownTimer = 0f; // Timer for cooldown

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (reflectShield == null)
        {
            Debug.LogError("ReflectShield is not assigned in the Inspector.");
        }

        if (reflectionBar == null)
        {
            Debug.LogError("ReflectionBar is not assigned in the Inspector.");
        }
        else
        {
            reflectionBar.maxValue = maxReflections;
            currentReflections = maxReflections;
            reflectionBar.value = currentReflections;
        }
    }

    void Update()
    {
        if (GameManager.Instance.currentHealth <= 0)
        {
            this.enabled = false; // Disable player movement
            animator.SetTrigger("Die"); // Trigger death animation
            Debug.Log("Player eliminated");
            return;
        }

        float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        // Update animations
        if (Mathf.Abs(move) > 0.01f) // Small threshold to prevent idle flicker
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetButtonDown("Jump") && !inCooldown && currentReflections > 0)
        {
            StartReflecting();
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

        // Handle cooldown
        if (inCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= reflectionCooldown)
            {
                EndCooldown();
            }
        }
    }

    void StartReflecting()
    {
        animator.SetTrigger("Reflect");
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Add jump force
        isReflecting = true;
        currentReflections--;
        reflectionBar.value = currentReflections;
        Invoke("StopReflecting", animator.GetCurrentAnimatorStateInfo(0).length); // Stop reflecting when the animation ends

        if (currentReflections <= 0)
        {
            StartCooldown();
        }
    }

    void StopReflecting()
    {
        isReflecting = false;
    }

    void StartCooldown()
    {
        inCooldown = true;
        cooldownTimer = 0f;
        reflectionBar.value = 0;
    }

    void EndCooldown()
    {
        inCooldown = false;
        currentReflections = maxReflections;
        reflectionBar.value = currentReflections;
    }

    void ReflectProjectiles()
    {
        if (!isReflecting || reflectShield == null) return;

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
                }
                Destroy(col.gameObject, 2f); // Destroy projectile after 2 seconds
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

    void FixedUpdate()
    {
        if (isReflecting)
        {
            ReflectProjectiles();
        }
    }
}
