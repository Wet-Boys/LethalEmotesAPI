using EmotesAPI;
using GameNetcodeStuff;
using LethalVRM;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using static LethalVRM.LethalVRMManager;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class VRMCompat
    {

        internal static void SetManager()
        {
            if (Why.iWantToSeeYourManager is null)
            {
                foreach (var item in Resources.FindObjectsOfTypeAll(typeof(LethalVRMManager)) as LethalVRMManager[])
                {
                    Why.iWantToSeeYourManager = item;
                }
            }
        }
        internal static void GetPlayerInDictionary(PlayerControllerB player)
        {
            if (Why.iWantToSeeYourManager is not null)
            {
                if (!Why.playersToVRMInstances.ContainsKey(player))
                {
                    foreach (var item in Why.iWantToSeeYourManager.instances)
                    {
                        if (item.PlayerControllerB == player)
                        {
                            Why.playersToVRMInstances.Add(item.PlayerControllerB, item);
                        }
                    }
                }
            }
        }

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
        internal static LethalVRMInstance PerformAllChecks(PlayerControllerB player)
        {
            SetManager();
            GetPlayerInDictionary(player);
            if (Why.playersToVRMInstances.ContainsKey(player))
            {
                return Why.playersToVRMInstances[player];
            }
            return null;
        }
        internal static Hook UpdateVisibilityHook;
        internal static void ScaleModelAsMapper(Vector3 currentScale, Vector3 ogScale, PlayerControllerB player)
        {
            LethalVRMInstance bod = PerformAllChecks(player);
            if (bod is not null)
            {
                if (!Why.originalBodyScales.ContainsKey(bod))
                {
                    Why.originalBodyScales.Add(bod, bod.Vrm10Instance.transform.localScale);
                }
                Vector3 scaleVector = new Vector3(currentScale.x / ogScale.x, currentScale.y / ogScale.y, currentScale.z / ogScale.z);
                bod.Vrm10Instance.transform.localScale = new Vector3(Why.originalBodyScales[bod].x * scaleVector.x, Why.originalBodyScales[bod].y * scaleVector.y, Why.originalBodyScales[bod].z * scaleVector.z);
            }
        }
    }
}
