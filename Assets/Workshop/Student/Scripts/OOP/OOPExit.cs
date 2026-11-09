using UnityEngine;

namespace Solution
{
    public class OOPExit : Identity
    {
        public bool isLocked = true;

        public override void SetUP()
        {
            base.SetUP();
            Name = "exit";
        }

        public void Unlock()
        {
            isLocked = false;
            Debug.Log("ทางออกปลดล็อกแล้ว!");
        }

        public override bool Hit()
        {
            if (isLocked)
            {
                Debug.Log("ทางออกยังล็อกอยู่! ต้องหากุญแจก่อน");
                return false;
            }

            Debug.Log("ผ่านด่านสำเร็จ!");
            // เพิ่มระบบจบเกม/เปลี่ยนฉากตามที่ต้องการได้ที่นี่
            return true;
        }
    }
}