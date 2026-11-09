using UnityEngine;

namespace Solution
{
    public class Character : Identity
    {
        [Header("Character Attributes")]
        public int energy = 100;
        public int maxEnergy = 100;
        public int AttackPoint = 10;

        public bool isAlive = true;
        protected bool isFreeze;

        [Header("Visual Feedback")]
        public SpriteRenderer spriteRenderer;

        [Header("Color Energy Levels")]
        public Color normalColor = Color.white;    // HP > 66%
        public Color damagedColor1 = Color.yellow; // HP 34% - 66%
        public Color damagedColor2 = Color.red;    // HP <= 33%
        public Color freezeColor = Color.blue;     // สถานะแช่แข็ง

        public override void SetUP()
        {
            base.SetUP();
            isAlive = true;
            isFreeze = false;
            Debug.Log("SetUP " + Name);

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (maxEnergy <= 0) maxEnergy = 100;
            energy = maxEnergy;

            UpdateSpriteColorBasedOnHealth();
        }

        public void GetRemainEnergy()
        {
            Debug.Log(Name + " remaining energy: " + energy);
        }

        public virtual void Move(Vector2 direction)
        {
            if (!isAlive) return;

            if (isFreeze)
            {
                isFreeze = false;
                UpdateSpriteColorBasedOnHealth();
                Debug.Log($"{Name} unfroze and skipped turn.");
                return;
            }

            int toX = (int)(positionX + direction.x);
            int toY = (int)(positionY + direction.y);

            if (HasPlacement(toX, toY))
            {
                Identity targetObject = mapGenerator.GetMapData(toX, toY);
                if (targetObject != null && targetObject.Hit())
                {
                    UpdatePosition(toX, toY);
                }
            }
            else
            {
                UpdatePosition(toX, toY);
                TakeDamage(1);
            }
        }

        public virtual void UpdatePosition(int toX, int toY)
        {
            if (mapGenerator != null && mapGenerator.mapdata != null)
            {
                if (positionX >= 0 && positionX < mapGenerator.X && positionY >= 0 && positionY < mapGenerator.Y)
                {
                    mapGenerator.mapdata[positionX, positionY] = null;
                }

                positionX = toX;
                positionY = toY;

                if (positionX >= 0 && positionX < mapGenerator.X && positionY >= 0 && positionY < mapGenerator.Y)
                {
                    mapGenerator.mapdata[positionX, positionY] = this;
                }
            }

            transform.position = new Vector3(positionX, positionY, transform.position.z);
        }

        public bool HasPlacement(int x, int y)
        {
            if (mapGenerator == null) return false;
            var mapData = mapGenerator.GetMapData(x, y);
            return mapData != null;
        }

        public virtual void TakeDamage(int damage)
        {
            energy = Mathf.Max(0, energy - damage);
            Debug.Log(Name + " Current Energy : " + energy);
            UpdateSpriteColorBasedOnHealth();
            CheckDead();
        }

        public virtual void TakeDamage(int damage, bool freeze)
        {
            energy = Mathf.Max(0, energy - damage);
            isFreeze = freeze;
            Debug.Log(Name + " Current Energy : " + energy + " | Freeze: " + freeze);
            UpdateSpriteColorBasedOnHealth();
            CheckDead();
        }

        public void Heal(int healPoint)
        {
            Heal(healPoint, false);
        }

        public void Heal(int healPoint, bool bonus)
        {
            energy += healPoint * (bonus ? 2 : 1);
            if (energy > maxEnergy)
            {
                energy = maxEnergy;
            }
            Debug.Log("Current Energy : " + energy);
            UpdateSpriteColorBasedOnHealth();
        }

        protected virtual void CheckDead()
        {
            if (energy <= 0)
            {
                isAlive = false;

                if (mapGenerator != null && mapGenerator.mapdata != null)
                {
                    if (positionX >= 0 && positionX < mapGenerator.X && positionY >= 0 && positionY < mapGenerator.Y)
                    {
                        mapGenerator.mapdata[positionX, positionY] = null;
                    }
                }

                Destroy(gameObject);
            }
        }

        private void UpdateSpriteColorBasedOnHealth()
        {
            if (spriteRenderer == null) return;

            if (isFreeze)
            {
                spriteRenderer.color = freezeColor;
                return;
            }

            if (maxEnergy <= 0) maxEnergy = 100;

            float healthPercentage = (float)energy / maxEnergy;

            if (healthPercentage > 0.66f)
            {
                spriteRenderer.color = normalColor;
            }
            else if (healthPercentage > 0.33f)
            {
                spriteRenderer.color = damagedColor1;
            }
            else
            {
                spriteRenderer.color = damagedColor2;
            }

            Debug.Log($"{Name} Health Percentage: {healthPercentage * 100:F1}%");
        }
    }
}