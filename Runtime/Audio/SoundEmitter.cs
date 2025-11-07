using UnityEngine;
using System.Collections;

namespace UnityUtility.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData data { get; private set; }

        private AudioSource audioSource;
        private Coroutine coroutine;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void Play()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            audioSource.Play();
            coroutine = StartCoroutine(WaitForSoundToEnd());
        }

        public void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            SoundManager.Instance.ReturnToPool(this);
        }

        public void Initialize(SoundData newData)
        {
            data = newData;

            audioSource.clip = newData.clip;
            audioSource.loop = newData.loop;
            audioSource.pitch = newData.pitch;
            audioSource.volume = newData.volume;
            audioSource.playOnAwake = newData.playOnAwake;
            audioSource.outputAudioMixerGroup = newData.group;
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
    }
}