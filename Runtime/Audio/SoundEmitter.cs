using UnityEngine;
using System.Collections;

namespace Utility.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }

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

        public void Initialize(SoundData data)
        {
            Data = data;

            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.group;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;
            audioSource.volume = data.volume;
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
    }
}