using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

namespace LD49 {
    public class Prop : MonoBehaviour {
        public float score = 1f;

        private Rigidbody rigidBody = null;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
        }

        private float cooldownTime = 0f;

        public void OnCollisionEnter(Collision collision) {
            if (collision.relativeVelocity.magnitude >= 2.0f && cooldownTime - Time.time < 0f) {
                AddChaos();
                cooldownTime = Time.time + 2f;
                FMODUnity.RuntimeManager.PlayOneShot("event:/PropCollisionDefault");
            }
        }

        public void AddChaos(float multiplier = 1f) {
            ChaosManager.UpdateChaos((int)(score * multiplier));
        }

        private void Update() {
            if (transform.position.y < -5f) {
                Destroy(gameObject);
            }
        }
    }
}
