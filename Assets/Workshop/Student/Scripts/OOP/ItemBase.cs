using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Base class for collectible map items.
    /// It integrates with Identity so OOPMapGenerator can store the item in
    /// its mapdata array and Character.Move can call Hit().
    /// </summary>
    public abstract class ItemBase : Identity
    {
        [Header("Item")]
        [SerializeField]
        private ItemType itemType;

        public ItemType ItemType => itemType;

        /// <summary>
        /// Executes the item-specific collection behavior.
        /// </summary>
        public abstract void OnPlayerCollect();

        /// <summary>
        /// Character.Move invokes Hit when the player enters this map cell.
        /// The item handles its own collection and allows the player to move in.
        /// </summary>
        public override bool Hit()
        {
            OnPlayerCollect();
            RemoveFromMap();
            return true;
        }

        protected void RemoveFromMap()
        {
            if (mapGenerator != null &&
                positionX >= 0 &&
                positionX < mapGenerator.X &&
                positionY >= 0 &&
                positionY < mapGenerator.Y)
            {
                mapGenerator.mapdata[positionX, positionY] = null;
            }

            Destroy(gameObject);
        }
    }
}