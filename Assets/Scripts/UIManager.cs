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
        private Image chaosBar = null;


        private static UIManager _instance;
        public static UIManager Instance {
            get {
                if (_instance == null) {
                    Debug.LogError("A GameManager is required");
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

        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
        }

        public void ToggleClenchBar(bool on) {
            if (on) {
                clenchBar.DOFade(1.0f, 0.5f);
                clenchBackground.DOFade(1.0f, 0.5f);
            }
            else {
                clenchBar.DOFade(0.0f, 0.5f);
                clenchBackground.DOFade(0.0f, 0.5f);
            }
        }

        public void UpdateClenchBar(float clenchValue) {
            clenchBar.transform.localScale = new Vector3(clenchValue, 1.0f, 1.0f);
        }

        public void UpdateChaos(float chaosValue) {

        }
    }
}
