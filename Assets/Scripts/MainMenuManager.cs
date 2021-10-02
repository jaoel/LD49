using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class MainMenuManager : MonoBehaviour {

        private void Start() {
            GameManager.MusicManager.PlayMainMenuMusic();
        }

        public void StartGame() {
            LevelManager.RequestLevel(0);
            SceneManager.LoadScene("MainScene");
        }
    }
}
