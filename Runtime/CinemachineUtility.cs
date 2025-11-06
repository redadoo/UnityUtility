#if CINEMACHINE_PRESENT
using Unity.Cinemachine;
#endif
using UnityEngine;

namespace UnityUtility
{
    public static class CinemachineUtility
    {
#if CINEMACHINE_PRESENT
        public static void SetTarget(CinemachineCamera cam, Transform newTarget)
        {
            if (cam == null) return;
            cam.Target.TrackingTarget = newTarget;
            cam.Target.LookAtTarget = newTarget;
        }
#else
        public static void SetTarget(object cam, Transform newTarget)
        {
            Debug.LogWarning("Cinemachine non è presente nel progetto. Installa il pacchetto Cinemachine per usare questa funzione.");
        }
#endif
    }
}
