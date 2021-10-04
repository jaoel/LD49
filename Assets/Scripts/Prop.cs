using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class Prop : MonoBehaviour {
        public float score = 1f;

        private Rigidbody rigidBody = null;
        private AudioSource audioSource = null;

        [SerializeField]
        private List<AudioClip> clips = null;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
        }

        private float cooldownTime = 0f;

        public void OnCollisionEnter(Collision collision) {
            if (collision.relativeVelocity.magnitude >= 2.0f && cooldownTime - Time.time < 0f) {
                AddChaos();
                cooldownTime = Time.time + 2f;

                audioSource.clip = clips[Random.Range(0, clips.Count)];
                audioSource.Play();
            }
        }

        public void AddChaos(float multiplier = 1f) {
            GameManager.Instance.UpdateChaos((int)(score * multiplier));
        }

        private void Update() {
            if (transform.position.y < -5f) {
                Destroy(gameObject);
            }
        }
    }
}
