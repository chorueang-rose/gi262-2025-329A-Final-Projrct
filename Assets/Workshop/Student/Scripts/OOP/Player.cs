using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Specification-compatible player component.
    /// It inherits all movement and combat behavior from OOPPlayer while
    /// exposing Player.Instance to item classes.
    /// </summary>
    public sealed class Player : OOPPlayer
    {
        public static Player Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        protected override void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            base.OnDestroy();
        }
    }
}