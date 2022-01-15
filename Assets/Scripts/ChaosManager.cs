using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace LD49 {
    public class ChaosManager : MonoBehaviour {

        private float chaosScore = 0f;

        [SerializeField]
        private int maxChaos = 100;

        public static bool IsDead => _instance != null && _instance.chaosScore >= _instance.maxChaos;

        private static ChaosManager _instance;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(gameObject);
                Debug.LogWarning("A duplicate GameManager was found");
            }
        }

        private void Update() {
        }

        public static void ResetChaos() {
            SetChaos(0f);
        }

        public static float GetChaos() {
            if (_instance != null) {
                return _instance.chaosScore / (float)_instance.maxChaos;
            }
            return 0f;
        }

        public static void UpdateChaos(float score) {
            if (_instance != null) {
                SetChaos(_instance.chaosScore + score);
            }
        }

        public static void SetChaos(float value) {
            if (_instance != null) {
                _instance.chaosScore = value;
                UIManager.Instance?.UpdateChaos(Mathf.Clamp01(_instance.chaosScore / (float)_instance.maxChaos));
            }
        }
    }
}
