using UnityEngine;

namespace LD49 {
    public class PlayerTrigger : MonoBehaviour {
        public delegate void PlayerTriggerDelegate();
        public event PlayerTriggerDelegate PlayerEnter;

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Player player)) {
                PlayerEnter?.Invoke();
            }
        }
    }
}
