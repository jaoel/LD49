using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace LD49 {
    public class UIManager : MonoBehaviour {

        [SerializeField]
        private Image clenchBar = null;

        [SerializeField]
        private Image clenchBackground = null;

        [SerializeField]
        private TMPro.TMP_Text clenchText = null;

        [SerializeField]
        private Image chaosBar = null;

        [SerializeField]
        private Gradient chaosGradient = null;

        [SerializeField]
        private Gradient clenchGradient = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI objectiveText = null;

        [SerializeField]
        private CanvasGroup objectiveGroup = null;

        [SerializeField]
        private RectTransform chaosWrapper = null;

        [SerializeField]
        private RectTransform clenchWrapper = null;

        private float noiseTime = 0f;

        private float clenchNoiseTime = 0f;

        private static UIManager _instance;
        public static UIManager Instance {
            get {
                if (_instance == null) {
                    Debug.LogError("A UIManager is required");
                }
                return _instance;
            }
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(gameObject);
                Debug.LogWarning("A duplicate UIManager was found");
            }
        }

        private void Update() {
            float chaos = ChaosManager.GetChaos();
            noiseTime += Time.unscaledDeltaTime;
            float t = noiseTime * Mathf.Lerp(5f, 15f, chaos);
            Vector2 noise = new Vector2(1f - 2f * Mathf.PerlinNoise(t + 100f, t), Mathf.PerlinNoise(t * 1.2f, t + 200f)) * 35f;
            chaosWrapper.anchoredPosition = noise * chaos;

            clenchNoiseTime += Time.unscaledDeltaTime * 30f;
            t = clenchNoiseTime;
            Vector2 clenchNoise = new Vector2(1f - 2f * Mathf.PerlinNoise(t + 400f, t), Mathf.PerlinNoise(t * 1.2f, t + 600f)) * 8f;
            clenchWrapper.anchoredPosition = clenchNoise * clenchBar.transform.localScale.x;
            clenchWrapper.localScale = Vector3.one * (1f + 0.01f * Mathf.Sin(Time.unscaledTime * 10f));
        }

        public void ToggleClenchBar(bool on) {
            if (on) {
                clenchBar.DOFade(1.0f, 0.5f);
                clenchBackground.DOFade(1.0f, 0.5f);
                clenchText.DOFade(1.0f, 0.5f);
            }
            else {
                clenchBar.DOFade(0.0f, 0.5f);
                clenchBackground.DOFade(0.0f, 0.5f);
                clenchText.DOFade(0.0f, 0.5f);
            }
        }

        public void UpdateClenchBar(float clenchValue) {
            clenchBar.transform.localScale = new Vector3(clenchValue, 1.0f, 1.0f);
            clenchBar.color = clenchGradient.Evaluate(clenchValue);
        }

        public void UpdateChaos(float chaosValue) {
            chaosBar.transform.localScale = new Vector3(chaosValue, 1.0f, 1.0f);
            chaosBar.color = chaosGradient.Evaluate(chaosValue);
        }

        public void ShowObjective(string text) {
            objectiveText.text = text;

            Sequence objSequence = DOTween.Sequence();
            //objSequence.Append(objectiveGroup.DOFade(2.0f, 1.0f));
            //objSequence.AppendInterval(1.0f);
            //objSequence.Append(objectiveGroup.DOFade(0.0f, 2.0f));
        }
    }
}
