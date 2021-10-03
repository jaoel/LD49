using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class MainMenuManager : MonoBehaviour {

        public GameObject mainObject;
        public GameObject levelSelectObject;

        private void Awake() {
            mainObject.SetActive(true);
            levelSelectObject.SetActive(false);
        }

        private void Start() {
            GameManager.MusicManager.PlayMainMenuMusic();
        }

        public void StartGame() {
            LevelManager.RequestLevel(0);
            SceneManager.LoadScene("MainScene");
        }

        public void LevelSelect() {
            mainObject.SetActive(false);
            levelSelectObject.SetActive(true);
        }

        public void BackFromLevelSelect() {
            mainObject.SetActive(true);
            levelSelectObject.SetActive(false);
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, 0f);
                if (plane.Raycast(mouseRay, out float enter)) {
                    Vector3 pos = mouseRay.origin + mouseRay.direction * enter;

                    foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>()) {
                        if (Vector3.Distance(rb.position, pos) < 5f) {
                            rb.AddExplosionForce(500f, pos, 5f);
                        }
                    }
                }
            }
        }
    }
}
