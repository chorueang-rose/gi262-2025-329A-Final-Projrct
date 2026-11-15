using UnityEngine;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public TMP_Text hpText;
    public TMP_Text deathText;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (deathText != null)
            deathText.gameObject.SetActive(false);

        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        isDead = true;

        if (deathText != null)
        {
            deathText.gameObject.SetActive(true);
            deathText.text = "YOU DIED";
        }

        Debug.Log("Player Died!");
    }
}