using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Solution
{
    public class ZombieParade : OOPEnemy
    {
        // ใช้ LinkedList ในการจัดการส่วนของงูเพื่อประสิทธิภาพในการเพิ่ม/ลบ
        private LinkedList<GameObject> Parade = new LinkedList<GameObject>();
        public int SizeParade = 3;
        private int timer = 0;
        public GameObject[] bodyPrefab; // Prefab ของส่วนลำตัวงู
        public float moveInterval = 0.5f; // ช่วงเวลาในการเคลื่อนที่ (0.5 วินาที)

        private Vector3 moveDirection;

        public override void SetUP()
        {
            base.SetUP();
            moveDirection = Vector3.up;

            // ตั้งค่าตำแหน่งเริ่มต้นให้ถูกต้องทั้ง X และ Y
            positionX = (int)transform.position.x;
            positionY = (int)transform.position.y;

            // เริ่ม Coroutine สำหรับการเคลื่อนที่
            StartCoroutine(MoveParade());
        }

        private Vector3 RandomizeDirection()
        {
            List<Vector3> possibleDirections = new List<Vector3>
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right
            };

            return possibleDirections[Random.Range(0, possibleDirections.Count)];
        }

        // Coroutine สำหรับการเคลื่อนที่ทีละช่อง
        IEnumerator MoveParade()
        {
            // 0. ใส่ส่วนหัวงูเข้า LinkedList
            Parade.AddFirst(this.gameObject);

            while (isAlive)
            {
                // 1. ดึงส่วนหัวงู
                GameObject firstPart = Parade.First.Value;

                int toX = positionX;
                int toY = positionY;
                bool isCollide = true;
                int countTryToFind = 0;

                // สุ่มหาทิศทางเดินที่ไม่ชน (ลองสูงสุด 10 ครั้ง)
                while (isCollide && countTryToFind < 10)
                {
                    moveDirection = RandomizeDirection();
                    toX = (int)(firstPart.transform.position.x + moveDirection.x);
                    toY = (int)(firstPart.transform.position.y + moveDirection.y);
                    countTryToFind++;

                    isCollide = IsCollision(toX, toY);
                }

                // ถ้าสุ่มครบ 10 ครั้งแล้วยังชนอยู่ ให้เดินช่องเดิม
                if (isCollide)
                {
                    toX = positionX;
                    toY = positionY;
                }

                // 2. เคลื่อนย้ายส่วนหางสุดมาไว้ที่หัว หรือเคลื่อนย้ายตำแหน่ง
                if (Parade.Count > 1)
                {
                    // ถ้างูมีลำตัว ให้ลบหางออกแล้วย้ายมาเป็นส่วนหน้า
                    GameObject lastPart = Parade.Last.Value;
                    Parade.RemoveLast();

                    // เคลียร์ตำแหน่งเก่าบน mapData
                    if (mapGenerator != null && mapGenerator.mapdata != null)
                    {
                        mapGenerator.mapdata[positionX, positionY] = null;
                    }

                    positionX = toX;
                    positionY = toY;

                    lastPart.transform.position = new Vector3(positionX, positionY, 0);

                    SpriteRenderer sr = lastPart.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.flipX = (moveDirection == Vector3.right);
                    }

                    // อัปเดตตำแหน่งใหม่บน mapData
                    Identity identity = lastPart.GetComponent<Identity>();
                    if (identity != null && mapGenerator != null)
                    {
                        mapGenerator.mapdata[positionX, positionY] = identity;
                    }

                    Parade.AddFirst(lastPart);
                }
                else
                {
                    // ถ้างูมีแค่หัวตัวเดียว เคลื่อนที่หัวตรงๆ
                    if (mapGenerator != null && mapGenerator.mapdata != null)
                    {
                        mapGenerator.mapdata[positionX, positionY] = null;
                    }

                    positionX = toX;
                    positionY = toY;

                    transform.position = new Vector3(positionX, positionY, 0);

                    SpriteRenderer sr = GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.flipX = (moveDirection == Vector3.right);
                    }

                    if (mapGenerator != null)
                    {
                        mapGenerator.mapdata[positionX, positionY] = this;
                    }
                }

                // 3. เพิ่มความยาวลำตัวงูตามเงื่อนไขเวลา
                if (Parade.Count < SizeParade)
                {
                    timer++;
                    if (timer > 3)
                    {
                        Grow();
                        timer = 0;
                    }
                }

                yield return new WaitForSeconds(moveInterval);
            }
        }

        private bool IsCollision(int x, int y)
        {
            // ตรวจสอบสิ่งกีดขวาง
            return HasPlacement(x, y);
        }

        // ฟังก์ชันสำหรับเพิ่มส่วนของงู (Grow)
        private void Grow()
        {
            if (bodyPrefab == null || bodyPrefab.Length == 0) return;

            GameObject newPart = Instantiate(bodyPrefab[0]);
            GameObject lastPart = Parade.Last.Value;

            newPart.transform.position = lastPart.transform.position;

            if (mapGenerator != null)
            {
                mapGenerator.SetUpItem(positionX, positionY, newPart, mapGenerator.enemyParent, mapGenerator.enemy);
            }

            // เพิ่มส่วนใหม่เข้าไปใน Linked List ต่อท้ายสุด
            Parade.AddLast(newPart);
        }
    }
}