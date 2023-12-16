using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using LethalEmotesAPI.Utils;

namespace LethalEmotesAPI.Patches;

public static class HUDManagerPatches
{
    [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
    public static class PingScanPerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
    public static class CanPlayerScanPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi(true)
                .InstructionEnumeration();
        }
    }
}