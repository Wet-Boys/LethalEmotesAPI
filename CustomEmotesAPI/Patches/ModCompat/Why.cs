using GameNetcodeStuff;
using LethalVRM;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LethalVRM.LethalVRMManager;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class Why
    {
        internal static Dictionary<LethalVRMInstance, Vector3> originalBodyScales = new Dictionary<LethalVRMInstance, Vector3>();
        internal static Dictionary<PlayerControllerB, LethalVRMInstance> playersToVRMInstances = new Dictionary<PlayerControllerB, LethalVRMInstance>();

        internal static LethalVRMManager iWantToSeeYourManager;
    }
}
