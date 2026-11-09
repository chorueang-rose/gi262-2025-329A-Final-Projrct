using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Applies an elemental buff to the active player.
    /// Player.Instance is used when the scene uses the Player component.
    /// OOPPlayer.ActivePlayer provides compatibility with existing OOPPlayer
    /// prefabs that have not yet been replaced.
    /// </summary>
    public sealed class ElementalPotion : PotionItem
    {
        [Header("Potion")]
        [SerializeField]
        private ElementType element = ElementType.Fire;

        public override void OnPlayerCollect()
        {
            // เรียกผลลัพธ์พื้นฐานจาก PotionItem (เช่น ข้อความ Log / Heal)
            base.OnPlayerCollect();

            Player player = Player.Instance;

            if (player != null)
            {
                player.SetElement(element);
                Debug.Log($"Player received element buff: {element}");
            }
            else if (OOPPlayer.ActivePlayer != null)
            {
                OOPPlayer.ActivePlayer.SetElement(element);
                Debug.Log($"OOPPlayer received element buff: {element}");
            }
            else
            {
                Debug.LogWarning("No active player was found for ElementalPotion.");
            }

            // Note: ไม่ต้องเรียก RemoveFromMap() ซ้ำที่นี่ เพราะ ItemBase.Hit() เป็นคนจัดการให้อยู่แล้วครับ
        }
    }
}