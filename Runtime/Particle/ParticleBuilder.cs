using UnityEngine;

namespace Utility.Particle
{
    public class ParticleBuilder
    {
        private readonly ParticleManager manager;
        private ParticleEmitter prefab;
        private Vector3 position = Vector3.zero;
        private Transform parent;

        public ParticleBuilder(ParticleManager manager)
        {
            this.manager = manager;
        }

        public ParticleBuilder WithPrefab(ParticleEmitter prefab)
        {
            this.prefab = prefab;
            return this;
        }

        public ParticleBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public ParticleBuilder WithParent(Transform parent)
        {
            this.parent = parent;
            return this;
        }

        public void Play()
        {
            if (prefab == null)
            {
                Debug.LogWarning("[ParticleBuilder] Missing prefab reference.");
                return;
            }

            var emitter = manager.Get(prefab);
            emitter.transform.position = position;
            emitter.transform.SetParent(parent != null ? parent : manager.transform);
            emitter.Play();
        }
    }
}
