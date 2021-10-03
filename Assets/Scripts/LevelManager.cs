using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class LevelManager : MonoBehaviour {
        private static int requestedLevel = 0;
        public static void RequestLevel(int level) {
            requestedLevel = level;
        }

        [SerializeField] 
        private List<Level> levelPrefabs = new List<Level>();

        private Level currentLevel = null;
        private int currentLevelIndex = -1;

        private void Start() {
            currentLevel = FindObjectOfType<Level>();
            if (currentLevel != null) {
                requestedLevel = -1;
            }
            GameManager.MusicManager.PlayGameMusic();
        }

        private void Update() {
            if (requestedLevel != -1 && requestedLevel != currentLevelIndex) {
                LoadLevel(requestedLevel);
                requestedLevel = -1;
            }
        }

        private void LoadLevel(int index) {
            if (index < 0 || index >= levelPrefabs.Count) {
                Debug.LogError($"There is no level with index {index}");
                return;
            }

            Level levelPrefab = levelPrefabs[index];
            if (levelPrefab == null) {
                Debug.LogError($"Level with index {index} is null");
                return;
            }

            if (currentLevel != null) {
                Destroy(currentLevel);
            }

            Debug.Log($"Loading '{levelPrefab.name}' at index {index}");
            currentLevelIndex = index;
            currentLevel = Instantiate(levelPrefab);
        }
    }
}
