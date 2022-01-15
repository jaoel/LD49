using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class EndSceneController : MonoBehaviour {
        public void LoadMainMenu() {
            GameManager.Instance.LoadMainMenu();
        }
    }
}
