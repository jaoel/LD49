using UnityEngine;

namespace LD49 {
    public class MusicController : MonoBehaviour {
        [SerializeField]
        private AudioClip menuMusic;

        [SerializeField]
        private AudioClip gameMusic;

        [SerializeField]
        private AudioSource musicAudioSource;

        private AudioClip currentClip;
        private AudioClip queuedClip;

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

        public void PlayMainMenuMusic() {
            QueueMusic(menuMusic);
        }

        public void PlayGameMusic() {
            QueueMusic(gameMusic);
        }
    }
}
