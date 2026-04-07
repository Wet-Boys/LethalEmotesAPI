using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class Why
    {
        internal static Dictionary<object, Vector3> originalBodyScales = new();
        internal static Dictionary<PlayerControllerB, object> playersToVRMInstances = new();

        internal static GameObject iWantToSeeYourManager;
    }
}
