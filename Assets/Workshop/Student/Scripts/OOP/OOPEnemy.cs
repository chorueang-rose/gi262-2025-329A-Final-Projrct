using TMPro;
using UnityEngine;

namespace Solution
{
    public class OOPEnemy : Character
    {
        public TMP_Text txtHp;

        public override void SetUP()
        {
            base.SetUP();
            PrintInfo();
            GetRemainEnergy();

            if (txtHp != null)
            {
                txtHp.text = energy.ToString();
            }
        }

        public override bool Hit()
        {
            if (mapGenerator != null && mapGenerator.player != null)
            {
                // ผู้เล่นโจมตีศัตรู
                mapGenerator.player.Attack(this);

                // เช็คว่าถ้าศัตรูยังไม่ตาย ค่อยโจมตีผู้เล่นสวนกลับ
                if (isAlive && energy > 0)
                {
                    Attack(mapGenerator.player);
                }
            }

            return false;
        }

        public void Attack(OOPPlayer _player)
        {
            if (_player != null)
            {
                _player.TakeDamage(AttackPoint);
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            if (txtHp != null)
            {
                txtHp.text = energy.ToString();
            }
        }

        protected override void CheckDead()
        {
            if (energy <= 0)
            {
                isAlive = false;

                if (mapGenerator != null)
                {
                    if (mapGenerator.mapdata != null &&
                        positionX >= 0 && positionX < mapGenerator.X &&
                        positionY >= 0 && positionY < mapGenerator.Y)
                    {
                        mapGenerator.mapdata[positionX, positionY] = null;
                    }

                    if (mapGenerator.EnemysOnMap != null)
                    {
                        mapGenerator.EnemysOnMap.Remove(this);
                    }
                }

                Destroy(gameObject);
            }
        }

        public void RandomMove()
        {
            if (!isAlive) return;

            int toX = positionX;
            int toY = positionY;
            int random = Random.Range(0, 4);

            switch (random)
            {
                case 0:
                    // Up
                    toY += 1;
                    break;
                case 1:
                    // Down
                    toY -= 1;
                    break;
                case 2:
                    // Left
                    toX -= 1;
                    break;
                case 3:
                    // Right
                    toX += 1;
                    break;
            }

            if (!HasPlacement(toX, toY))
            {
                UpdatePosition(toX, toY);
            }
        }
    }
}