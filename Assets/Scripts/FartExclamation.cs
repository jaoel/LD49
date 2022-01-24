using UnityEngine;

namespace LD49 {
    public class FartExclamation : MonoBehaviour {
        [SerializeField]
        private FartController fartController;

        [SerializeField]
        private Transform followTransform;

        [SerializeField]
        private Transform exclamationTransform;

        [SerializeField]
        private new Renderer renderer;

        [SerializeField]
        private Gradient gradient = new Gradient();

        [SerializeField]
        private AnimationCurve frequencyCurve = new AnimationCurve();

        [SerializeField]
        private AnimationCurve amplitudeCurve = new AnimationCurve();

        private MaterialPropertyBlock mpb;
        private int shaderColorParameterID;
        private float time = 0f;

        private void Awake() {
            mpb = new MaterialPropertyBlock();
            shaderColorParameterID = Shader.PropertyToID("_Color");
        }

        private void Update() {
            float amount = (fartController.FartAmount + fartController.ClenchAmount) * 0.5f;

            if (followTransform != null) {
                transform.position = followTransform.position + Vector3.up;
            }

            if (amount == 0f) {
                renderer.enabled = false;
            } else {
                renderer.enabled = true;

                // Fart wobble
                time += Time.deltaTime * frequencyCurve.Evaluate(amount);
                float scale = 1.0f + Mathf.Sin(time) * amplitudeCurve.Evaluate(amount);
                exclamationTransform.localScale = Vector3.one * scale;

                // Clench shake
                if (fartController.ClenchAmount > 0f) {
                    exclamationTransform.localPosition = new Vector3(
                        2f * Mathf.PerlinNoise(time * 2f, 1f) - 1f,
                        0f,
                        2f * Mathf.PerlinNoise(2f, time * 2f) - 1f) * 0.1f * fartController.ClenchAmount;
                } else {
                    exclamationTransform.localPosition = Vector3.zero;
                }

                mpb.SetVector(shaderColorParameterID, gradient.Evaluate(amount));
                renderer.SetPropertyBlock(mpb);
            }
        }
    }
}

