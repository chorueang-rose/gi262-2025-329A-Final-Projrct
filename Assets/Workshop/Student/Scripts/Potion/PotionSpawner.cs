using UnityEngine;
using System.Collections.Generic;

public class PotionSpawner : MonoBehaviour
{
    [Header("Potion Prefabs")]
    public GameObject bluePotionPrefab;
    public GameObject greenPotionPrefab;
    public GameObject redPotionPrefab;


    [Header("Spawn Settings")]
    public int blueCount = 5;
    public int greenCount = 2;
    public int redCount = 5;
    public BoxCollider2D spawnArea;

    private List<GameObject> spawnedPotions =
        new List<GameObject>();

    void Start()
    {
        SpawnPotions();
    }

    void Update()
    {
        spawnedPotions.RemoveAll(p => p == null);

        if (spawnedPotions.Count == 0)
        {
            SpawnPotions();
        }
    }

    void SpawnPotions()
    {
        if (spawnArea == null)
            return;

        SpawnPotion(bluePotionPrefab, blueCount);
        SpawnPotion(greenPotionPrefab, greenCount);
        SpawnPotion(redPotionPrefab, redCount);
    }

    void SpawnPotion(GameObject prefab, int count)
    {
        if (prefab == null)
            return;

        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            GameObject potion = Instantiate(
                prefab,
                randomPosition,
                Quaternion.identity
            );

            spawnedPotions.Add(potion);
        }
    }
}