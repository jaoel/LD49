using System;
using UnityEngine;

namespace LD49 {
    public class Level : MonoBehaviour {
        public Transform startPosition;
        public PlayerTrigger endTrigger;

        private void OnEnable() {
            endTrigger.PlayerEnter += OnPlayerTriggerEnd;
        }

        private void OnDisable() {
            endTrigger.PlayerEnter -= OnPlayerTriggerEnd;
        }

        private void Start() {
            Player player = FindObjectOfType<Player>();
            if (player != null) {
                player.transform.position = startPosition.position;
                player.transform.eulerAngles = new Vector3(0f, startPosition.eulerAngles.y, 0f);
            }
        }

        private void OnPlayerTriggerEnd() {
            Debug.Log("You Win!");
        }
    }
}
