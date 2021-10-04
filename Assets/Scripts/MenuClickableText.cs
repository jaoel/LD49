using UnityEngine;
using UnityEngine.Events;

namespace LD49 {
    public class MenuClickableText : MonoBehaviour {
        public GameObject textObject;
        public UnityEvent Clicked = new UnityEvent();

        private bool over = false;
        private float jumpAnim = 1f;

        private void OnDisable() {
            over = false;
            jumpAnim = 1f;
        }

        private void OnMouseOver() {
            if (!over) {
                GameManager.Instance.PlayUIHover();
                jumpAnim = 0f;
            }
            over = true;
        }

        private void OnMouseExit() {
            over = false;
        }

        private void OnMouseUpAsButton() {
            GameManager.Instance.PlayUIClick();
            Clicked?.Invoke();
        }

        private void Update() {
            jumpAnim = jumpAnim += Time.deltaTime * 0.75f;
            float x = Mathf.Min(jumpAnim, 1f);
            float anim = Mathf.Abs(Mathf.Sin(x * 4f * Mathf.PI) * (1f - x));
            textObject.transform.localPosition = Vector3.up * anim;
            if (over) {
                textObject.transform.localScale = Vector3.one * 1.25f;
                if (jumpAnim >= 1.5f) {
                    jumpAnim = 0f;
                }
            } else {
                textObject.transform.localScale = Vector3.one * 1f;
            }
        }
    }
}
