using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class EndSceneManager : MonoBehaviour {
        public void LoadMainMenu() {
            GameManager.Instance.LoadMainMenu();
        }
    }
}
