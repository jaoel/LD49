using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace LD49 {
    public class GameManager : MonoBehaviour {

        private float chaosScore = 0f;

        [SerializeField]
        private int maxChaos = 100;

        private static GameManager _instance;

        public bool IsDead => chaosScore >= maxChaos;

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
                    SceneManager.LoadScene("Bootstrap");
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                LevelManager.Instance.ReloadLevel();
            }
        }

        public void ResetChaos() {
            SetChaos(0f);
        }

        internal float GetChaos() {
            return chaosScore / (float)maxChaos;
        }

        public void UpdateChaos(float score) {
            SetChaos(chaosScore + score);

            if (chaosScore >= maxChaos) {
                DOTween.To(x => Time.timeScale = x, Time.timeScale, 0.3f, 2.0f);
            }
        }

        private void SetChaos(float value) {
            chaosScore = value;
            UIManager.Instance.UpdateChaos(Mathf.Clamp01(chaosScore / (float)maxChaos));
        }
    }
}
