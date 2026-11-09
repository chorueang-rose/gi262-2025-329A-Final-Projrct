using UnityEngine;

namespace Solution
{
    /// <summary>
    /// Stores shared player state such as energy and collected keys.
    /// Items update this class, and scene events can reduce its energy.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player State")]
        [SerializeField]
        private int energy = 100;

        [SerializeField]
        private int keyCount;

        public int Energy => energy;
        public int KeyCount => keyCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void AddKey()
        {
            keyCount++;
            Debug.Log("Key collected. Total keys: " + keyCount);
        }

        public bool ConsumeKeys(int amount)
        {
            if (amount < 0 || keyCount < amount)
            {
                return false;
            }

            keyCount -= amount;
            return true;
        }

        public void ReduceEnergyOnTimeExpired(int amount)
        {
            if (amount <= 0) return;

            energy = Mathf.Max(0, energy - amount);

            Debug.Log("Time expired. GameManager energy: " + energy);
        }

        public void AddEnergy(int amount)
        {
            if (amount <= 0) return;

            energy += amount;
            Debug.Log($"Energy added: {amount}. Total energy: {energy}");
        }

        public void SetEnergy(int value)
        {
            energy = Mathf.Max(0, value);
        }
    }
}