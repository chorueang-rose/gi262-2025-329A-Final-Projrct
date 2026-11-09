using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Base class for every object placed on the dungeon grid.
    /// The map generator stores these objects in its Identity[,] map.
    /// </summary>
    public class Identity : MonoBehaviour
    {
        [Header("Identity")]
        public string Name;
        public int positionX;
        public int positionY;

        public OOPMapGenerator mapGenerator;

        private void Start()
        {
            SetUP();
        }

        public virtual void SetUP()
        {
        }

        public void PrintInfo()
        {
            Debug.Log(
                "Created " + Name + " at " + positionX + ":" + positionY);
        }

        public virtual bool Hit()
        {
            return false;
        }
    }
}