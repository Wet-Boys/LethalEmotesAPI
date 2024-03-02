using EmotesAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LethalEmotesAPI.Patches.ModCompat
{
    internal class AdvancedCompanyCompat
    {
        public static void SetupUpdateVisibilityHook()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(AdvancedCompany.Game.MobileTerminal), typeof(AdvancedCompanyCompat), "Open", BindingFlags.Public, nameof(Open), OpenHook);
        }
        private void Open(Action<AdvancedCompany.Game.MobileTerminal> orig, AdvancedCompany.Game.MobileTerminal self)
        {
            if (self.Player.Controller)
            {
                CustomEmotesAPI.PlayAnimation("none", BoneMapper.playersToMappers[self.Player.Controller.gameObject]);
            }
            orig(self);
        }
        internal static Hook OpenHook;
    }
}
