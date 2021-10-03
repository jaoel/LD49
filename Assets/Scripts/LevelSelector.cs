using UnityEngine;
using DG.Tweening;

namespace LD49 {
    public class LevelSelector : MonoBehaviour {
        public int index = 0;
        public GameObject poopGameObject;

        private void OnEnable() {
            int hasCompleted = PlayerPrefs.GetInt($"level{index}", 0);
            if (hasCompleted == 0) {
                poopGameObject.SetActive(false);
            } else if (hasCompleted == 1) {
                //PlayerPrefs.SetInt($"level{index}", 2);
                Vector3 targetScale = poopGameObject.transform.localScale;
                Debug.Log(targetScale);
                poopGameObject.transform.localScale = Vector3.zero;
                poopGameObject.transform.DOScale(targetScale, 1f).SetDelay(index * 0.5f).SetEase(Ease.OutElastic);
            }
        }
    }
}
