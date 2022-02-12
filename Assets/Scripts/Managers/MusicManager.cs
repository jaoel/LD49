using UnityEngine;
using FMOD.Studio;

namespace LD49 {
    public class MusicManager : MonoBehaviour {
        private static MusicManager _instance;
        private FMOD.Studio.EventInstance musicInstance;
        private string currentEvent;
        private string queuedEvent;
        private EventInstance eventInstance;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(this);
                Debug.LogWarning("A duplicate MusicController was found");
            }
        }

        private void Start() {
            SetMusicVolume(0.25f);
        }

        private void QueueMusic(string fmodEvent) {
            if (currentEvent != fmodEvent) {
                queuedEvent = fmodEvent;
            }
        }

        private void Update() {
            if (!string.IsNullOrEmpty(queuedEvent) && queuedEvent != currentEvent) {
                if (!eventInstance.hasHandle()) {
                    currentEvent = queuedEvent;
                    queuedEvent = null;

                    eventInstance = FMODUnity.RuntimeManager.CreateInstance(currentEvent);
                    eventInstance.setVolume(1f);
                    eventInstance.start();
                } else {
                    eventInstance.getVolume(out float volume);

                    volume = Mathf.MoveTowards(volume, 0f, Time.deltaTime * 2f);
                    eventInstance.setVolume(volume);

                    if (volume <= 0f) {
                        currentEvent = queuedEvent;
                        queuedEvent = null;

                        eventInstance.release();

                        eventInstance = FMODUnity.RuntimeManager.CreateInstance(currentEvent);
                        eventInstance.start();
                    }
                }

            } else if (eventInstance.hasHandle()) {
                eventInstance.getVolume(out float volume);

                if (volume < 1f) {
                    volume = Mathf.MoveTowards(volume, 1f, Time.deltaTime * 10f);
                    eventInstance.setVolume(volume);
                }
            }
        }

        public static void SetMusicVolume(float volume) {
            volume = Mathf.Clamp01(volume);

            // TODO: Set value in settings and save.
            Bus musicBus = FMODUnity.RuntimeManager.GetBus("Bus:/Music");
            musicBus.setVolume(volume);
        }

        public static void PlayMusic(string fmodEvent) {
            if (_instance != null && !string.IsNullOrEmpty(fmodEvent)) {
                _instance.QueueMusic(fmodEvent);
            }
        }

        public static void PlayMusic(FMODUnity.EventReference eventRef) {
            if (_instance != null && !eventRef.IsNull) {
                _instance.QueueMusic(eventRef.Path);
            }
        }

        public static void PlayMainMenuMusic() {
            if (_instance != null) {
                PlayMusic("event:/Music/MainMenuTheme");
            }
        }

        public static void PlayGameMusic() {
            if (_instance != null) {
                PlayMusic("event:/Music/LevelTheme");
            }
        }
    }
}
