using EmotesAPI;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using LethalEmotesApi.Ui;
using LethalEmotesAPI.Utils;

namespace LethalEmotesAPI.Patches;

public static class PlayerControllerPatches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public static class UpdatePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            //new CodeMatch(code => code.Calls(AccessTools.Method(typeof(IngamePlayerSettings), "get_Instance"))),
            //    new CodeMatch(code => code.LoadsField(AccessTools.Field(typeof(IngamePlayerSettings), "playerInput"))),
            matcher.MatchForward(true,
                new CodeMatch(code => code.opcode == OpCodes.Ldarg_0),
                new CodeMatch(code => code.opcode == OpCodes.Call),
                new CodeMatch(code => code.opcode == OpCodes.Ldfld),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Ldstr),
                new CodeMatch(code => code.opcode == OpCodes.Ldc_I4_0),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Stfld),
                new CodeMatch(code => code.opcode == OpCodes.Call),
                new CodeMatch(code => code.opcode == OpCodes.Ldfld),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Ldstr),
                new CodeMatch(code => code.opcode == OpCodes.Ldc_I4_0),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Stloc_0)
            );

            matcher
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomEmotesAPI), nameof(CustomEmotesAPI.AutoWalking), new[] { typeof(PlayerControllerB) })));

            return matcher.InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "Interact_performed")]
    public static class InteractPerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "ItemSecondaryUse_performed")]
    public static class ItemSecondaryUsePerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "ItemTertiaryUse_performed")]
    public static class ItemTertiaryUsePerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "ActivateItem_performed")]
    public static class ActivateItemPerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "ScrollMouse_performed")]
    public static class ScrollMousePerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "InspectItem_performed")]
    public static class InspectItemPerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions, generator)
                .ReturnIfInEmoteUi()
                .InstructionEnumeration();
        }
    }
}