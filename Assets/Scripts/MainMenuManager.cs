using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class MainMenuManager : MonoBehaviour {
        public void StartGame() {
            SceneManager.LoadScene("MainScene");
        }
    }
}
