using UnityEngine;

namespace LightItUp.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceCtrl : MonoBehaviour {
        AudioSource _audioSource;
        AudioSource audioSource {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                }
                return _audioSource;
            }
        }
        public void PlaySound(SoundManager.AudioData data)
        {
            audioSource.clip = data.clip;
            audioSource.volume = data.volume;
            audioSource.name = data.clipName;
            gameObject.SetActive(true);
            audioSource.Play();
        }
        void Update() {
            if (IsDonePlaying())
            {
                StopSound();
            }
        }
        bool IsDonePlaying() {
            return !audioSource.isPlaying;
        }
        void StopSound() {
            gameObject.SetActive(false);
        }
    }
}
