using UnityEngine;

namespace LD49 {
    public class FarticleSystem : MonoBehaviour {
        public FMODUnity.EventReference fartEvent = new FMODUnity.EventReference();

        public ParticleSystem[] particleSystems;

        private void Awake() {
            if (fartEvent.IsNull) {
                fartEvent = FMODUnity.RuntimeManager.PathToEventReference("event:/Farts/Fart_Default");
            }
        }

        public void Fart() {
            foreach (var ps in particleSystems) {
                ps.Play();
            }

            FMODUnity.RuntimeManager.PlayOneShotAttached(fartEvent.Guid, gameObject);
        }
    }
}
