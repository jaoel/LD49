using UnityEngine;
using FMOD.Studio;

namespace LD49 {
    public class MusicManager : MonoBehaviour {
        public FMODUnity.EventReference mainMenuTheme;
        public FMODUnity.EventReference defaultLevelTheme;

        private static MusicManager _instance;
        private FMODUnity.EventReference currentEvent;
        private FMODUnity.EventReference queuedEvent;
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

        private void QueueMusic(FMODUnity.EventReference fmodEvent) {
            if (!fmodEvent.IsNull && currentEvent.Guid != fmodEvent.Guid) {
                queuedEvent = fmodEvent;
            }
        }

        private void Update() {
            if (!queuedEvent.IsNull && queuedEvent.Guid != currentEvent.Guid) {
                if (!eventInstance.hasHandle()) {
                    currentEvent = queuedEvent;
                    queuedEvent = new FMODUnity.EventReference();

                    eventInstance = FMODUnity.RuntimeManager.CreateInstance(currentEvent);
                    eventInstance.setVolume(1f);
                    eventInstance.start();
                } else {
                    eventInstance.getVolume(out float volume);

                    volume = Mathf.MoveTowards(volume, 0f, Time.deltaTime * 2f);
                    eventInstance.setVolume(volume);

                    if (volume <= 0f) {
                        currentEvent = queuedEvent;
                        queuedEvent = new FMODUnity.EventReference();

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

        public static void PlayMusic(FMODUnity.EventReference eventRef) {
            if (_instance != null && !eventRef.IsNull) {
                _instance.QueueMusic(eventRef);
            }
        }

        public static void PlayMainMenuMusic() {
            if (_instance != null) {
                PlayMusic(_instance.mainMenuTheme);
            }
        }

        public static void PlayGameMusic() {
            if (_instance != null) {
                PlayMusic(_instance.defaultLevelTheme);
            }
        }
    }
}
