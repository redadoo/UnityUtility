using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace Utility.Audio
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitter = new();
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

        [SerializeField] private SoundEmitter soundEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxSoundInstaces = 30;

        private void OnEnable()
        {
            InitializePool();
        }

        public SoundBuilder CreateSound() =>
            new(this);

        public bool CanPlaySound(SoundData data)
        {
            if (data.frequentSound) return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstaces && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
            {
                try
                {
                    soundEmitter.Stop();
                    return true;
                }
                catch (System.Exception)
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            soundEmitterPool.Release(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }


        void OnReturnToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitter.Remove(soundEmitter);
        }


        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitter.Add(soundEmitter);
        }


        SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }


        private void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }
    }



}