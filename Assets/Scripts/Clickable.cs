using UnityEngine;
using UnityEngine.Events;

namespace LD49 {
    public class Clickable : MonoBehaviour {
        public UnityEvent Clicked = new UnityEvent();

        private void OnMouseUpAsButton() {
            Clicked?.Invoke();
        }
    }
}
