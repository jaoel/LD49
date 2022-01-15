using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class MainMenuController : MonoBehaviour {

        public GameObject mainObject;
        public GameObject levelSelectObject;
        public LevelHolder levelHolder;
        public GameObject levelSelectButtons;

        private bool loading = false;

        private void Awake() {
            mainObject.SetActive(true);
            levelSelectObject.SetActive(false);
        }

        private void Start() {
            MusicManager.PlayMainMenuMusic();
        }

        public void StartGame() {
            StartLevel(0);
        }

        public void ExitGame() {
            Application.Quit();
        }

        public void LevelSelect() {
            if (!loading) {
                mainObject.SetActive(false);
                levelSelectObject.SetActive(true);

                int i = 1;
                foreach (Transform button in levelSelectButtons.transform) {
                    if (levelHolder.levelPrefabs.Count < i) {
                        button.gameObject.SetActive(false);
                    } else {
                        button.gameObject.SetActive(true);
                    }
                    i++;
                }
            }
        }

        public void BackFromLevelSelect() {
            if (!loading) {
                mainObject.SetActive(true);
                levelSelectObject.SetActive(false);
            }
        }

        public void StartLevel(int levelID) {
            if (!loading) {
                loading = true;
                GameManager.LoadGameAtLevel(levelID);
            }
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, 0f);
                if (plane.Raycast(mouseRay, out float enter)) {
                    Vector3 pos = mouseRay.origin + mouseRay.direction * enter;

                    foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>()) {
                        if (Vector3.Distance(rb.position, pos) < 5f) {
                            rb.AddExplosionForce(1000f, pos, 10f);
                        }
                    }
                }
            }
        }
    }
}
