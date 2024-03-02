using EmotesAPI;
using LethalVRM;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class VRMCompat
    {
        public static void SetupUpdateVisibilityHook()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(LethalVRMManager.LethalVRMInstance), typeof(VRMCompat), "UpdateVisibility", BindingFlags.Public, nameof(UpdateVisibility), UpdateVisibilityHook);
        }
        private void UpdateVisibility(Action<LethalVRMManager.LethalVRMInstance> orig, LethalVRMManager.LethalVRMInstance self)
        {
            bool doAfter = false;
            if (CustomEmotesAPI.localMapper.isInThirdPerson && CustomEmotesAPI.localMapper.playerController.gameplayCamera.enabled)
            {
                doAfter = true;
                CustomEmotesAPI.localMapper.playerController.gameplayCamera.enabled = false;
            }
            orig(self);
            if (doAfter)
            {
                CustomEmotesAPI.localMapper.playerController.gameplayCamera.enabled = true;
            }
        }
        internal static Hook UpdateVisibilityHook;
    }
}
