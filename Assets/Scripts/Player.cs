using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class Player : MonoBehaviour {

        [SerializeField]
        private CharacterController characterController;

        [SerializeField]
        private float acceleration = 0.0f;

        [SerializeField]
        private float rotationAcceleration = 0.0f;

        [SerializeField]
        private float rotationDeceleration = 0.0f;

        [SerializeField]
        private float maxMovementVelocity = 10.0f;

        [SerializeField]
        private float maxRotationVelocity = 10.0f;

        private float currentVelocity = 0.0f;
        private float rotationVelocity = 0.0f;

        private int oldRotation = 0;
        private int oldMovement = 0;

        private void Start() {

        }

        private void Update() {
            Movement();
        }

        private void Movement() {
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
                }
                else if (rotationVelocity != 0.0f) {
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
                }
                else if (currentVelocity != 0.0f) {
                    currentVelocity -= acceleration * Time.deltaTime * oldMovement;

                    if (oldMovement == -1) {
                        currentVelocity = Mathf.Min(currentVelocity, 0.0f);
                    }
                    else if (oldMovement == 1) {
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
    }
}

