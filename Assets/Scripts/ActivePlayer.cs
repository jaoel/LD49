using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LD49 {
    public class ActivePlayer : MonoBehaviour {
        class JointInfo {
            public Transform source;
            public ConfigurableJoint joint;
            public Vector3 initialPosition;
            public Quaternion initialRotation;
            public JointDrive initialAngularXDrive;
            public JointDrive initialAngularYZDrive;
            public int stuckFrames = 0;
        }

        public Animator animator;

        public Transform sourceRoot;
        public Rigidbody spine1;
        public Rigidbody spine2;

        public GameObject eyes;
        public Transform farticleSystemHolder;

        public float uprightTorque = 2000f;
        public float forwardTorque = 150f;
        public float forwardSpeed = 3f;
        public float acceleration = 15f;
        public float deceleration = 10f;

        [SerializeField]
        private float minFartForce = 50.0f;

        [SerializeField]
        private float maxFartForce = 100.0f;

        private Rigidbody[] rigidbodies = new Rigidbody[0];
        private JointInfo[] jointInfos = new JointInfo[0];

        private float avgSpeed = 0f;
        private Vector3 targetForward;
        private float runSpeed = 0f;
        private bool isRagdoll = false;
        private FarticleSystem farticleSystem;
        private float clenchAmount = 0f;

        private void Awake() {
            rigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (var rigidbody in rigidbodies) {
                rigidbody.solverIterations = 20;
                rigidbody.solverVelocityIterations = 20;
                rigidbody.maxAngularVelocity = 200f;
            }

            foreach (var collider1 in GetComponentsInChildren<Collider>()) {
                foreach (var collider2 in GetComponentsInChildren<Collider>()) {
                    Physics.IgnoreCollision(collider1, collider2);
                }
            }

            var configurableJoints = GetComponentsInChildren<ConfigurableJoint>();
            jointInfos = new JointInfo[configurableJoints.Length];
            for (int i = 0; i < configurableJoints.Length; i++) {
                var joint = configurableJoints[i];
                jointInfos[i] = new JointInfo() {
                    source = sourceRoot.FindRecursive(joint.name),
                    joint = joint,
                    initialPosition = joint.transform.localPosition,
                    initialRotation = joint.transform.localRotation,
                    initialAngularXDrive = joint.angularXDrive,
                    initialAngularYZDrive = joint.angularYZDrive,
                };
            }
            targetForward = spine1.transform.forward;

            eyes.SetActive(false);
        }

        private void Start() {
            FarticleSystem farticleSystemPrefab = GameManager.ActiveFarticleSystemPrefab;
            if (farticleSystemPrefab != null) {
                farticleSystem = Instantiate(GameManager.ActiveFarticleSystemPrefab, farticleSystemHolder);
                farticleSystem.transform.localPosition = Vector3.zero;
            }
        }

        private void Update() {
            Camera mainCamera = Camera.main;
            Vector3 inputVector =
                mainCamera.transform.right * Input.GetAxisRaw("Horizontal") +
                mainCamera.transform.forward * Input.GetAxisRaw("Vertical");

            if (inputVector.magnitude > 0.1f) {

                inputVector.y = 0f;
                inputVector.Normalize();

                targetForward = Vector3.RotateTowards(targetForward, inputVector, Mathf.PI * Time.deltaTime, 0f);
                runSpeed = Mathf.MoveTowards(runSpeed, 1f, acceleration * Time.deltaTime);
            } else {
                runSpeed = Mathf.MoveTowards(runSpeed, 0f, deceleration * Time.deltaTime);
            }
            animator.SetFloat("RunSpeed", runSpeed);

            if (Input.GetKeyDown(KeyCode.P)) {
                SetRagdoll(!isRagdoll);
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                Fart();
            }
        }

        private void FixedUpdate() {
            if (!isRagdoll) {
                foreach (var jointInfo in jointInfos) {
                    AntiStuck(jointInfo);
                    jointInfo.joint.SetTargetRotationLocal(jointInfo.source.localRotation, jointInfo.initialRotation);
                }

                // localUp is used to tilt the character back a little when idle, and forward when running
                // it's to compensate for the rest pose of the spine bones being a little bit tilted.
                Vector3 localUp = new Vector3(0f, 1f, (1f - runSpeed) * 0.15f).normalized;
                spine1.AddRelativeTorque(Vector3.Cross(localUp, spine1.transform.InverseTransformDirection(Vector3.up)) * uprightTorque, ForceMode.VelocityChange);
                spine2.AddRelativeTorque(Vector3.Cross(localUp, spine1.transform.InverseTransformDirection(Vector3.up)) * uprightTorque, ForceMode.VelocityChange);

                spine1.AddTorque(Vector3.Cross(spine1.transform.forward, targetForward) * forwardTorque);

                Vector3 targetVelocity = targetForward * forwardSpeed * runSpeed;
                Vector3 appliedVelocity = targetVelocity - spine1.velocity;

                Debug.DrawLine(spine1.position, spine1.position + targetVelocity, Color.blue);
                Debug.DrawLine(spine1.position, spine1.position + appliedVelocity, Color.magenta);

                spine1.AddForce(appliedVelocity * Mathf.Max(0f, Vector3.Dot(spine1.transform.forward, targetForward)), ForceMode.VelocityChange);

                Debug.DrawLine(spine1.position, spine1.position + spine1.velocity, Color.green);

                avgSpeed = Mathf.Lerp(avgSpeed, spine1.velocity.magnitude, 0.1f);

                animator.speed = Mathf.Min(0.5f + avgSpeed * 0.5f, 2f);
            }
        }

        private static void AntiStuck(JointInfo jointInfo) {
            // Teleport joint if it is too far from it's parent
            if (Vector3.Distance(jointInfo.joint.transform.localPosition, jointInfo.initialPosition) > 0.1f) {
                jointInfo.stuckFrames++;
            } else {
                jointInfo.stuckFrames = 0;
            }
            if (jointInfo.stuckFrames > 10) {
                jointInfo.joint.transform.localPosition = jointInfo.initialPosition;
            }
        }

        public void Fart() {
            SetRagdoll(true);

            const float explosionDistance = 5f;
            foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>()) {
                // This should probably be optimized
                if (!rigidbodies.Contains(rb) && !rb.isKinematic) {
                    float distance = Vector3.Distance(rb.position, farticleSystemHolder.position);
                    if (distance < explosionDistance) {
                        rb.AddExplosionForce(1000f + 1000f * clenchAmount, farticleSystemHolder.position - Vector3.up, explosionDistance);
                        if (rb.TryGetComponent(out Prop prop)) {
                            prop.AddChaos(0.25f / (1f + distance / explosionDistance));
                        }
                    }
                }
            }

            foreach (NPC npc in FindObjectsOfType<NPC>()) {
                float dist = Vector3.Distance(npc.transform.position, farticleSystemHolder.position);
                if (dist < 2f) {
                    npc.SetRagdoll(farticleSystemHolder.position, 1000f);
                    ChaosManager.UpdateChaos(50f);
                }
            }

            Vector3 force = 
                Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), spine1.transform.forward) * 
                Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), Vector3.up) * 
                (spine1.transform.forward + Vector3.up * 2f) * 
                Random.Range(minFartForce, maxFartForce);

            if (farticleSystem != null) {
                farticleSystem.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -force);
                farticleSystem.Fart();
            }
            foreach (var rb in rigidbodies) {
                rb.AddForce(force / rigidbodies.Length, ForceMode.Impulse);
            }
        }

        public void SetRagdoll(bool ragdollEnabled) {
            eyes.SetActive(ragdollEnabled);
            foreach (var jointInfo in jointInfos) {
                if (ragdollEnabled) {
                    JointDrive xDrive = jointInfo.joint.angularXDrive;
                    JointDrive yzDrive = jointInfo.joint.angularYZDrive;

                    xDrive.positionSpring = 0f;
                    xDrive.positionDamper = 0f;
                    yzDrive.positionSpring = 0f;
                    yzDrive.positionDamper = 0f;

                    jointInfo.joint.angularXDrive = xDrive;
                    jointInfo.joint.angularYZDrive = yzDrive;
                } else {
                    jointInfo.joint.angularXDrive = jointInfo.initialAngularXDrive;
                    jointInfo.joint.angularYZDrive = jointInfo.initialAngularYZDrive;
                }
            }

            // Give a small push when standing up
            if (isRagdoll && !ragdollEnabled) {
                Vector3 standingForce = Vector3.up * 100f;
                foreach (var rb in rigidbodies) {
                    rb.AddForce(standingForce / rigidbodies.Length, ForceMode.Impulse);
                }
            }

            isRagdoll = ragdollEnabled;
        }
    }
}

