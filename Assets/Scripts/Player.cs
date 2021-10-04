using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LD49 {
    public class Player : MonoBehaviour {

        [SerializeField]
        private GameObject fartWarningYellow = null;

        [SerializeField]
        private GameObject fartWarningRed = null;

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

        private const float fartWarningDuration = 2f;

        private float fartTimer = 0.0f;
        private float clenchTimer = 0.0f;
        private float deadTimer = 0.0f;
        private float warningWobbleTime = 0f;
        private bool hasMoved = false;

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
            fartWarningRed.SetActive(false);
            fartWarningYellow.SetActive(false);
            SetFartTimer();
        }

        private void Update() {
            if (!hasMoved) {
                SetFartTimer();
            }

            Movement();

            if (Input.GetKeyDown(KeyCode.Return)) {
                //ToggleRagdoll();
                Fart();
            }

            if (Input.GetKeyDown(KeyCode.Backspace)) {
                ToggleRagdoll();
            }
            bool farted = false;
            if (fartTimer > 0) {

                if (FartImminent()) {
                    fartWarningYellow.SetActive(false);
                    fartWarningRed.SetActive(true);
                } else if (WillFartSoon()) {
                    fartWarningYellow.SetActive(true);
                    fartWarningRed.SetActive(false);
                }

                if (fartTimer - Time.time < 0.0f && (clenchTimer == 0.0f || clenchTimer >= maxClenchTimer)) {
                    Fart();
                    farted = true;
                }
            }

            Clench();

            if (!farted) {
                ResetPlayer();
            }

            float farty = 1f - HowFarty();
            warningWobbleTime += Time.deltaTime * farty * 5f;
            float warningScale = Mathf.Lerp(1f, 1.25f + 0.25f * Mathf.Sin(warningWobbleTime * 5f) * farty, farty);
            fartWarningYellow.transform.rotation = fartWarningRed.transform.rotation = Quaternion.identity;
            fartWarningYellow.transform.localScale = fartWarningRed.transform.localScale = Vector3.one * warningScale;
        }

        private bool WillFartSoon() {
            return fartTimer - Time.time < fartWarningDuration;
        }

        private bool FartImminent() {
            return fartTimer - Time.time < fartWarningDuration / 4f;
        }

        private float HowFarty() {
            return (fartTimer - Time.time) / fartWarningDuration;
        }

        private void ResetPlayer() {
            if (rigidbody.isKinematic && spineRigidbody.velocity.magnitude > 0.1f) {
                deadTimer = Time.time + 1.0f;
            }

            if (deadTimer > 0.0f && deadTimer - Time.time < 0.0f) {
                deadTimer = 0.0f;
                ToggleRagdoll();
                SetFartTimer();
            }
        }

        private void Clench() {
            if (Input.GetKey(KeyCode.Space) && !rigidbody.isKinematic) {
                if (clenchTimer == 0.0f) {
                    UIManager.Instance.ToggleClenchBar(true);
                }

                clenchTimer += Time.deltaTime;
                UIManager.Instance.UpdateClenchBar(Mathf.Clamp01(clenchTimer / maxClenchTimer));
            }

            if (clenchTimer > 0.0f && Input.GetKeyUp(KeyCode.Space)) {
                clenchTimer = 0.0f;

                if (WillFartSoon()) {
                    Fart();
                }

                UIManager.Instance.ToggleClenchBar(false);
            }
        }

        private void SetFartTimer() {
            fartTimer = Time.time + Random.Range(minFartTimer, maxFartTimer);
            fartWarningYellow.SetActive(false);
        }

        private void Fart() {
            float clenchAmount = clenchTimer / maxClenchTimer;
            const float explosionDistance = 5f;
            foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>()) {
                if (rb != rigidbody && !rb.isKinematic) {
                    float distance = Vector3.Distance(rb.position, farticleSystem.transform.position);
                    if (distance < explosionDistance) {
                        rb.AddExplosionForce(1000f + 1000f * clenchAmount, farticleSystem.transform.position - Vector3.up, explosionDistance);
                        if (rb.TryGetComponent(out Prop prop)) {
                            prop.AddChaos(0.25f / (1f + distance / explosionDistance));
                        }
                    }
                }
            }

            if (!rigidbody.isKinematic) {
                ToggleRagdoll();
            }
            //Quaternion.Euler(Random.Range(0.0f, 90.0f), Random.Range(-45.0f, 45.0f), 0.0f)

            Vector3 force = Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), transform.forward) * Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), Vector3.up) * (transform.forward + Vector3.up) * Random.Range(minFartForce, maxFartForce);
            farticleSystem.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -force);
            farticleSystem.Play();
            spineRigidbody.AddForce(force, ForceMode.Impulse);

            fartTimer = 0.0f;
            clenchTimer = 0f;
            UIManager.Instance.ToggleClenchBar(false);
            fartWarningYellow.SetActive(false);
            fartWarningRed.SetActive(false);
            //SetFartTimer();
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

                //spineRigidbody.position = new Vector3(spineRigidbody.position.x, 0.0f, spineRigidbody.position.z);
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
                hasMoved = true;
            } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                forwardDir = -Camera.main.transform.forward;
                hasMoved = true;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                rightDir = Camera.main.transform.right;
                hasMoved = true;
            } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                rightDir = -Camera.main.transform.right;
                hasMoved = true;
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
                Vector3 movementDir = Vector3.RotateTowards(transform.forward.normalized, direction.normalized, 0.5f * Time.deltaTime, 0.0f).normalized;
                rigidbody.velocity = Vector3.Scale(movementDir, new Vector3(currentVelocity,  -Physics.gravity.y, currentVelocity));

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

                    clenchTimer = 0f;
                    UIManager.Instance.ToggleClenchBar(false);
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

