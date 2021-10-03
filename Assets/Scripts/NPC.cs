using System;
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

        private AnimationType currentAnimation;

        private void Awake() {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = true;
            }
            foreach (Collider c in GetComponentsInChildren<Collider>()) {
                c.enabled = false;
            }
        }

        private void Start() {
            UpdateAnimationIndex();
        }

        private void UpdateAnimationIndex() {
            currentAnimation = animation;
            animator.Play(currentAnimation.ToString());
        }

        private void Update() {
            if (currentAnimation != animation) {
                UpdateAnimationIndex();
            }
        }

        public void SetRagdoll(Vector3 explosionPosition, float explosionForce) {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, explosionPosition, 5f);
            }
            foreach (Collider c in GetComponentsInChildren<Collider>()) {
                c.enabled = true;
            }
        }
    }
}
