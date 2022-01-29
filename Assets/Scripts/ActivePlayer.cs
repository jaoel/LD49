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

        private Rigidbody[] rigidbodies = new Rigidbody[0];
        private JointInfo[] jointInfos = new JointInfo[0];

        private float avgSpeed = 0f;
        private Vector3 targetForward;
        private float runSpeed = 0f;
        private FarticleSystem farticleSystem;
        private float clenchAmount = 0f;


        [SerializeField, Tooltip("The amount of time in seconds while ragdolling (and stationary) until ragdolling can stop.")]
        private float ragdollDuration = 1f;
        private float ragdollStartTime = 0f;

        public bool IsRagdoll { get; private set; } = false;
        public bool IsRagdollBecauseFart { get; private set; } = false;
        public bool HasMovedSinceSpawn { get; private set; } = false;
        public bool IsStationary => avgSpeed <= 0.2f;

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
            Vector3 inputVector = GetInputVector();

            if (inputVector.magnitude > 0.1f) {
                HasMovedSinceSpawn = true;

                targetForward = Vector3.RotateTowards(targetForward, inputVector, Mathf.PI * Time.deltaTime, 0f);
                runSpeed = Mathf.MoveTowards(runSpeed, 1f, acceleration * Time.deltaTime);
            } else {
                runSpeed = Mathf.MoveTowards(runSpeed, 0f, deceleration * Time.deltaTime);
            }
            animator.SetFloat("RunSpeed", runSpeed);


            if (IsRagdoll && Time.time >= ragdollStartTime + ragdollDuration && IsStationary && !ChaosManager.IsDead) {
                SetRagdoll(false);
            }
        }

        private void FixedUpdate() {
            if (!IsRagdoll) {
                foreach (var jointInfo in jointInfos) {
                    AntiStuck(jointInfo);
                    jointInfo.joint.SetTargetRotationLocal(jointInfo.source.localRotation, jointInfo.initialRotation);
                }

                // localUp is used to tilt the character back a little when idle, and forward when running
                // it's to compensate for the rest pose of the spine bones being a little bit tilted.
                Vector3 localUp = new Vector3(0f, 1f, (1f - runSpeed) * 0.15f).normalized;
                spine1.AddRelativeTorque(Vector3.Cross(localUp, spine1.transform.InverseTransformDirection(Vector3.up)) * uprightTorque, ForceMode.VelocityChange);
                spine2.AddRelativeTorque(Vector3.Cross(localUp, spine1.transform.InverseTransformDirection(Vector3.up)) * uprightTorque, ForceMode.VelocityChange);

                Vector3 flatForward = spine1.transform.forward;
                flatForward.y = 0f;
                flatForward.Normalize();

                Vector3 flatTargetForward = targetForward;
                flatTargetForward.y = 0f;
                flatTargetForward.Normalize();

                Vector3 torqueToApply = Vector3.Cross(flatForward, flatTargetForward);

                // Apply more torque if the current angular velocity is in the opposite direction of where we want to turn
                // to avoid the spring effect
                float torqueDirection = Vector3.Dot(spine1.angularVelocity, torqueToApply);
                if (torqueDirection < 0f) {
                    torqueToApply *= Mathf.Abs(torqueDirection) * 2.5f;
                }

                spine1.AddTorque(torqueToApply * forwardTorque);

                Vector3 targetVelocity = flatTargetForward * forwardSpeed * runSpeed;
                Vector3 appliedVelocity = targetVelocity - spine1.velocity;

                spine1.AddForce(appliedVelocity * Mathf.Max(0f, Vector3.Dot(spine1.transform.forward, flatTargetForward)), ForceMode.VelocityChange);
            }

            avgSpeed = Mathf.Lerp(avgSpeed, spine1.velocity.magnitude, 0.1f);

            animator.speed = Mathf.Min(0.5f + avgSpeed * 0.5f, 2f);
        }

        private bool initialInputReceived = false;
        private Vector3 GetInputVector() {

            Camera mainCamera = Camera.main;
            Vector3 forward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;
            Vector3 right = mainCamera != null ? mainCamera.transform.right : Vector3.right;
            Vector3 inputVector = right * Input.GetAxisRaw("Horizontal") + forward * Input.GetAxisRaw("Vertical");
            inputVector.y = 0f;
            inputVector.Normalize();

            if (!initialInputReceived && inputVector.magnitude > 0.1f) {
                return Vector3.zero;
            }
            initialInputReceived = true;
            return inputVector;
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

        public void Fart(float strength) {
            SetRagdoll(true);
            IsRagdollBecauseFart = true;

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

            Vector3 fartDirection =
                Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), spine1.transform.forward) *
                Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), Vector3.up) *
                Vector3.up * 20f;

            Vector3 force = fartDirection.normalized * strength;

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
            if (IsRagdoll && !ragdollEnabled) {
                IsRagdollBecauseFart = false;
                Vector3 standingForce = Vector3.up * 100f;
                AddImpulseToBody(standingForce);
            } else {
                ragdollStartTime = Time.time;
            }

            IsRagdoll = ragdollEnabled;
        }

        public void AddImpulseToBody(Vector3 standingForce) {
            foreach (var rb in rigidbodies) {
                rb.AddForce(standingForce / rigidbodies.Length, ForceMode.Impulse);
            }
        }
    }
}

