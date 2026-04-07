using EmotesAPI;
using LethalEmotesAPI.Core;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LethalInternship.Core.Interns.AI;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class InternCompat
    {
        public static void SetupInternStartHook()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(InternAI), typeof(InternCompat), "Start", BindingFlags.Public, nameof(Start), StartHook);
        }
        private void Start(Action<InternAI> orig, InternAI self)
        {
            orig(self);
            if (CustomEmotesAPI.localMapper is not null)
            {
                CustomEmotesAPI.localMapper.StartCoroutine(EnemySkeletons.SetupSkeleton(self));
            }
        }
        internal static Hook StartHook;
    }
}
