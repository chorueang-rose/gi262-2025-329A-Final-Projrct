using UnityEngine;
using Solution;

public class GreenPotion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OOPPlayer player = other.GetComponent<OOPPlayer>();

            if (player != null)
            {
                player.ActivateGreenPotion();
                Destroy(gameObject);
            }
        }
    }
}