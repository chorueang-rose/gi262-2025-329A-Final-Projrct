using Searching;
using UnityEngine;

namespace Solution
{
    public class OOPExit : Identity
    {
        public Leaderboard leaderboard;

        [SerializeField]
        private int requiredKeys = 2;

        private bool isUnlocked;

        public void Unlock()
        {
            isUnlocked = GameManager.Instance != null &&
                         GameManager.Instance.KeyCount >= requiredKeys;
        }

        public override bool Hit()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("OOPExit requires a GameManager in the scene.");
                return false;
            }

            // อัปเดตสถานะปลดล็อกอีกครั้งเพื่อความถูกต้อง
            Unlock();

            if (!isUnlocked || GameManager.Instance.KeyCount < requiredKeys)
            {
                Debug.Log($"Need {requiredKeys} keys to open the exit.");
                return false;
            }

            if (!GameManager.Instance.ConsumeKeys(requiredKeys))
            {
                return false;
            }

            Debug.Log("You win!");

            int scoreReceived = CalculateScore();

            // ดึงชื่อผู้เล่นอย่างปลอดภัย
            string playerName = "Player";
            if (mapGenerator != null && mapGenerator.player != null && !string.IsNullOrEmpty(mapGenerator.player.Name))
            {
                playerName = mapGenerator.player.Name;
            }

            // บันทึกและแสดงผล Leaderboard
            if (leaderboard != null)
            {
                leaderboard.gameObject.SetActive(true);
                leaderboard.RecordScore(new PlayerScore(playerName, scoreReceived));
                leaderboard.PrintScores();
                leaderboard.ShowleaderBoard();
            }
            else
            {
                Debug.LogWarning("Leaderboard reference is missing on OOPExit!");
            }

            return true;
        }

        private int CalculateScore()
        {
            if (Time.time <= 0f)
            {
                return 0;
            }

            // ดึงค่า Energy จาก GameManager เป็นหลัก หากไม่มีให้ดึงจาก Player
            int currentEnergy = 0;
            if (GameManager.Instance != null)
            {
                currentEnergy = GameManager.Instance.Energy;
            }
            else if (mapGenerator != null && mapGenerator.player != null)
            {
                currentEnergy = mapGenerator.player.energy;
            }

            return Mathf.Max(
                0,
                (int)((currentEnergy * 100f) / Time.time));
        }
    }
}