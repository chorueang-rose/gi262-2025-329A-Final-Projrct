using UnityEngine;
using System.Collections.Generic;

public class PotionSpawner : MonoBehaviour
{
    [Header("Potion Prefabs")]
    public GameObject bluePotionPrefab;
    public GameObject greenPotionPrefab;
    public GameObject redPotionPrefab;


    [Header("Spawn Settings")]
    public int potionCount = 5;
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
        if (potionPrefabs == null || potionPrefabs.Length == 0)
            return;

        if (spawnArea == null)
            return;

        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < potionCount; i++)
        {
            GameObject randomPotion = potionPrefabs[
                Random.Range(0, potionPrefabs.Length)
            ];

            Vector2 randomPosition = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            GameObject potion = Instantiate(
                randomPotion,
                randomPosition,
                Quaternion.identity
            );

            spawnedPotions.Add(potion);
        }
    }
}