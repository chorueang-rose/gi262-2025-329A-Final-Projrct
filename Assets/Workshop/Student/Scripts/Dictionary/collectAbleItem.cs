using System.Security.Principal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Solution
{
    public class CollectAbleItem : Identity
    {
        public override bool Hit()
        {
            Debug.Log("Item: " + Name + " has been picked up.");

            // เพิ่มไอเทมเข้ากระเป๋าผู้เล่น
            mapGenerator.player.inventory.AddItem(Name, 1);

            // เคลียร์ตำแหน่งไอเทมออกจากตาราง mapData ก่อน
            if (mapGenerator != null)
            {
                mapGenerator.mapdata[positionX, positionY] = null;
            }

            // ทำลายไอเท็มออกจากฉาก
            Destroy(gameObject);

            return true;
        }
    }
}

