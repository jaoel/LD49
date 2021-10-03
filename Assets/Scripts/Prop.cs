using UnityEngine;

namespace LD49 {
    public class Prop : MonoBehaviour {
        public float score = 2f;

        public void OnCollisionEnter(Collision collision) {
            if (collision.relativeVelocity.magnitude >= 2.0f) {
                GameManager.Instance.UpdateChaos((int)score);
            }
        }
    }
}
