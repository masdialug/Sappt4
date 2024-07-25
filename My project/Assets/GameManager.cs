using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int maxHealth = 10;
    public int currentHealth;
    public Slider healthBar;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (healthBar != null)
            {
                DontDestroyOnLoad(healthBar.transform.root.gameObject);
                healthBar.maxValue = maxHealth; // Initialize health bar max value
                currentHealth = maxHealth;
                healthBar.value = currentHealth; // Initialize health bar current value
            }
            else
            {
                Debug.LogError("Health bar is not assigned in the Inspector.");
            }
        }
        else
        {
            Destroy(gameObject);
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
            Debug.Log("Player eliminated");
            // Add additional logic for player death here if needed
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Update health bar
        }
    }
}
