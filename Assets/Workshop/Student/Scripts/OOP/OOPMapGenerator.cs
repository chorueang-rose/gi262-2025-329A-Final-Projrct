using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solution
{
    public class OOPMapGenerator : MonoBehaviour
    {
        [Header("Set MapGenerator")]
        public int X = 10;
        public int Y = 10;

        [Header("Set Player")]
        public OOPPlayer player;
        public Vector2Int playerStartPos;

        [Header("Set NPC")]
        public NPC Npc;
        public NPCSkill NpcSkill;

        [Header("Set Exit")]
        public OOPExit Exit;

        [Header("Set Wall")]
        public Identity Wall;

        [Header("Set Prefab")]
        public GameObject[] floorsPrefab;
        public GameObject[] wallsPrefab;
        public GameObject[] demonWallsPrefab;
        public GameObject[] itemsPrefab;
        public GameObject[] collectItemsPrefab;
        public GameObject[] EnemyPrefab;
        public GameObject[] SkillPrefab;

        [Header("Set Key Item Prefab")]
        public GameObject keyItemPrefab; // ช่องสำหรับลาก Prefab กุญแจมาใส่ใน Inspector

        [Header("Set Transform")]
        public Transform floorParent;
        public Transform wallParent;
        public Transform itemParent;
        public Transform enemyParent;

        [Header("Set Object Count")]
        public int obstacleCount;
        public int itemPotionCount;
        public int collectItemCount;
        public int EnemyCount = 3;
        public int SkillCount;

        public Identity[,] mapdata;

        [Header("Enemy on Map")]
        public List<OOPEnemy> EnemysOnMap = new List<OOPEnemy>();

        // Block types ...
        [HideInInspector] public string empty = "";
        [HideInInspector] public string demonWall = "demonWall";
        [HideInInspector] public string potion = "potion";
        [HideInInspector] public string bonuesPotion = "bonuesPotion";
        [HideInInspector] public string exit = "exit";
        [HideInInspector] public string playerOnMap = "player";
        [HideInInspector] public string collectItem = "collectItem";
        [HideInInspector] public string enemy = "enemy";
        [HideInInspector] public string npc = "Npc";

        private void Awake()
        {
            CreateMap();
        }

        private void Start()
        {
            StartCoroutine(SetUPMap());
        }

        private IEnumerator SetUPMap()
        {
            SetUpPlayer();
            SetUpExit();
            SetUpNpc();

            PlaceItemsOnMap(obstacleCount, demonWallsPrefab, wallParent, demonWall);
            PlaceItemsOnMap(collectItemCount, collectItemsPrefab, itemParent, collectItem);
            PlaceItemsOnMap(SkillCount, SkillPrefab, itemParent, collectItem);

            // 🔑 สร้างกุญแจ 1 ดอกสุ่มลงบนแผนที่
            if (keyItemPrefab != null)
            {
                PlaceItemsOnMap(1, new GameObject[] { keyItemPrefab }, itemParent, collectItem);
            }

            // สเปอว์นศัตรูลงบนแผนที่
            PlaceItemsOnMap(EnemyCount, EnemyPrefab, enemyParent, enemy);

            yield return new WaitForSeconds(0.5f);
            RandomDamageToListEnemies();
        }

        private void PlaceItemsOnMap(int count, GameObject[] prefab, Transform parent, string itemType, System.Action onComplete = null)
        {
            if (prefab == null || prefab.Length == 0) return;

            int placedCount = 0;
            int preventInfiniteLoop = 1000;

            while (placedCount < count)
            {
                if (--preventInfiniteLoop < 0)
                {
                    Debug.LogWarning($"Could not place all '{itemType}'. Map may be too full.");
                    break;
                }

                int x = UnityEngine.Random.Range(0, X);
                int y = UnityEngine.Random.Range(0, Y);

                if (mapdata[x, y] == null)
                {
                    SetUpItem(x, y, prefab, parent, itemType);
                    placedCount++;
                }
            }
            onComplete?.Invoke();
        }

        private void SetUpNpc()
        {
            if (Npc != null)
            {
                mapdata[Npc.positionX, Npc.positionY] = Npc;
                Npc.transform.position = new Vector3(Npc.positionX, Npc.positionY, 0);
                Npc.mapGenerator = this;
            }

            if (NpcSkill != null)
            {
                mapdata[NpcSkill.positionX, NpcSkill.positionY] = NpcSkill;
                NpcSkill.transform.position = new Vector3(NpcSkill.positionX, NpcSkill.positionY, 0);
                NpcSkill.mapGenerator = this;
            }
        }

        private void SetUpExit()
        {
            if (Exit != null)
            {
                mapdata[X - 1, Y - 1] = Exit;
                Exit.transform.position = new Vector3(X - 1, Y - 1, 0);
                Exit.mapGenerator = this;
            }
        }

        private void SetUpPlayer()
        {
            if (player != null)
            {
                player.mapGenerator = this;
                player.positionX = playerStartPos.x;
                player.positionY = playerStartPos.y;
                player.transform.position = new Vector3(playerStartPos.x, playerStartPos.y, -0.1f);
                mapdata[playerStartPos.x, playerStartPos.y] = player;
            }
        }

        private void CreateMap()
        {
            mapdata = new Identity[X, Y];

            for (int x = -1; x < X + 1; x++)
            {
                for (int y = -1; y < Y + 1; y++)
                {
                    if (x == -1 || x == X || y == -1 || y == Y)
                    {
                        if (wallsPrefab != null && wallsPrefab.Length > 0)
                        {
                            int r = Random.Range(0, wallsPrefab.Length);
                            GameObject obj = Instantiate(wallsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
                            obj.transform.parent = wallParent;
                            obj.name = $"Wall_{x}, {y}";
                        }
                    }
                    else
                    {
                        if (floorsPrefab != null && floorsPrefab.Length > 0)
                        {
                            int r = Random.Range(0, floorsPrefab.Length);
                            GameObject obj = Instantiate(floorsPrefab[r], new Vector3(x, y, 1), Quaternion.identity);
                            obj.transform.parent = floorParent;
                            obj.name = $"floor_{x}, {y}";
                        }
                        mapdata[x, y] = null;
                    }
                }
            }
        }

        public Identity GetMapData(float x, float y)
        {
            if (x >= X || x < 0 || y >= Y || y < 0)
            {
                return Wall;
            }

            return mapdata[(int)x, (int)y];
        }

        public void SetUpItem(int x, int y, GameObject[] _itemsPrefab, Transform parrent, string _name)
        {
            if (_itemsPrefab == null || _itemsPrefab.Length == 0) return;

            int r = Random.Range(0, _itemsPrefab.Length);
            GameObject obj = Instantiate(_itemsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            SetUpItem(x, y, obj, parrent, _name);
        }

        public void SetUpItem(int x, int y, GameObject _itemsPrefab, Transform parrent, string _name)
        {
            if (_itemsPrefab == null) return;

            _itemsPrefab.transform.parent = parrent;

            Identity identity = _itemsPrefab.GetComponent<Identity>();
            if (identity != null)
            {
                mapdata[x, y] = identity;
                identity.positionX = x;
                identity.positionY = y;
                identity.mapGenerator = this;

                if (_name != collectItem)
                {
                    identity.Name = _name;
                }

                if (_name == enemy)
                {
                    OOPEnemy enemyComponent = _itemsPrefab.GetComponent<OOPEnemy>();
                    if (enemyComponent != null)
                    {
                        EnemysOnMap.Add(enemyComponent);
                    }
                }

                _itemsPrefab.name = $"Object_{identity.Name} {x}, {y}";
            }
        }

        public OOPEnemy[] GetEnemies()
        {
            EnemysOnMap.RemoveAll(e => e == null);
            return EnemysOnMap.ToArray();
        }

        public void MoveEnemies()
        {
            EnemysOnMap.RemoveAll(e => e == null);

            foreach (var enemyObj in EnemysOnMap)
            {
                if (enemyObj != null)
                {
                    enemyObj.RandomMove();
                }
            }
        }

        public void RandomDamageToListEnemies()
        {
            EnemysOnMap.RemoveAll(e => e == null);
            Debug.Log($"Damage to {EnemysOnMap.Count} EnemysOnMap");

            foreach (var enemyObj in EnemysOnMap)
            {
                if (enemyObj != null)
                {
                    int damage = Random.Range(1, 15);
                    enemyObj.TakeDamage(damage);
                }
            }
        }
    }
}