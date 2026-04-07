using AdvancedCompany.Game;
using BepInEx.Bootstrap;
using EmotesAPI;
using GameNetcodeStuff;
using LethalEmotesAPI.Patches.ModCompat;
using ModelReplacement;
using ModelReplacement.Modules;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Patches
{
    internal class ModelReplacementAPICompat
    {
        public static void SetupViewStateHook()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(ViewStateManager), typeof(ModelReplacementAPICompat), "GetViewState", BindingFlags.Public, nameof(GetViewState), GetViewStateHook);
        }
        private ViewState GetViewState(Func<ViewStateManager, ViewState> orig, ViewStateManager self)
        {
            if (CustomEmotesAPI.localMapper is not null && CustomEmotesAPI.localMapper.isInThirdPerson)
            {
                return ViewState.ThirdPerson;
            }
            return orig(self);
        }
        internal static Hook GetViewStateHook;
        internal static void ScaleModelAsMapper(Vector3 currentScale, Vector3 ogScale, PlayerControllerB player)
        {
            BodyReplacementBase bod;
            ModelReplacementAPI.GetPlayerModelReplacement(player, out bod);
            if (bod is not null)
            {
                if (!Why2.originalBodyScales.ContainsKey(bod.gameObject))
                {
                    Why2.originalBodyScales.Add(bod.gameObject, bod.replacementModel.transform.localScale);
                }
                Vector3 scaleVector = new Vector3(currentScale.x / ogScale.x, currentScale.y / ogScale.y, currentScale.z / ogScale.z);
                bod.replacementModel.transform.localScale = new Vector3(Why2.originalBodyScales[bod.gameObject].x * scaleVector.x, Why2.originalBodyScales[bod.gameObject].y * scaleVector.y, Why2.originalBodyScales[bod.gameObject].z * scaleVector.z);
            }
        }
    }
}
