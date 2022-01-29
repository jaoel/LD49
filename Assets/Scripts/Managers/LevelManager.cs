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

        private GameScene currentLevelScene = null;
        private int currentLevelSceneIndex = -1;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                // We delete 'gameObject' here, and not 'this', because GameManager should be the root object
                // and container for multiple other managers
                Destroy(gameObject);
                Debug.LogWarning("A duplicate LevelManager was found and destroyed");
            }
        }

        private void Start() {
            currentLevelScene = levelHolder.GetLoadedLevelScene(out currentLevelSceneIndex);
            MusicManager.PlayGameMusic();
        }

        public static bool LoadLevel(string levelScenePath) {
            if (_instance && _instance.levelHolder.TryGetLevel(levelScenePath, out GameScene levelScene, out int levelSceneIndex)) {
                return _instance.TransitionToLevel(levelScene, levelSceneIndex);
            }
            return false;
        }

        public static bool LoadLevel(int levelSceneIndex) {
            if (_instance && _instance.levelHolder.TryGetLevel(levelSceneIndex, out GameScene levelScene)) {
                return _instance.TransitionToLevel(levelScene, levelSceneIndex);
            }
            return false;
        }

        public static void LoadNextLevel(Action onComplete = null) {
            if (_instance != null) {
                int nextLevelIndex = _instance.currentLevelSceneIndex + 1;
                if (nextLevelIndex >= _instance.levelHolder.levels.Count) {
                    GameManager.TransitionToScene(Scenes.End);
                } else if (_instance.levelHolder.TryGetLevel(nextLevelIndex, out GameScene nextLevelScene)) {
                    _instance.TransitionToLevel(nextLevelScene, nextLevelIndex, onComplete);
                }
            }
        }

        public static void ReloadCurrentLevel(Action onComplete = null) {
            if (_instance != null && _instance.currentLevelScene != null) {
                _instance.TransitionToLevel(_instance.currentLevelScene, _instance.currentLevelSceneIndex, onComplete);
            }
        }

        private bool TransitionToLevel(GameScene levelScene, int levelSceneIndex, Action onComplete = null) {
            if (levelScene != null) {
                GameManager.playerInputAllowed = false;
                OverlayManager.QueueFadeTransition(
                    () => LoadLevelImmediate(levelScene, levelSceneIndex),
                    () => {
                        onComplete?.Invoke();
                        GameManager.playerInputAllowed = true;
                    });
                return true;
            }
            return false;
        }

        private void UnloadCurrentLevel() {
            if (currentLevelScene != null) {
                DOTween.KillAll();
                currentLevelScene.Unload();
                currentLevelScene = null;
            }
        }

        private void UnloadMainMenu() {
            if (Scenes.MainMenu.IsLoaded()) {
                Scenes.MainMenu.Unload();
            }
        }

        private void LoadLevelImmediate(GameScene levelScene, int levelSceneIndex) {
            UnloadCurrentLevel();

            Debug.Log($"Loading level '{levelScene.fullPathWithExtension}'");
            var asyncLoadOp = levelScene.LoadAsync(LoadSceneMode.Additive);
            asyncLoadOp.completed += op => {
                levelScene.MakeActiveScene();
                currentLevelScene = levelScene;
                currentLevelSceneIndex = levelSceneIndex;
                GameManager.ResetChaos();

                UnloadMainMenu();
            };
        }

        public static void RecordCurrentLevelWin() {
            if (_instance != null && _instance.currentLevelScene != null) {
                PlayerPrefs.SetInt($"level{_instance.currentLevelScene.fullPathWithExtension}", 1);
            }
        }
    }
}
