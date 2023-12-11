using EmotesAPI;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace LethalEmotesAPI.Patches
{
    public static class PlayerControllerPatches
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        public static class UpdatePatcher
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var matcher = new CodeMatcher(instructions);

                matcher.MatchForward(true,
                    new CodeMatch(code => code.IsLdarg(0)),
                    new CodeMatch(code => code.Calls(AccessTools.Method(typeof(IngamePlayerSettings), "get_Instance"))),
                    new CodeMatch(code => code.LoadsField(AccessTools.Field(typeof(IngamePlayerSettings), "playerInput"))),
                    new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                    new CodeMatch(code => code.opcode == OpCodes.Ldstr),
                    new CodeMatch(code => code.opcode == OpCodes.Ldc_I4_0),
                    new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                    new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                    new CodeMatch(code => code.opcode == OpCodes.Stfld)
                    );

                matcher
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomEmotesAPI), nameof(CustomEmotesAPI.AutoWalking), new[] { typeof(PlayerControllerB) })));

                return matcher.InstructionEnumeration();
            }
        }
    }
}
