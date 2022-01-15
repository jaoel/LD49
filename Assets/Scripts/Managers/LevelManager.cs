using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class LevelManager : MonoBehaviour {
        private static LevelManager _instance;

        [SerializeField]
        private LevelHolder levelHolder = null;

        private Level currentLevel = null;
        private int currentLevelIndex = -1;

        private void Awake() {
            _instance = this;
        }

        private void Start() {
            currentLevel = FindObjectOfType<Level>();
            MusicManager.PlayGameMusic();
        }

        private bool TryGetLevelPrefab(int levelID, out Level levelPrefab) {
            if (levelID < 0 || levelID >= levelHolder.levelPrefabs.Count) {
                Debug.LogError($"There is no level with index {levelID}");
                levelPrefab = null;
                return false;
            }

            levelPrefab = levelHolder.levelPrefabs[levelID];
            if (levelPrefab == null) {
                Debug.LogError($"Level with index {levelID} is null");
                return false;
            }

            return true;
        }

        public static void LoadLevel(int levelID) {
            if (_instance && _instance.TryGetLevelPrefab(levelID, out Level levelPrefab)) {
                _instance.LoadLevel(levelPrefab, levelID);
            }
        }

        public static void LoadNextLevel(Action onComplete = null) {
            if (_instance != null) {
                int nextLevelIndex = _instance.currentLevelIndex + 1;
                if (nextLevelIndex >= _instance.levelHolder.levelPrefabs.Count) {
                    GameManager.LoadEnd();
                } else if (_instance.TryGetLevelPrefab(nextLevelIndex, out Level levelPrefab)) {
                    GameManager.playerInputAllowed = false;
                    OverlayManager.QueueFadeTransition(
                        () => _instance.LoadLevel(levelPrefab, nextLevelIndex),
                        () => {
                            onComplete?.Invoke();
                            GameManager.playerInputAllowed = true;
                        });
                }
            }
        }

        public static void ReloadCurrentLevel(Action onComplete = null) {
            if (_instance != null) {
                GameManager.playerInputAllowed = false;
                OverlayManager.QueueFadeTransition(
                    () => LoadLevel(_instance.currentLevelIndex),
                    () => {
                        onComplete?.Invoke();
                        GameManager.playerInputAllowed = true;
                    });
            }
        }

        private void LoadLevel(Level levelPrefab, int levelID) {
            if (currentLevel != null) {
                DOTween.KillAll();
                Destroy(currentLevel.gameObject);
            }

            Debug.Log($"Loading '{levelPrefab.name}' with id {levelID}");
            currentLevel = Instantiate(levelPrefab);
            currentLevelIndex = levelID;
            GameManager.ResetChaos();
        }

        public static void RecordCurrentLevelWin() {
            if (_instance != null && _instance.currentLevel != null) {
                PlayerPrefs.SetInt($"level{_instance.currentLevelIndex}", 1);
            }
        }
    }
}
