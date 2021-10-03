using UnityEngine;

namespace LD49 {
    public class PlayerTrigger : MonoBehaviour {
        public delegate void PlayerTriggerDelegate();
        public event PlayerTriggerDelegate PlayerEnter;

        [SerializeField]
        private GameObject marker = null;

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Player player)) {
                PlayerEnter?.Invoke();
            }
        }

        private void Update() {
            if (PlayerEnter != null && marker != null && !marker.activeInHierarchy) {
                marker.SetActive(true);
            }
            else if (PlayerEnter == null) {
                marker.SetActive(false);
            }

            if (marker.activeSelf) {

            }
        }
    }
}
