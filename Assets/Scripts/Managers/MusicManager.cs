using UnityEngine;

namespace LD49 {
    public class MusicManager : MonoBehaviour {
        private static MusicManager _instance;

        [SerializeField]
        private AudioClip menuMusic;

        [SerializeField]
        private AudioClip gameMusic;

        [SerializeField]
        private AudioSource musicAudioSource;

        private AudioClip currentClip;
        private AudioClip queuedClip;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(this);
                Debug.LogWarning("A duplicate MusicController was found");
            }
        }

        private void QueueMusic(AudioClip clip) {
            if (currentClip != clip) {
                queuedClip = clip;
            }
        }

        private void Update() {
            if (queuedClip != null && queuedClip != currentClip) {
                if (musicAudioSource.clip == null) {
                    musicAudioSource.volume = 1f;
                    musicAudioSource.clip = queuedClip;
                    currentClip = queuedClip;
                    queuedClip = null;
                    musicAudioSource.Play();
                } else {
                    musicAudioSource.volume = Mathf.MoveTowards(musicAudioSource.volume, 0f, Time.deltaTime * 2f);
                    if (musicAudioSource.volume <= 0f) {
                        currentClip = queuedClip;
                        musicAudioSource.clip = queuedClip;
                        //musicAudioSource.volume = 1f;
                        musicAudioSource.Play();
                        queuedClip = null;
                    }
                }
            } else if (musicAudioSource.volume < 1f) {
                musicAudioSource.volume = Mathf.MoveTowards(musicAudioSource.volume, 1f, Time.deltaTime * 10f);
            }
        }

        public static void PlayMusic(AudioClip audioClip) {
            if (_instance != null && audioClip != null) {
                _instance.QueueMusic(audioClip);
            }
        }

        public static void PlayMainMenuMusic() {
            if (_instance != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/MainMenuTheme");
                //PlayMusic(_instance.menuMusic);
            }
        }

        public static void PlayGameMusic() {
            if (_instance != null) {
                PlayMusic(_instance.gameMusic);
            }
        }
    }
}
