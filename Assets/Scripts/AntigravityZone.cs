using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class AntigravityZone : MonoBehaviour {
        private ActivePlayer playerInTrigger = null;

        private void OnTriggerStay(Collider other) {
            if (other.gameObject.TryGetComponent(out PlayerSpine spine)) {
                playerInTrigger = spine.player;
                playerInTrigger.IsInSpace = true;
            }
        }

        private void Update() {
            if (playerInTrigger != null) {
                Debug.Log("YES");
            } else {
                Debug.Log("NO");
            }
        }

        private void FixedUpdate() {
            if (playerInTrigger != null) {
                playerInTrigger.IsInSpace = false;
            }
            playerInTrigger = null;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            foreach (var collider in GetComponentsInChildren<BoxCollider>()) {
                Gizmos.matrix = collider.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(collider.center, collider.size);
            }
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
    }
}
