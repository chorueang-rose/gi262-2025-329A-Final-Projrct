using UnityEngine;

namespace Solution
{
    public class CollectAbleItem : Identity
    {
        public override bool Hit()
        {
            Debug.Log("Item: " + Name + " has been picked up.");

            // 1. เพิ่มไอเทมเข้ากระเป๋าผู้เล่น (ส่งชื่อ และ จำนวน 1 ชิ้น)
            if (mapGenerator != null && mapGenerator.player != null && mapGenerator.player.inventory != null)
            {
                mapGenerator.player.inventory.AddItem(Name, 1);
            }

            // 2. เคลียร์ตำแหน่งไอเทมออกจากตาราง mapdata
            if (mapGenerator != null && mapGenerator.mapdata != null)
            {
                mapGenerator.mapdata[positionX, positionY] = null;
            }

            // 3. ทำลายไอเทมออกจากฉาก
            Destroy(gameObject);

            return true;
        }
    }
}
