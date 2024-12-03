using GameNetcodeStuff;
using ModelReplacement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class Why2
    {
        internal static Dictionary<BodyReplacementBase, Vector3> originalBodyScales = new Dictionary<BodyReplacementBase, Vector3>();
    }
}
