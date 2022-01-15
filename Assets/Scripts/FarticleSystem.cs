using UnityEngine;

namespace LD49 {
    public class FarticleSystem : MonoBehaviour {
        public ParticleSystem[] particleSystems;

        public void Fart() {
            foreach (var ps in particleSystems) {
                ps.Play();
            }
        }
    }
}
