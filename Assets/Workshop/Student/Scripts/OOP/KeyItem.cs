using UnityEngine;

namespace Solution
{
    public class KeyItem : Identity
    {
        public string ItemType = "Key";
        public OOPExit Exit;

        public override void SetUP()
        {
            base.SetUP();
            Name = "Key";
        }

        public override bool Hit()
        {
            // ใช้ FindObjectOfType เพื่อให้รองรับ Unity ทุกเวอร์ชัน (แก้ Error CS0619)
            if (Exit == null)
            {
                Exit = FindObjectOfType<OOPExit>();
            }

            // สั่งปลดล็อกประตูทางออก
            if (Exit != null)
            {
                Exit.Unlock();
                Debug.Log("ทางออกปลดล็อกแล้ว!");
            }

            // ลบกุญแจออกจากระบบ Grid
            if (mapGenerator != null && mapGenerator.mapdata != null)
            {
                mapGenerator.mapdata[positionX, positionY] = null;
            }

            Destroy(gameObject);
            return true;
        }
    }
}