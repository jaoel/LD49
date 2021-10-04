using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace LD49 {
    public class GameManager : MonoBehaviour {

        private float chaosScore = 0f;

        [SerializeField]
        private Image fadeImage;

        [SerializeField]
        private int maxChaos = 100;

        private static GameManager _instance;
        private Tweener timeScale = null;

        public bool IsDead => chaosScore >= maxChaos;
        public float deadTime = 0.0f;

        public AudioClip fanfareClip;
        public AudioClip hoverClip;
        public AudioClip clickClip;
        public AudioClip fadeClip;
        public AudioClip fadeClip2;

        public AudioSource audioSource;
        public AudioSource audioSourceUI;

        public static GameManager Instance {
            get {
                if (_instance == null) {
                    Debug.LogError("A GameManager is required");
                }
                return _instance;
            }
        }

        public static MusicManager MusicManager => Instance.musicManager;

        [SerializeField]
        private MusicManager musicManager;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
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
                LevelManager.Instance.ReloadLevel();
            }

            if (deadTime > 0.0f && deadTime - Time.unscaledTime <= 0.0f) {
                deadTime = 0.0f;
                LevelManager.Instance.ReloadLevel();
            }
        }

        public void LoadMainMenu() {
            FadeToBlack(() => {
                SceneManager.LoadScene("Bootstrap");
                FadeFromBlack();
                Destroy(UIManager.Instance.gameObject);
            });
        }

        public void LoadEnd() {
            FadeToBlack(() => {
                SceneManager.LoadScene("EndScene");
                FadeFromBlack();
                Destroy(UIManager.Instance.gameObject);
                MusicManager.PlayMainMenuMusic();
            });
        }

        public void ResetChaos() {
            SetChaos(0f);
        }

        internal float GetChaos() {
            return chaosScore / (float)maxChaos;
        }

        public void UpdateChaos(float score) {
            if (!IsDead) {
                SetChaos(chaosScore + score);
                if (chaosScore >= maxChaos) {
                    deadTime = Time.time + 5.0f;
                }
            }
        }

        private void SetChaos(float value) {
            chaosScore = value;
            UIManager.Instance.UpdateChaos(Mathf.Clamp01(chaosScore / (float)maxChaos));
        }

        public void FadeToBlack(Action doneCallback) {
            if (fadeImage.color.a > 0.5f) {
                audioSource.Stop();
                audioSource.PlayOneShot(fadeClip);
                fadeImage.DOColor(new Color(0f, 0f, 0f, 0.25f), 1.5f).SetEase(Ease.InOutCubic).OnComplete(() => doneCallback?.Invoke()).SetUpdate(true);
            } else {
                doneCallback?.Invoke();
            }
        }

        public void FadeFromBlack() {
            audioSource.Stop();
            audioSource.PlayOneShot(fadeClip2);
            fadeImage.DOColor(new Color(0f, 0f, 0f, 1f), 1.5f).SetEase(Ease.InOutCubic).SetUpdate(true);
        }

        public void PlayFanfare() {
            audioSource.PlayOneShot(fanfareClip, 0.5f);
        }

        public void PlayUIHover() {
            audioSourceUI.PlayOneShot(hoverClip, 0.5f);
        }

        public void PlayUIClick() {
            audioSourceUI.PlayOneShot(clickClip, 0.5f);
        }
    }
}
