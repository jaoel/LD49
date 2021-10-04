using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace LD49 {
    public class TutorialButtonController : MonoBehaviour {
        public GameObject textObject;
        public List<GameObject> buttons = new List<GameObject>();
        public bool holdLonger = false;

        private Vector3 textOrigPos;
        private float offset;

        private void Start() {
            offset = Random.Range(0f, 100f);
            Sequence buttonsSequence = DOTween.Sequence();

            if (holdLonger) {
                buttonsSequence.AppendInterval(1f);
            }

            for (int i = 0; i < buttons.Count; i++) {
                GameObject button = buttons[i];
                buttonsSequence.Append(button.transform.DOScale(new Vector3(1f, 0.5f, 1f), 0.1f).SetEase(Ease.OutElastic));
                if (holdLonger) {
                    buttonsSequence.AppendInterval(1f);
                }
                buttonsSequence.Append(button.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f).SetEase(Ease.OutBounce));
                if (holdLonger) {
                    buttonsSequence.AppendInterval(0.25f);
                }
            }

            buttonsSequence.SetLoops(-1);
            buttonsSequence.Play();

            textOrigPos = textObject.transform.localPosition;
        }

        private void Update() {
            textObject.transform.localPosition = textOrigPos + Vector3.forward * Mathf.Sin(Time.time * 2f + offset) * 0.1f;
        }
    }
}
