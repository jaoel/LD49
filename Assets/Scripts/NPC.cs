using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace LD49 {
    public class NPC : MonoBehaviour {
        private enum AnimationType {
            IdleCoffee,
            IdleWaiting,
            IdleNormal,
            SittingIdle,
        }

        [SerializeField]
        private new AnimationType animation = AnimationType.IdleNormal;

        [SerializeField]
        private Animator animator;
        public Collider playerCollider;

        private AnimationType currentAnimation;

        public Transform spineBone;

        private void Awake() {
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
                FMODUnity.RuntimeManager.PlayOneShot("event:/Scream");
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
