using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class Player : MonoBehaviour {

        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private Rigidbody rigidbody = null;

        [SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private float acceleration = 0.0f;

        [SerializeField]
        private float maxMovementVelocity = 0.0f;

        private float currentVelocity = 0.0f;

        private void Start() {

        }

        private void Update() {
            //CarMovement();
            Movement();
        }

        private void Movement() {
            int dirX = 0;
            int dirZ = 0;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                dirZ = 1;
            } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                dirZ = -1;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                dirX = 1;
            } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                dirX = -1;
            }

            Vector3 direction = new Vector3(dirX, 0.0f, dirZ);
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
                //characterController.Move(Vector3.Scale(movementDir, new Vector3(currentVelocity, 0.0f, currentVelocity) * Time.deltaTime));

                if (animator != null) {
                    animator.SetFloat("RunSpeed", Mathf.MoveTowards(1.0f, Mathf.Clamp01(currentVelocity / maxMovementVelocity), Time.deltaTime));
                }
            }
            else {
                animator.SetFloat("RunSpeed", 0.0f);
            }
        }

        /*
        private void CarMovement() {
            if (characterController != null) {
                int movementDirection = 0;
                int rotationDirection = 0;

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                    rotationDirection = 1;
                } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                    rotationDirection = -1;
                }

                if (rotationDirection != 0) {
                    rotationVelocity += rotationAcceleration * Time.deltaTime * rotationDirection;
                    rotationVelocity = Mathf.Clamp(rotationVelocity, -maxRotationVelocity, maxRotationVelocity);
                    oldRotation = rotationDirection;
                } else if (rotationVelocity != 0.0f) {
                    rotationVelocity -= rotationAcceleration * Time.deltaTime * oldRotation;

                    if (oldRotation == -1) {
                        rotationVelocity = Mathf.Min(rotationVelocity, 0.0f);
                    } else if (oldRotation == 1) {
                        rotationVelocity = Mathf.Max(rotationVelocity, 0.0f);
                    }

                    if (rotationVelocity == 0.0f) {
                        oldRotation = 0;
                    }
                }

                if (rotationVelocity != 0.0f) {
                    transform.forward = (Quaternion.Euler(0.0f, rotationVelocity, 0.0f) * transform.forward).normalized;
                }


                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                    movementDirection = 1;
                } else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                    movementDirection = -1;
                }

                if (movementDirection != 0) {
                    currentVelocity += acceleration * Time.deltaTime * movementDirection;
                    currentVelocity = Mathf.Clamp(currentVelocity, -maxMovementVelocity, maxMovementVelocity);

                    oldMovement = movementDirection;
                } else if (currentVelocity != 0.0f) {
                    currentVelocity -= acceleration * Time.deltaTime * oldMovement;

                    if (oldMovement == -1) {
                        currentVelocity = Mathf.Min(currentVelocity, 0.0f);
                    } else if (oldMovement == 1) {
                        currentVelocity = Mathf.Max(currentVelocity, 0.0f);
                    }

                    if (currentVelocity == 0.0f) {
                        oldMovement = 0;
                    }
                }

                Vector3 velocity = new Vector3(currentVelocity, 0.0f, currentVelocity);
                characterController.Move(Vector3.Scale(transform.forward.normalized, velocity * Time.deltaTime));
            }
        }
        */
    }
}

