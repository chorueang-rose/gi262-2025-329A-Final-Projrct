using UnityEngine;
using Solution;

public class RedPotion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OOPPlayer player = other.GetComponent<OOPPlayer>();

            if (player != null)
            {
                player.ActivateRedPotion();
                Destroy(gameObject);
            }
        }
    }
}