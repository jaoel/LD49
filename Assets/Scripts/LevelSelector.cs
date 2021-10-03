using UnityEngine;

namespace LD49 {
    public class LevelSelector : MonoBehaviour {
        public int index = 0;
        public GameObject poopGameObject;

        private void OnEnable() {
            int hasCompleted = PlayerPrefs.GetInt($"level{index}", 0);
            if (hasCompleted == 0) {
                poopGameObject.SetActive(false);
            }
        }
    }
}
