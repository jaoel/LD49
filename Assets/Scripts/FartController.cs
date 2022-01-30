using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LD49 {
    // Fastna inte i fartkontrollen nu, höhö
    public class FartController : MonoBehaviour {

        [SerializeField]
        private ActivePlayer player;

        [SerializeField, Tooltip("The amount of time in seconds the player can hold a fart in while clenching.")]
        private float clenchDuration = 3f;

        [SerializeField, Tooltip("The amount of time in seconds from the start of feeling farty to actually farting. This timer can be extended by clenching.")]
        private float fartDuration = 2f;

        [SerializeField]
        private float minFartInterval = 2f;

        [SerializeField]
        private float maxFartInterval = 4f;

        [SerializeField]
        private float minFartStrength = 50.0f;

        [SerializeField]
        private float maxFartStrength = 100.0f;

        private bool isGoingToFart = false;
        private float fartTimerStart = 0f;

        private bool isClenching = false;
        private float clenchTimerStart = 0f;

        public float FartAmount => isGoingToFart ? Mathf.Clamp01((Time.time - fartTimerStart) / fartDuration) : 0f;
        public float ClenchAmount => isClenching ? Mathf.Clamp01((Time.time - clenchTimerStart) / clenchDuration) : 0f;

        // cheats
        private bool superSphincter = false;

        private void Update() {
#if UNITY_EDITOR
            DebugControls();
#endif
            if (player.HasMovedSinceSpawn && !player.IsRagdollBecauseFart) {
                UpdateClench();
                UpdateFart();
            } else {
                // Ugly hack, but it works...
                ScheduleNextFart();
            }
        }

        private void UpdateFart() {
            if (Time.time >= fartTimerStart) {
                isGoingToFart = true;
            }

            // Can do exact compare here because clamping
            if (!isClenching && isGoingToFart && !superSphincter && FartAmount == 1f) {
                Fart();
                clenchTimerStart = 0f;
            }
        }

        private void UpdateClench() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                clenchTimerStart = Time.time;
                isClenching = true;
            }

            if (isClenching) {
                // Unclenching will always result in a fart regardless if we're already farting or not
                if (Input.GetKeyUp(KeyCode.Space) || Time.time >= (clenchTimerStart + clenchDuration)) {
                    isClenching = false;
                    isGoingToFart = true;
                    fartTimerStart = Time.time - fartDuration;
                }
            }

            UIManager.UpdateClenchBar(ClenchAmount);
        }

        private void ScheduleNextFart() {
            isGoingToFart = false;
            fartTimerStart = Time.time + Random.Range(minFartInterval, maxFartInterval);
        }

        private void Fart() {
            player.Fart(Random.Range(minFartStrength, maxFartStrength));
            ScheduleNextFart();
        }

        private void DebugControls() {
            if (Input.GetKeyDown(KeyCode.F)) {
                Fart();
            }
            if (Input.GetKeyDown(KeyCode.P)) {
                superSphincter = !superSphincter;
                if (superSphincter) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/People/Scream");
                    player.SetRagdoll(false);
                    isGoingToFart = false;
                    isClenching = false;
                }
            }
        }
    }
}

