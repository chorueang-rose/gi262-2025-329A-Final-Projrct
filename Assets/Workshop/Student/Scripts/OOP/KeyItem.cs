using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Collecting a key updates GameManager and unlocks the scene exit.
    /// </summary>
    public sealed class KeyItem : ItemBase
    {
        [SerializeField]
        private OOPExit exit;

        public override void OnPlayerCollect()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("KeyItem requires a GameManager in the scene.");
                return;
            }

            // เพิ่มจำนวนกุญแจเข้า GameManager
            GameManager.Instance.AddKey();

            // ค้นหา OOPExit หากไม่ได้กำหนดไว้ใน Inspector
            OOPExit targetExit = exit != null
                ? exit
                : FindFirstObjectByType<OOPExit>();

            if (targetExit != null)
            {
                targetExit.Unlock();
            }

            // Note: ไม่ต้องสั่ง RemoveFromMap() ที่นี่ เพราะ ItemBase.Hit() เป็นคนจัดการให้อยู่แล้วครับ
        }
    }
}