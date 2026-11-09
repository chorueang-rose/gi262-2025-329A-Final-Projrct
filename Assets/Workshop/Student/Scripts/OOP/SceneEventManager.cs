using UnityEngine;

namespace Solution
{
    public class SceneEventManager : MonoBehaviour
    {
        public static SceneEventManager Instance { get; private set; }

        [Header("Timer Settings")]
        public float timeRemaining = 60f;
        public bool isTimerActive = false;

        [Header("Penalty Settings")]
        public int energyPenalty = 20;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartTimer();
        }

        private void Update()
        {
            if (isTimerActive)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    timeRemaining = 0;
                    OnTimeExpired();
                }
            }
        }

        public void StartTimer()
        {
            timeRemaining = 60f;
            isTimerActive = true;
        }

        public void OnTimeExpired()
        {
            isTimerActive = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReduceEnergyOnTimeExpired(energyPenalty);
                Debug.Log($"Time expired! Energy reduced by {energyPenalty}.");
            }
            else
            {
                Debug.LogWarning("Time expired, but GameManager.Instance was not found.");
            }
        }
    }
}