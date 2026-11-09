using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Base class for potion items.
    /// Concrete potion classes define the effect applied to the player.
    /// </summary>
    public abstract class PotionItem : ItemBase
    {
        [Header("Potion Properties")]
        [SerializeField]
        protected int healAmount = 20;

        public override void OnPlayerCollect()
        {
            // เพิ่มการทำงานพื้นฐานของโพชัน (เช่น ฟื้นฟูพลังงาน/พลังชีวิต)
            if (GameManager.Instance != null)
            {
                Debug.Log($"Collected {Name}! Restoration value: {healAmount}");
            }
        }
    }
}