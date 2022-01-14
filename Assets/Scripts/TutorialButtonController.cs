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

        // Song is 69 bpm (nice)
        private const float beatTime = 1f / 69f * 60f / 2f;

        private void Start() {
            offset = Random.Range(0f, 100f);
            Sequence buttonsSequence = DOTween.Sequence();

            for (int i = 0; i < buttons.Count; i++) {
                float timeLeft = beatTime;
                GameObject button = buttons[i];
                buttonsSequence.Append(button.transform.DOScale(new Vector3(1f, 0.5f, 1f), 0.1f).SetEase(Ease.OutElastic));
                timeLeft -= 0.1f;

                if (holdLonger) {
                    buttonsSequence.AppendInterval(beatTime * 2f);
                }

                buttonsSequence.Append(button.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f).SetEase(Ease.OutBounce));
                timeLeft -= 0.25f;

                buttonsSequence.AppendInterval(timeLeft);
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
