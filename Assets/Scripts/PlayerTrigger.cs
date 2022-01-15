using UnityEngine;
using UnityEngine.Events;

namespace LD49 {
    public class PlayerTrigger : MonoBehaviour {
        public delegate void PlayerTriggerDelegate();
        public event PlayerTriggerDelegate PlayerEnter;
        public UnityEvent playerEnterUnityEvent = new UnityEvent();
        public bool hasUnityEvent = false;

        [SerializeField]
        private GameObject marker = null;

        [SerializeField]
        private ParticleSystem particles = null;

        Vector3 posOffset = new Vector3();
        Vector3 tempPos = new Vector3();

        private void Start() {
            posOffset = marker.transform.position;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Player player) && !ChaosManager.IsDead) {
                PlayerEnter?.Invoke();
                playerEnterUnityEvent?.Invoke();
            }
        }

        private void Update() {
            if (!hasUnityEvent) {
                if (PlayerEnter != null && marker != null && !marker.activeInHierarchy) {
                    marker.SetActive(true);
                } else if (PlayerEnter == null) {
                    if (particles != null && marker.activeInHierarchy) {
                        particles.Play();
                    }
                    marker.SetActive(false);
                }
            }

            if (marker.activeSelf) {
                tempPos = posOffset;
                tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * 0.5f) * 0.5f;

                marker.transform.position = tempPos;
            }
        }
    }
}
