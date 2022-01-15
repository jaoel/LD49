using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace LD49 {
    public class GameManager : MonoBehaviour {

        [SerializeField]
        private Image fadeImage;

        private static GameManager _instance;
        private Tweener timeScale = null;

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
                LevelManager.Instance?.ReloadLevel();
            }

            if (deadTime == 0f && ChaosManager.IsDead) {
                deadTime = Time.time + 5.0f;
            }

            if (!LevelManager.IsLoadingLevel && deadTime > 0.0f && deadTime - Time.unscaledTime <= 0.0f) {
                deadTime = 0.0f;
                LevelManager.Instance?.ReloadLevel();
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
            ChaosManager.SetChaos(0f);
        }

        public void FadeToBlack(Action doneCallback) {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;

            fadeImage.DOKill();
            audioSource.Stop();
            audioSource.PlayOneShot(fadeClip);
            fadeImage.enabled = true;
            fadeImage.DOColor(new Color(0f, 0f, 0f, 0.25f), 1.5f).SetEase(Ease.InOutCubic).OnComplete(() => doneCallback?.Invoke()).SetUpdate(true);
        }

        public void FadeFromBlack() {
            Color color = fadeImage.color;
            color.a = 0.25f;
            fadeImage.color = color;

            audioSource.Stop();
            audioSource.PlayOneShot(fadeClip2);
            fadeImage.DOColor(new Color(0f, 0f, 0f, 1f), 1.5f).SetEase(Ease.InOutCubic).SetUpdate(true).OnComplete(() => {
                if (fadeImage != null) {
                    fadeImage.enabled = false;
                }
            });
        }

        public void PlayFanfare() {
            audioSourceUI.PlayOneShot(fanfareClip, 0.5f);
        }

        public void PlayUIHover() {
            audioSourceUI.PlayOneShot(hoverClip, 0.5f);
        }

        public void PlayUIClick() {
            audioSourceUI.PlayOneShot(clickClip, 0.5f);
        }
    }
}
