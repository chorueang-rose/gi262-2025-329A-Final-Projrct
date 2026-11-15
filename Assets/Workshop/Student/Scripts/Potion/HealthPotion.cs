using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem health =
                other.GetComponent<HealthSystem>();

            if (health != null)
            {
                health.AddHealth(healAmount);
                Destroy(gameObject);
            }
        }
    }
}