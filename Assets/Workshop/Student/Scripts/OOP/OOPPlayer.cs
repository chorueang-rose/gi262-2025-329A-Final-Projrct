using UnityEngine;
using TMPro;

namespace Solution
{
    public class OOPPlayer : Identity
    {
        [Header("Character Attributes")]
        public int energy = 20;
        public int maxEnergy = 20;
        public int attackPoint = 10;
        public TMP_Text damageText;
        public bool isAlive = true;

        // แก้ CS1061: ใช้ isAutoMoving (ตัวเล็ก) และ IsAutoMoving (ตัวใหญ่) เพื่อรองรับทุกสคริปต์
        public bool isAutoMoving = false;
        public bool IsAutoMoving
        {
            get => isAutoMoving;
            set => isAutoMoving = value;
        }

        public bool redPotionActive = false;
        public bool greenPotionActive = false;

        [Header("Potion Stack")]
        public int greenMoveStacks = 0;
        public TMP_Text greenStackText;

        public void ActivateRedPotion()
        {
            redPotionActive = true;

            UpdateAttackUI();

            Debug.Log("Red Potion Activated! Damage: " + (attackPoint * 2));
        }

        public void ActivateGreenPotion()
        {
            greenMoveStacks++;
            greenPotionActive = true;

            UpdateGreenStackUI();

            Debug.Log("Green Potion Stack: " + greenMoveStacks);
        }

        private void UpdateGreenStackUI()
        {
            if (greenStackText != null)
            {
                greenStackText.text = "Double Move: " + greenMoveStacks;
            }
        }

        private void UpdateAttackUI()
        {
            if (damageText != null)
            {
                int currentDamage = redPotionActive
                    ? attackPoint * 2
                    : attackPoint;

                damageText.text = redPotionActive
                    ? $"Damage: {attackPoint} → {currentDamage} (RED)"
                    : $"Damage: {attackPoint}";
            }
        }

        [Header("Components")]
        public Inventory inventory;
        public ActionHistoryManager actionHistoryManager;

        // แก้ CS0120: ทำ ActivePlayer เป็น static method ตามที่ ElementalPotion เรียกใช้
        public static void ActivePlayer()
        {
            Debug.Log("Active Player called");
        }

        private void Start()
        {
            if (inventory == null)
                inventory = GetComponent<Inventory>();

            if (actionHistoryManager == null)
                actionHistoryManager = GetComponent<ActionHistoryManager>();

            UpdateGreenStackUI();
        }

        // --- ฟังก์ชัน Heal รองรับ 1 และ 2 arguments ---
        public void Heal(int amount)
        {
            energy = Mathf.Min(energy + amount, maxEnergy);
            Debug.Log($"Player healed by {amount}. Current energy: {energy}");
        }

        // แก้ CS1501: เพิ่ม Overload รองรับ 2 arguments สำหรับ OOPItemPotion
        public void Heal(int amount, Identity target) { Heal(amount); }
        public void Heal(int amount, int extra) { Heal(amount); }

        // --- ฟังก์ชันรับดาเมจ และโจมตี ---
        public void TakeDamage(int damage)
        {
            energy -= damage;
            if (energy <= 0) isAlive = false;
        }

        public void TakeDamage(int damage, Identity source) { TakeDamage(damage); }
        public void TakeDamage(int damage, ElementType type) { TakeDamage(damage); }
        public void TakeDamage(int damage, int extra) { TakeDamage(damage); }

        public void Attack() { }
        public void Attack(int damage) { }
        public void Attack(Identity target) { }

        public void SetElement(ElementType element)
        {
            Debug.Log("Set element to: " + element);
        }

        public void UpdatePosition(int x, int y)
        {
            positionX = x;
            positionY = y;
            transform.position = new Vector3(x, y, -0.1f);
        }

        public void updatePosition(int x, int y) { UpdatePosition(x, y); }

        // --- ระบบการเดินและการจัดการไอเทม/กุญแจ ---

        public void Move(int deltaX, int deltaY)
        {
            HealthSystem health = GetComponent<HealthSystem>();

            if (!isAlive || (health != null && health.currentHealth <= 0))
            {
                isAlive = false;
                return;
            }

            int steps = greenMoveStacks > 0 ? 2 : 1;
            bool movedAny = false;

            for (int i = 0; i < steps; i++)
            {
                if (!MoveOneStep(deltaX, deltaY))
                    break;

                movedAny = true;

                if (health != null && health.currentHealth <= 0)
                {
                    isAlive = false;
                    break;
                }
            }

            if (movedAny && greenMoveStacks > 0)
            {
                greenMoveStacks--;
                greenPotionActive = greenMoveStacks > 0;
                UpdateGreenStackUI();
            }
        }

        private bool MoveOneStep(int deltaX, int deltaY)
        {
            int targetX = positionX + deltaX;
            int targetY = positionY + deltaY;

            Identity targetObj = mapGenerator.GetMapData(targetX, targetY);

            if (targetObj != null)
            {
                // 1. เจอกุญแจ
                KeyItem keyItem = targetObj.GetComponent<KeyItem>();
                bool isKeyByName = targetObj.Name != null &&
                                   targetObj.Name.ToLower().Contains("key");

                if (keyItem != null || isKeyByName)
                {
                    OOPExit exitObj =
                        UnityEngine.Object.FindFirstObjectByType<OOPExit>();

                    if (exitObj != null)
                        exitObj.Unlock();

                    mapGenerator.mapdata[targetX, targetY] = null;
                    Destroy(targetObj.gameObject);

                    MoveTo(targetX, targetY, deltaX, deltaY);
                    return true;
                }

                // 2. เจอประตู
                OOPExit exit = targetObj.GetComponent<OOPExit>();
                if (exit != null)
                {
                    exit.Hit();
                    return false;
                }

                // 3. เจอไอเทม
                CollectAbleItem item = targetObj.GetComponent<CollectAbleItem>();
                if (item != null)
                {
                    item.Hit();
                    MoveTo(targetX, targetY, deltaX, deltaY);
                    return true;
                }

                // 4. เจอศัตรู
                OOPEnemy enemy = targetObj.GetComponent<OOPEnemy>();
                if (enemy != null)
                {
                    int damage = redPotionActive
                        ? attackPoint * 2
                        : attackPoint;

                    enemy.TakeDamage(damage);

                    redPotionActive = false;

                    UpdateAttackUI();

                    Debug.Log("Player Damage: " + damage);

                    return false;
                }

                return false;
            }

            MoveTo(targetX, targetY, deltaX, deltaY);
            return true;
        }

        // รองรับการเรียก MoveTo แบบ 2 พารามิเตอร์ และ 4 พารามิเตอร์
        private void MoveTo(int newX, int newY)
        {
            MoveTo(newX, newY, newX - positionX, newY - positionY);
        }

        private void MoveTo(int newX, int newY, int deltaX, int deltaY)
        {
            mapGenerator.mapdata[positionX, positionY] = null;
            positionX = newX;
            positionY = newY;
            mapGenerator.mapdata[positionX, positionY] = this;
            transform.position = new Vector3(positionX, positionY, -0.1f);


            HealthSystem health = GetComponent<HealthSystem>();

            if (health != null)
            {
                health.TakeDamage(1);

                if (health.currentHealth <= 0)
                {
                    isAlive = false;
                }
            }


            if (actionHistoryManager != null)
            {
                actionHistoryManager.RecordMove(deltaX, deltaY);
            }

            mapGenerator.MoveEnemies();
        }

        private void Update()
        {
            if (!isAlive || isAutoMoving)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move(0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Move(0, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(-1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(1, 0);
            }

            
        }
    }
}