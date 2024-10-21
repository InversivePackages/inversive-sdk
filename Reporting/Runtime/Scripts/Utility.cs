#if UNITY_2022_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
namespace Reporting
{
    public static class Utility
    {
        public static bool IsXRPresent() {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems) {
                if (xrDisplay.running) {
                    return true;
                }
            }
            return false;
        }
        public static bool IsVR() {
            return IsXRPresent();
        }

    }
}
#endif