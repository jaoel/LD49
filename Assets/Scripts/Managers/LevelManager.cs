﻿using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class LevelManager : MonoBehaviour {
        private static int requestedLevel = 0;
        private static bool loadingLevel = false;
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
        private LevelHolder levelHolder = null;

        private Level currentLevel = null;
        private int currentLevelIndex = -1;

        public static bool IsLoadingLevel => loadingLevel;

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
                if (requestedLevel >= levelHolder.levelPrefabs.Count) {
                    GameManager.Instance.LoadEnd();
                    requestedLevel = -1;
                } else {
                    TryStartLoadLevel(requestedLevel);
                    requestedLevel = -1;
                }
            }
        }

        public void Abort() {
            if (IsLoadingLevel) {
                loadingLevel = false;
                DOTween.KillAll();
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

        private bool TryStartLoadLevel(int index) {
            if (loadingLevel) {
                return false;
            }

            loadingLevel = true;
            DOTween.KillAll();
            if (index < 0 || index >= levelHolder.levelPrefabs.Count) {
                Debug.LogError($"There is no level with index {index}");
                return false;
            }

            Level levelPrefab = levelHolder.levelPrefabs[index];
            if (levelPrefab == null) {
                Debug.LogError($"Level with index {index} is null");
                return false;
            }

            GameManager.Instance.FadeToBlack(() => LoadLevelActually(levelPrefab, index));
            return true;
        }

        private void LoadLevelActually(Level levelPrefab, int index) {

            if (currentLevel != null) {
                Destroy(currentLevel.gameObject);
            }

            Debug.Log($"Loading '{levelPrefab.name}' at index {index}");
            currentLevel = Instantiate(levelPrefab);
            currentLevelIndex = index;
            GameManager.Instance.ResetChaos();

            GameManager.Instance.FadeFromBlack();

            loadingLevel = false;
        }

        public void ReloadLevel() {
            TryStartLoadLevel(currentLevelIndex);
        }
    }
}
