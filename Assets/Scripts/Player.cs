using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class Player : MonoBehaviour {

        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private new Rigidbody rigidbody = null;

        [SerializeField]
        private float acceleration = 0.0f;

        [SerializeField]
        private float maxMovementVelocity = 0.0f;

        private float currentVelocity = 0.0f;

        private void Start() {

        }

        private void Update() {
            Movement();
        }

        private void Movement() {
            Vector3 forwardDir = Vector3.zero;
            Vector3 rightDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                forwardDir = Camera.main.transform.forward;
            } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                forwardDir = -Camera.main.transform.forward;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                rightDir = Camera.main.transform.right;
            } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                rightDir = -Camera.main.transform.right;
            }

            Vector3 direction = (new Vector3(forwardDir.x, 0.0f, forwardDir.z) + new Vector3(rightDir.x, 0.0f, rightDir.z)).normalized;// new Vector3(dirX, 0.0f, dirZ);
            if (direction.magnitude > 0.0f) {
                currentVelocity += acceleration * Time.deltaTime;
                currentVelocity = Mathf.Min(maxMovementVelocity, currentVelocity);
            } else {
                currentVelocity -= acceleration * Time.deltaTime;
                currentVelocity = Mathf.Max(currentVelocity, 0.0f);
            }

            if (currentVelocity > 0.0f) {
                transform.forward = Vector3.RotateTowards(transform.forward.normalized, direction.normalized, 4.0f * Time.deltaTime, 0.0f).normalized;
                Vector3 movementDir = Vector3.RotateTowards(transform.forward.normalized, direction.normalized, 0.1f * Time.deltaTime, 0.0f).normalized;
                rigidbody.velocity = Vector3.Scale(movementDir, new Vector3(currentVelocity, 0.0f, currentVelocity));

                if (animator != null) {
                    animator.SetFloat("RunSpeed", Mathf.MoveTowards(1.0f, Mathf.Clamp01(currentVelocity / maxMovementVelocity), Time.deltaTime * 0.1f));
                }
            }
            else {
                animator.SetFloat("RunSpeed", 0.0f);
            }
        }
    }
}

