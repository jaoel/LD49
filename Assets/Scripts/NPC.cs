using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class NPC : MonoBehaviour {
        private enum AnimationType {
            IdleCoffee,
            IdleWaiting,
            IdleNormal,
            SittingIdle,
        }

        [SerializeField]
        private AnimationType animation = AnimationType.IdleNormal;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private List<AudioClip> screams = null;

        public Collider playerCollider;

        private AnimationType currentAnimation;
        private AudioSource audioSource = null;

        public Transform spineBone;

        private void Awake() {

            audioSource = GetComponent<AudioSource>();
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = true;
            }
            foreach (Collider c in GetComponentsInChildren<Collider>()) {
                if (c != playerCollider) {
                    c.enabled = false;
                }
            }
        }

        private void Start() {
            UpdateAnimationIndex();
        }

        private void UpdateAnimationIndex() {
            currentAnimation = animation;
            animator.Play(currentAnimation.ToString(), 0, UnityEngine.Random.Range(0f, 5f));
        }

        private void Update() {
            if (currentAnimation != animation) {
                UpdateAnimationIndex();
            }

            if (spineBone.position.y < -5f) {
                Destroy(gameObject);
            }
        }

        private static float lastClipPlayTime = 0f;
        public void SetRagdoll(Vector3 explosionPosition, float explosionForce) {

            if (Time.time - lastClipPlayTime > 1f) {
                lastClipPlayTime = Time.time;
                audioSource.clip = screams[UnityEngine.Random.Range(0, screams.Count)];
                audioSource.volume = 0.25f;
                audioSource.Play();
            }

            animator.enabled = false;
            playerCollider.enabled = false;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, explosionPosition, 5f);
            }
            foreach (Collider c in GetComponentsInChildren<Collider>()) {
                if (c != playerCollider) {
                    c.enabled = true;
                }
            }
        }
    }
}
