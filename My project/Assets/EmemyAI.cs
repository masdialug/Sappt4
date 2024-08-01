using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public GameObject projectilePrefab;
    public GameObject buttonProjectilePrefab; // Reference to the button projectile prefab
    public Transform firePoint;
    public float projectileSpeed = 5f;
    public float fireRate = 2f;
    public float buttonProjectileCooldown = 6f; // Cooldown for the button projectile
    public float moveRangeX = 8f; // Range for random horizontal movement
    public int maxHits = 12; // Max hits before the enemy dies
    public string nextSceneName; // Name of the next scene to load after four hits

    private int hitCount = 0; // Counter for hits by reflected projectiles
    private float nextFireTime = 0f;
    private float nextButtonProjectileTime = 0f;
    private Vector2 targetPosition;

    void Start()
    {
        SetRandomTargetPosition();
    }

    void Update()
    {
        if (GameManager.Instance.currentHealth <= 0)
        {
            return; // Stop updating if the player is dead
        }

        // Move towards target position horizontally
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If the enemy reaches the target position, set a new random target position
        if ((Vector2)transform.position == targetPosition)
        {
            SetRandomTargetPosition();
        }

        // Fire regular projectiles at intervals
        if (Time.time > nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Fire button projectiles at intervals only in the second and third scenes
        if ((SceneManager.GetActiveScene().name == "SecondScene" || SceneManager.GetActiveScene().name == "ThirdScene") && Time.time > nextButtonProjectileTime)
        {
            FireButtonProjectile();
            nextButtonProjectileTime = Time.time + buttonProjectileCooldown;
        }
    }

    void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-moveRangeX, moveRangeX);
        targetPosition = new Vector2(randomX, transform.position.y);
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (projectile != null)
        {
            // Set the initial scale of the projectile to (1, 1, 1)
            projectile.transform.localScale = new Vector3(1f, 1f, 1f);

            // Adjust the size of the projectile to 2/3 the size of the player
            Vector3 playerScale = player.localScale;
            projectile.transform.localScale = new Vector3(playerScale.x * 2f / 3f, playerScale.y * 2f / 3f, playerScale.z);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 direction = ((Vector2)(player.position - firePoint.position)).normalized;
            rb.velocity = direction * projectileSpeed;
            Destroy(projectile, 5f); // Destroy projectile after 5 seconds
        }
    }

    void FireButtonProjectile()
    {
        GameObject buttonProjectile = Instantiate(buttonProjectilePrefab, firePoint.position, firePoint.rotation);

        if (buttonProjectile != null)
        {
            // Adjust the size of the button projectile to be similar to the player
            Vector3 playerScale = player.localScale;
            buttonProjectile.transform.localScale = new Vector3(playerScale.x * 0.8f, playerScale.y * 0.8f, playerScale.z);

            Rigidbody2D rb = buttonProjectile.GetComponent<Rigidbody2D>();
            Vector2 direction = Vector2.down + Vector2.right;
            rb.velocity = direction.normalized * projectileSpeed;
            ButtonProjectile buttonProjScript = buttonProjectile.GetComponent<ButtonProjectile>();
            buttonProjScript.SetUp(16); // Set up with max bounces and any other necessary parameters
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            hitCount++;
            Destroy(collision.gameObject); // Destroy projectile on hit

            if (hitCount == 4)
            {
                // Check if the current scene is the last level
                if (SceneManager.GetActiveScene().name == "LastLevel")
                {
                    // In the last level, the boss dies after four hits
                    Destroy(gameObject); // Destroy the boss
                    Debug.Log("Boss defeated in the last level");
                }
                else
                {
                    // In other levels, change to the next scene after four hits
                    SceneManager.LoadScene(nextSceneName);
                }
            }

            if (hitCount >= maxHits)
            {
                Destroy(gameObject); // Destroy enemy after maxHits
                Debug.Log("Enemy eliminated");
            }
        }
    }
}
