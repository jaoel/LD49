using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class GameManager : MonoBehaviour {
        private static GameManager _instance;
        public static GameManager Instance {
            get {
                if (_instance == null) {
#if UNITY_EDITOR
                    _instance = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameManager>("Prefabs/GameManager"));
#else
                    Debug.LogError("A GameManager is required");
#endif
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
        }
    }
}
