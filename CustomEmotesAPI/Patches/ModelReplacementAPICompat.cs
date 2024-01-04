using BepInEx.Bootstrap;
using EmotesAPI;
using GameNetcodeStuff;
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

    }
}
