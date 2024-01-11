using EmotesAPI;
using GameNetcodeStuff;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.SocialPlatforms;

namespace LethalEmotesAPI.Patches
{
    internal class CentipedePatches
    {
        private void ClingToPlayer(Action<CentipedeAI, PlayerControllerB> orig, CentipedeAI self, PlayerControllerB player)
        {
            orig(self, player);
            BoneMapper b = BoneMapper.playersToMappers[self.clingingToPlayer];
            b.canThirdPerson = false;
            if (b.isInThirdPerson)
            {
                b.UnlockCameraStuff();
                b.LockCameraStuff(false);
            }
        }
        private static Hook ClingToPlayerHook;

        private void StopClingingToPlayer(Action<CentipedeAI, bool> orig, CentipedeAI self, bool dead)
        {
            try
            {
                BoneMapper b = BoneMapper.playersToMappers[self.clingingToPlayer];
                b.canThirdPerson = true;
                if ((b.temporarilyThirdPerson == TempThirdPerson.on) || (b.local && b.ThirdPersonCheck()))
                {
                    b.UnlockCameraStuff();
                    b.LockCameraStuff(true);
                }
            }
            catch (Exception e)
            {
                DebugClass.Log($"couldn't find player when removing a snare flea: {e}");
            }
            orig(self, dead);
        }
        private static Hook StopClingingToPlayerHook;
        private static void PatchClingToPlayer()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(CentipedeAI), typeof(CentipedePatches), "ClingToPlayer", BindingFlags.NonPublic, nameof(ClingToPlayer), ClingToPlayerHook);
        }
        private static void PatchStopClingingToPlayer()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(CentipedeAI), typeof(CentipedePatches), "StopClingingToPlayer", BindingFlags.NonPublic, nameof(StopClingingToPlayer), StopClingingToPlayerHook);
        }
        internal static void PatchAll()
        {
            PatchClingToPlayer();
            PatchStopClingingToPlayer();
        }
    }
}
