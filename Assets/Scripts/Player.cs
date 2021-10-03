using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LD49 {
    public class Player : MonoBehaviour {

        [SerializeField]
        private GameObject fartWarning = null;

        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private new Rigidbody rigidbody = null;

        [SerializeField]
        private Rigidbody spineRigidbody = null;

        [SerializeField]
        private ParticleSystem farticleSystem = null;

        [SerializeField]
        private float acceleration = 0.0f;

        [SerializeField]
        private float maxMovementVelocity = 0.0f;

        private float currentVelocity = 0.0f;

        [SerializeField]
        private GameObject armature = null;

        [SerializeField]
        private CapsuleCollider collider = null;

        private Quaternion[] boneRotations = null;

        [SerializeField]
        private float minFartForce = 0.0f;

        [SerializeField]
        private float maxFartForce = 0.0f;

        [SerializeField]
        private float minFartTimer = 0.0f;

        [SerializeField]
        private float maxFartTimer = 0.0f;

        [SerializeField]
        private float maxClenchTimer = 0.0f;

        private float fartTimer = 0.0f;
        private float clenchTimer = 0.0f;

        private void Awake() {
            boneRotations = armature.GetComponentsInChildren<Transform>().Select(x => x.localRotation).ToArray();
            foreach (Rigidbody rb in armature.GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = true;
            }

            foreach (Collider c in armature.GetComponentsInChildren<Collider>()) {
                c.enabled = false;
            }
        }

        private void Start() {
            SetFartTimer();
        }

        private void Update() {
            Movement();

            if (Input.GetKeyDown(KeyCode.Return)) {
                //ToggleRagdoll();
                Fart();
            }

            if (Input.GetKeyDown(KeyCode.Backspace)) {
                ToggleRagdoll();
            }

            if (fartTimer > 0) {

                if (fartTimer - Time.time < 2.0f) {
                    fartWarning.SetActive(true);
                }


                if (fartTimer - Time.time < 0.0f && (clenchTimer == 0.0f || clenchTimer >= maxClenchTimer)) {
                    Fart();
                }
            }

            Clench();
        }

        private void Clench() {
            if (Input.GetKey(KeyCode.Space)) {
                clenchTimer += Time.deltaTime;
            }

            if (clenchTimer > 0.0f && Input.GetKeyUp(KeyCode.Space) && fartWarning.activeSelf) {
                clenchTimer = 0.0f;
                Fart();
            }
        }

        private void SetFartTimer() {
            fartTimer = Time.time + Random.Range(minFartTimer, maxFartTimer);
            fartWarning.SetActive(false);
        }

        private void Fart() {
            if (!rigidbody.isKinematic) {
                ToggleRagdoll();
            }
            //Quaternion.Euler(Random.Range(0.0f, 90.0f), Random.Range(-45.0f, 45.0f), 0.0f)

            Vector3 force = Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), transform.forward) * Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), Vector3.up) * (transform.forward + Vector3.up) * Random.Range(minFartForce, maxFartForce);
            farticleSystem.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -force);
            farticleSystem.Play();
            spineRigidbody.AddForce(force, ForceMode.Impulse);

            SetFartTimer();
        }

        private void ToggleRagdoll() {
            rigidbody.isKinematic = !rigidbody.isKinematic;
            animator.enabled = !animator.enabled;
            collider.enabled = !collider.enabled;

            if (!rigidbody.isKinematic) {
                int i = 0;
                foreach (Transform t in armature.GetComponentsInChildren<Transform>()) {
                    t.localRotation = boneRotations[i++];
                }

                collider.transform.position = spineRigidbody.position;
            }


            foreach (Rigidbody rb in armature.GetComponentsInChildren<Rigidbody>()) {
                rb.isKinematic = !rb.isKinematic;
            }

            foreach (Collider c in armature.GetComponentsInChildren<Collider>()) {
                c.enabled = !c.enabled;
            }

            currentVelocity = 0.0f;

        }

        private void Movement() {
            if (rigidbody.isKinematic) {
                return;
            }


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

            Debug.Log(direction);
            if (currentVelocity > 0.0f) {
                transform.forward = Vector3.RotateTowards(transform.forward.normalized, direction.normalized, 4.0f * Time.deltaTime, 0.0f).normalized;
                Vector3 movementDir = Vector3.RotateTowards(transform.forward.normalized, direction.normalized, 0.5f * Time.deltaTime, 0.0f).normalized;
                rigidbody.velocity = Vector3.Scale(movementDir, new Vector3(currentVelocity, 0.0f, currentVelocity));

                if (animator != null) {
                    animator.SetFloat("RunSpeed", Mathf.MoveTowards(1.0f, Mathf.Clamp01(currentVelocity / maxMovementVelocity), Time.deltaTime * 0.1f));
                }
            } else {
                animator.SetFloat("RunSpeed", 0.0f);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "Props") {
                if (currentVelocity >= maxMovementVelocity * 0.75f) {
                    if (!rigidbody.isKinematic) {
                        ToggleRagdoll();
                    }

                    spineRigidbody.AddForce(Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), transform.forward) * Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), Vector3.up)
                        * (-transform.forward + Vector3.up) * Random.Range(10, 25), ForceMode.Impulse);
                }
            }
        }

        private void OnCollisionStay(Collision collision) {
            if (LayerMask.LayerToName(collision.collider.gameObject.layer) == "Props") {
                currentVelocity = Mathf.Min(currentVelocity, maxMovementVelocity * 0.5f);
            }
        }
    }
}

