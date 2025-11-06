using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Utility.Particle
{
    public class ParticleManager : GenericSingleton<ParticleManager>
    {
        private readonly Dictionary<ParticleEmitter, IObjectPool<ParticleEmitter>> pools = new();

        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;

        protected override void Awake()
        {
            base.Awake();
            //pre-init-pool
        }

        public ParticleBuilder CreateParticle()
        {
            return new ParticleBuilder(this);
        }

        private IObjectPool<ParticleEmitter> GetOrCreatePool(ParticleEmitter prefab)
        {
            if (!pools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool<ParticleEmitter>(
                    () => CreateParticleEmitter(prefab),
                    OnTakeFromPool,
                    OnReturnToPool,
                    OnDestroyPoolObject,
                    collectionCheck,
                    defaultCapacity,
                    maxPoolSize
                );

                pools.Add(prefab, pool);
            }

            return pool;
        }

        private ParticleEmitter CreateParticleEmitter(ParticleEmitter prefab)
        {
            var emitter = Instantiate(prefab);
            emitter.gameObject.SetActive(false);
            return emitter;
        }

        public ParticleEmitter Get(ParticleEmitter prefab)
        {
            return GetOrCreatePool(prefab).Get();
        }

        public void ReturnToPool(ParticleEmitter emitter)
        {
            foreach (var kvp in pools)
            {
                if (emitter.name.StartsWith(kvp.Key.name))
                {
                    kvp.Value.Release(emitter);
                    return;
                }
            }

            Destroy(emitter.gameObject); // fallback
        }

        private void OnTakeFromPool(ParticleEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
        }

        private void OnReturnToPool(ParticleEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(ParticleEmitter emitter)
        {
            Destroy(emitter.gameObject);
        }
    }
}
