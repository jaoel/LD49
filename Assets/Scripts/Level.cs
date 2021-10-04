using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace LD49 {
    public class Level : MonoBehaviour {
        public Transform startPosition;
        //public PlayerTrigger endTrigger;

        [SerializeField]
        protected List<PlayerTrigger> playerTriggers = null;

        [SerializeField]
        protected List<string> objectiveText = null;

        private int currentObjective = 0;

        private void OnEnable() {
            //endTrigger.PlayerEnter +=
            //OnPlayerTriggerEnd;
            currentObjective = 0;

            if (playerTriggers != null && playerTriggers.Count > 0) {
                playerTriggers[currentObjective].PlayerEnter += OnPlayerTriggerEnd;

            }

        }

        private void OnDisable() {
            //endTrigger.PlayerEnter -= OnPlayerTriggerEnd;

            if (playerTriggers != null && playerTriggers.Count > 0) {
                playerTriggers?.ForEach(x => {
                    x.PlayerEnter -= OnPlayerTriggerEnd;
                });
            }


        }

        private void Start() {
            Player player = FindObjectOfType<Player>();
            if (player != null) {
                player.transform.position = startPosition.position;
                player.transform.eulerAngles = new Vector3(0f, startPosition.eulerAngles.y, 0f);
            }

            if (objectiveText != null && objectiveText.Count > 0) {
                UIManager.Instance.ShowObjective(objectiveText[currentObjective]);
            }
        }

        private void OnPlayerTriggerEnd() {

            if (playerTriggers != null && playerTriggers.Count > 0) {
                playerTriggers[currentObjective++].PlayerEnter -= OnPlayerTriggerEnd;
            }

            if (currentObjective >= playerTriggers.Count) {
                LevelManager.Instance.RecordCurrentLevelWin();
                LevelManager.Instance.RequestNextLevel();
                //U WIN LMAO
            } else {
                playerTriggers[currentObjective].PlayerEnter += OnPlayerTriggerEnd;
                UIManager.Instance.ShowObjective(objectiveText[currentObjective]);
            }
        }
    }
}
