using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace LD49 {
    public class GameManager : MonoBehaviour {

        private static GameManager _instance;
        private Coroutine respawnCoroutine = null;

        public FarticleSystem activeFarticleSystemPrefab;

        public static FarticleSystem ActiveFarticleSystemPrefab => _instance == null ? null : _instance.activeFarticleSystemPrefab;

        // TODO: Add input manager
        public static bool playerInputAllowed = true;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                // We delete 'gameObject' here, and not 'this', because GameManager should be the root object
                // and container for multiple other managers
                Destroy(gameObject);
                Debug.LogWarning("A duplicate GameManager was found");
            }
        }

        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (!Scenes.MainMenu.IsLoaded()) {
                    OverlayManager.ClearQueue();
                    TransitionToScene(Scenes.MainMenu);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                OverlayManager.ClearQueue();
                LevelManager.ReloadCurrentLevel();
            }

            if (ChaosManager.IsDead && respawnCoroutine == null) {
                respawnCoroutine = StartCoroutine(Respawn());
            }
        }

        private IEnumerator Respawn() {
            yield return new WaitForSeconds(5f);
            LevelManager.ReloadCurrentLevel(() => respawnCoroutine = null);
        }

        public static void ResetChaos() {
            ChaosManager.SetChaos(0f);
        }

        public static void PlayFanfare() {
            if (_instance != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Effects/LevelEndFanfare");
            }
        }

        public static void PlayUIHover() {
            if (_instance != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/ButtonHover");
            }
        }

        public static void PlayUIClick() {
            if (_instance != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/ButtonClick");
            }
        }

        public static bool LoadGameAtLevel(int levelSceneIndex) {
            return LevelManager.LoadLevel(levelSceneIndex);
        }

        public static void TransitionToScene(GameScene scene, Action onComplete = null) {
            OverlayManager.QueueFadeTransition(() => {
                DOTween.KillAll();
                scene.Load();
            }, onComplete);
        }
    }
}
