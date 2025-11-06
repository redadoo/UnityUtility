using Unity.Cinemachine;
using UnityEngine;

namespace Utility
{
    public class CinemachineUtility 
    {
        private void SetTarget(CinemachineCamera cam, Transform newTarget)
        {
            cam.Target.TrackingTarget = newTarget;
            cam.Target.LookAtTarget = newTarget;
        }
    }

}
