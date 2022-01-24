using UnityEngine;
using UnityEngine.Events;

namespace LD49 {
    public class PlayerRagdollTrigger : MonoBehaviour {
        public ActivePlayer player;
        public float collisionRagdollCooldown = 2f;

        private float exitRagdollTime = 0f;
        private bool CanRagdollFromCollision => Time.time - exitRagdollTime > collisionRagdollCooldown;

        private void Update() {
            if (player.IsRagdoll) {
                exitRagdollTime = Time.time;
            }
        }

        private void OnTriggerEnter(Collider other) {
            int layerMask = Layers.GetMask(Layers.Props, Layers.Wall) & Layers.GetMask(other.gameObject.layer);
            if (layerMask != 0 && CanRagdollFromCollision) {
                player.SetRagdoll(true);
                
                PlayHitSound();

                Vector3 randomVector = Random.onUnitSphere;
                randomVector.y = Mathf.Abs(randomVector.y) + 3f;

                Vector3 vectorToOther = player.spine1.position - other.ClosestPoint(player.spine1.position);
                randomVector = Quaternion.FromToRotation(Vector3.up, vectorToOther.normalized) * randomVector + Vector3.up;

                randomVector.Normalize();

                player.AddImpulseToBody(randomVector * 50f);
            }
        }

        private void PlayHitSound() {
            // TODO: Make actual hit sounds
            FMODUnity.RuntimeManager.PlayOneShot("event:/Props/PropCollisionDefault", transform.position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/People/Scream");
        }
    }
}
