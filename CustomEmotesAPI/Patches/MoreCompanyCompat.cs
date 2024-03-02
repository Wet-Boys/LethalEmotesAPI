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
            CosmeticApplication[] cosmeticApplications = cosmeticRoot.GetComponents<CosmeticApplication>();
            foreach (var item in cosmeticApplications)
            {
                if (item)
                {
                    foreach (var cosmetic in item.spawnedCosmetics)
                    {
                        cosmetic.gameObject.layer = 23;//I hate layer 23 so much but everyone does it cause mirror mod does it so I guess I fall in line :/
                        foreach (var t in cosmetic.gameObject.GetComponentsInChildren<Transform>())
                        {
                            t.gameObject.layer = 23;
                        }
                    }
                }
            }

        }
    }
}
