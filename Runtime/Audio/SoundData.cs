using UnityEngine;
using UnityEngine.Audio;

namespace UnityUtility.Audio
{
    [System.Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup group;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;

        [Range(0,1)]
        public float volume = 1;
    }
}



