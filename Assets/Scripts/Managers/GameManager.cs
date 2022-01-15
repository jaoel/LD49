using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace LD49 {
    public class GameManager : MonoBehaviour {

        [SerializeField]
        private Image fadeImage = null;

        private static GameManager _instance;

        public float deadTime = 0.0f;

        public AudioClip fanfareClip;
        public AudioClip hoverClip;
        public AudioClip clickClip;
        public AudioClip fadeClip;
        public AudioClip fadeClip2;

        public AudioSource audioSource;
        public AudioSource audioSourceUI;

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
                if (SceneManager.GetActiveScene().name != "Bootstrap") {
                    LoadMainMenu();
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                LevelManager.ReloadLevel();
            }

            if (!LevelManager.IsLoadingLevel) {
                if (deadTime == 0f && ChaosManager.IsDead) {
                    deadTime = Time.time + 5.0f;
                }

                if (deadTime > 0.0f && deadTime - Time.unscaledTime <= 0.0f) {
                    deadTime = 0.0f;
                    LevelManager.ReloadLevel();
                }
            }
        }

        public static void LoadMainMenu() {
            if (_instance != null) {
                LevelManager.Abort();
                FadeToBlack(() => {
                    SceneManager.LoadScene("Bootstrap");
                    FadeFromBlack();
                });
            }
        }

        public static void LoadEnd() {
            if (_instance != null) {
                FadeToBlack(() => {
                    SceneManager.LoadScene("EndScene");
                    FadeFromBlack();
                    MusicManager.PlayMainMenuMusic();
                });
            }
        }

        public static void ResetChaos() {
            ChaosManager.SetChaos(0f);
        }

        public static void FadeToBlack(Action doneCallback) {
            if (_instance != null) {
                _instance.fadeImage.DOKill();
                _instance.audioSource.Stop();
                _instance.audioSource.PlayOneShot(_instance.fadeClip);
                _instance.fadeImage.enabled = true;
                _instance.fadeImage.DOColor(new Color(0f, 0f, 0f, 0.25f), 1.5f).SetEase(Ease.InOutCubic).OnComplete(() => doneCallback?.Invoke()).SetUpdate(true);
            }
        }

        public static void FadeFromBlack() {
            if (_instance != null) {
                _instance.audioSource.Stop();
                _instance.audioSource.PlayOneShot(_instance.fadeClip2);
                _instance.fadeImage.DOColor(new Color(0f, 0f, 0f, 1f), 1.5f).SetEase(Ease.InOutCubic).SetUpdate(true).OnComplete(() => {
                    if (_instance.fadeImage != null) {
                        _instance.fadeImage.enabled = false;
                    }
                });
            }
        }

        public static void PlayFanfare() {
            if (_instance != null) {
                _instance.audioSourceUI.PlayOneShot(_instance.fanfareClip, 0.5f);
            }
        }

        public static void PlayUIHover() {
            if (_instance != null) {
                _instance.audioSourceUI.PlayOneShot(_instance.hoverClip, 0.5f);
            }
        }

        public static void PlayUIClick() {
            if (_instance != null) {
                _instance.audioSourceUI.PlayOneShot(_instance.clickClip, 0.5f);
            }
        }
    }
}
