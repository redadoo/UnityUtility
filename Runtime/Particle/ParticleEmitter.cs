using UnityEngine;
using System.Collections;

namespace UnityUtility.Particle
{
    public class ParticleEmitter : MonoBehaviour
    {
        private Coroutine coroutine;
        private ParticleSystem system;

        public bool frequent;

        private void Awake()
        {
            if (system == null )
                system = GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            system.Play();
            coroutine = StartCoroutine(WaitForParticleToFinish());
        }

        public void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleManager.Instance.ReturnToPool(this);
        }

        IEnumerator WaitForParticleToFinish()
        {
            yield return new WaitWhile(() => system != null && system.IsAlive(true));

            if (ParticleManager.Instance != null)
                ParticleManager.Instance.ReturnToPool(this);
        }
    }
}