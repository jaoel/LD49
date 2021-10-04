using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace LD49 {
    public class TutorialButtonController : MonoBehaviour {
        public GameObject textObject;
        public List<GameObject> buttons = new List<GameObject>();

        private Vector3 textOrigPos;

        private void Start() {
            Sequence buttonsSequence = DOTween.Sequence();

            for (int i = 0; i < buttons.Count; i++) {
                GameObject button = buttons[i];
                buttonsSequence.Append(button.transform.DOScale(new Vector3(1f, 0.5f, 1f), 0.1f).SetEase(Ease.OutElastic));
                buttonsSequence.Append(button.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f).SetEase(Ease.OutBounce));
            }

            buttonsSequence.SetLoops(-1);
            buttonsSequence.Play();

            textOrigPos = textObject.transform.localPosition;
        }

        private void Update() {
            textObject.transform.localPosition = textOrigPos + Vector3.forward * Mathf.Sin(Time.time * 2f) * 0.1f;
        }
    }
}
