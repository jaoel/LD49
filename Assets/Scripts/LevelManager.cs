using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class LevelManager : MonoBehaviour {
        private static int requestedLevel = 0;
        public static void RequestLevel(int level) {
            requestedLevel = level;
        }

        private static LevelManager _instance;
        public static LevelManager Instance {
            get {
                if (_instance == null) {
                    Debug.LogError("A LevelManager is required");
                }
                return _instance;
            }
        }

        [SerializeField]
        private LevelHolder levelHolder;

        private Level currentLevel = null;
        private int currentLevelIndex = -1;

        private void Awake() {
            _instance = this;
        }

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

        public void RecordCurrentLevelWin() {
            if (currentLevel != null) {
                PlayerPrefs.SetInt($"level{currentLevelIndex}", 1);
            }
        }

        public void RequestNextLevel() {
            requestedLevel = currentLevelIndex + 1;
        }

        private void LoadLevel(int index) {
            if (index < 0 || index >= levelHolder.levelPrefabs.Count) {
                Debug.LogError($"There is no level with index {index}");
                return;
            }

            Level levelPrefab = levelHolder.levelPrefabs[index];
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
