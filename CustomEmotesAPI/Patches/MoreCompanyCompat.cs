using MoreCompany;
using MoreCompany.Cosmetics;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Patches
{
    internal class MoreCompanyCompat
    {
        internal static bool TurnOnCosmetics(BoneMapper mapper)
        {
            //copied this from https://github.com/notnotnotswipez/MoreCompany/blob/master/MoreCompany/MimicPatches.cs#L32
            Transform cosmeticRoot = mapper.basePlayerModelAnimator.transform;
            CosmeticApplication cosmeticApplication = cosmeticRoot.GetComponent<CosmeticApplication>();
            List<string> cosmetics = CosmeticRegistry.locallySelectedCosmetics;
            if (cosmeticApplication)
            {
                cosmeticApplication.ClearCosmetics();
                GameObject.Destroy(cosmeticApplication);
            }

            cosmeticApplication = cosmeticRoot.gameObject.AddComponent<CosmeticApplication>();
            foreach (var cosmetic in cosmetics)
            {
                cosmeticApplication.ApplyCosmetic(cosmetic, true);
            }

            foreach (var cosmetic in cosmeticApplication.spawnedCosmetics)
            {
                cosmetic.transform.localScale *= CosmeticRegistry.COSMETIC_PLAYER_SCALE_MULT;
            }
            return true;
        }
        internal static void TurnOffCosmetics(BoneMapper mapper)
        {
            Transform cosmeticRoot = mapper.basePlayerModelAnimator.transform;
            CosmeticApplication cosmeticApplication = cosmeticRoot.GetComponent<CosmeticApplication>();
            if (cosmeticApplication)
            {
                cosmeticApplication.ClearCosmetics();
            }
        }
    }
}
