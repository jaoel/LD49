using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace LD49 {
    public class OverlayManager : MonoBehaviour {
        private class Transition {
            public Action onFadedOut;
            public Action onFadedIn;
            public float fadeOutDuration;
            public float fadeInDuration;
            public float waitDuration;
        }

        private static OverlayManager _instance;

        [SerializeField]
        private Image fadeImage = null;
        private Coroutine fadeTransitionCoroutine = null;
        private Queue<Transition> transitionQueue = new Queue<Transition>();

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(this);
                Debug.LogWarning("A duplicate OverlayManager was found");
            }
        }

        private void Update() {
            if (fadeTransitionCoroutine == null && transitionQueue.Count > 0) {
                Transition nextTransition = transitionQueue.Dequeue();
                fadeTransitionCoroutine = StartCoroutine(FadeTransitionCoroutine(nextTransition));
            }
        }

        public static void ClearQueue() {
            if (_instance != null) {
                if (_instance.fadeTransitionCoroutine != null) {
                    _instance.StopCoroutine(_instance.fadeTransitionCoroutine);
                    _instance.fadeTransitionCoroutine = null;
                }
                _instance.transitionQueue.Clear();
            }
        }

        public static void QueueFadeTransition(Action onFadedOut, Action onFadedIn, float fadeOutDuration = 0.5f, float fadeInDuration = 0.5f, float waitDuration = 0.5f) {
            if (_instance != null) {
                Transition transition = new Transition {
                    onFadedOut = onFadedOut,
                    onFadedIn = onFadedIn,
                    fadeOutDuration = fadeOutDuration,
                    fadeInDuration = fadeInDuration,
                    waitDuration = waitDuration,
                };
                _instance.QueueFadeTransitionPrivate(transition);
            }
        }

        private void QueueFadeTransitionPrivate(Transition transition) {
            transitionQueue.Enqueue(transition);
        }

        private IEnumerator FadeTransitionCoroutine(Transition transition) {
            float GetT(float doneTime, float duration) {
                return Mathf.Clamp01((doneTime - Time.unscaledTime) / duration);
            }
            // Fade out
            if (transition.fadeOutDuration > 0f) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SwooshOne");
                float fadeOutDoneTime = Time.unscaledTime + transition.fadeOutDuration;
                while (Time.unscaledTime < fadeOutDoneTime) {
                    float t = GetT(fadeOutDoneTime, transition.fadeOutDuration);
                    SetAlpha(1f - t);
                    yield return null;
                }
            }
            SetAlpha(1f);
            transition.onFadedOut?.Invoke();

            yield return new WaitForSeconds(transition.waitDuration);

            // Fade in
            if (transition.fadeInDuration > 0f) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SwooshTwo");
                float fadeInDoneTime = Time.unscaledTime + transition.fadeInDuration;
                while (Time.unscaledTime < fadeInDoneTime) {
                    float t = GetT(fadeInDoneTime, transition.fadeInDuration);
                    SetAlpha(t);
                    yield return null;
                }
            }
            SetAlpha(0f);
            transition.onFadedIn?.Invoke();

            fadeTransitionCoroutine = null;
        }

        private void SetAlpha(float alpha) {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}
