using UnityEngine;

namespace Solution
{
    public class OOPPlayer : Character
    {
        public static OOPPlayer ActivePlayer { get; private set; }

        [Header("Player Combat")]
        [SerializeField]
        private int baseDamage = 10;

        [SerializeField]
        private ElementType currentElement = ElementType.Normal;

        [SerializeField]
        private float fireDamageMultiplier = 2f;

        [Header("Components")]
        public Inventory inventory;
        public ActionHistoryManager actionHistoryManager;

        public bool isAutoMoving = false;

        public int BaseDamage => baseDamage;
        public ElementType CurrentElement => currentElement;

        protected virtual void Awake()
        {
            ActivePlayer = this;
        }

        protected virtual void OnDestroy()
        {
            if (ActivePlayer == this)
            {
                ActivePlayer = null;
            }
        }

        public void SetElement(ElementType element)
        {
            currentElement = element;
            Debug.Log($"{Name} element changed to {element}.");
        }

        public int GetDamage()
        {
            if (currentElement == ElementType.Fire)
            {
                return Mathf.RoundToInt(baseDamage * fireDamageMultiplier);
            }

            return baseDamage;
        }

        public override void SetUP()
        {
            base.SetUP();
            PrintInfo();
            GetRemainEnergy();

            inventory = GetComponent<Inventory>();
            actionHistoryManager = GetComponent<ActionHistoryManager>();

            if (actionHistoryManager != null)
            {
                actionHistoryManager.SaveStateForUndo(new Vector2(positionX, positionY));
            }
        }

        public override void Move(Vector2 direction)
        {
            base.Move(direction);

            if (mapGenerator != null)
            {
                mapGenerator.MoveEnemies();
            }

            if (actionHistoryManager != null)
            {
                Vector2 newPosition = new Vector2(positionX, positionY);
                actionHistoryManager.SaveStateForUndo(newPosition);
            }
        }

        public void Attack(OOPEnemy enemy)
        {
            if (enemy == null) return;

            enemy.TakeDamage(GetDamage());
            Debug.Log($"{enemy.Name} energy is {enemy.energy}.");
        }

        protected override void CheckDead()
        {
            base.CheckDead();

            if (energy <= 0)
            {
                Debug.Log("Player is dead.");
            }
        }
    }
}