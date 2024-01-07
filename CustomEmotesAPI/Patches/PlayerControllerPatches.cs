using EmotesAPI;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using LethalEmotesApi.Ui;
using LethalEmotesAPI.Utils;
using UnityEngine;

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
    
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public static class LateUpdatePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            var localVisorFieldInfo =
                AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.localVisor));
            var localVisorTargetPointFieldInfo = AccessTools.Field(typeof(PlayerControllerB),
                nameof(PlayerControllerB.localVisorTargetPoint));

            // localVisor.position = localVisorTargetPoint.position;
            matcher.MatchForward(true,
                new CodeMatch(code => code.IsLdarg(0)),
                new CodeMatch(code => code.LoadsField(localVisorFieldInfo)),
                new CodeMatch(code => code.IsLdarg(0)),
                new CodeMatch(code => code.LoadsField(localVisorTargetPointFieldInfo)),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt));
            
            // localVisor.rotation = Quaternion.Lerp(localVisor.rotation, localVisorTargetPoint.rotation, 53f * Mathf.Clamp(Time.deltaTime, 0.0167f, 20f));
            matcher.MatchForward(true,
                new CodeMatch(code => code.IsLdarg(0)),
                new CodeMatch(code => code.LoadsField(localVisorFieldInfo)),
                new CodeMatch(code => code.IsLdarg(0)),
                new CodeMatch(code => code.LoadsField(localVisorFieldInfo)),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.IsLdarg(0)),
                new CodeMatch(code => code.LoadsField(localVisorTargetPointFieldInfo)),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt),
                new CodeMatch(code => code.opcode == OpCodes.Ldc_R4),
                new CodeMatch(code => code.opcode == OpCodes.Call),
                new CodeMatch(code => code.opcode == OpCodes.Ldc_R4),
                new CodeMatch(code => code.opcode == OpCodes.Ldc_R4),
                new CodeMatch(code => code.opcode == OpCodes.Call),
                new CodeMatch(code => code.opcode == OpCodes.Mul),
                new CodeMatch(code => code.opcode == OpCodes.Call),
                new CodeMatch(code => code.opcode == OpCodes.Callvirt));

            matcher
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, localVisorFieldInfo))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomEmotesAPI), nameof(CustomEmotesAPI.LocalVisor), new[] { typeof(Transform) })));

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

    [HarmonyPatch(typeof(PlayerControllerB), "OpenMenu_performed")]
    public static class OpenMenuPerformedPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var matcher = new CodeMatcher(instructions, generator);
            
            // else if (!base.IsOwner || (!isPlayerControlled && !isPlayerDead) || (base.IsServer && !isHostPlayerObject))
            matcher.MatchForward(true,
                    // (!base.IsOwner ||
                    new CodeMatch(code => code.IsLdarg(0)),
                    new CodeMatch(code => code.opcode == OpCodes.Call),
                    new CodeMatch(code => code.opcode == OpCodes.Brfalse),
                    // (!isPlayerControlled && 
                    new CodeMatch(code => code.IsLdarg(0)),
                    new CodeMatch(code => code.opcode == OpCodes.Ldfld),
                    new CodeMatch(code => code.opcode == OpCodes.Brtrue),
                    // !isPlayerDead) ||
                    new CodeMatch(code => code.IsLdarg(0)),
                    new CodeMatch(code => code.opcode == OpCodes.Ldfld),
                    new CodeMatch(code => code.opcode == OpCodes.Brfalse),
                    // (base.IsServer &&
                    new CodeMatch(code => code.IsLdarg(0)),
                    new CodeMatch(code => code.opcode == OpCodes.Call),
                    new CodeMatch(code => code.opcode == OpCodes.Brfalse),
                    // !isHostPlayerObject))
                    new CodeMatch(code => code.IsLdarg(0)),
                    new CodeMatch(code => code.opcode == OpCodes.Ldfld),
                    new CodeMatch(code => code.opcode == OpCodes.Brtrue),
                    // return;
                    new CodeMatch(code => code.opcode == OpCodes.Ret));
            
            return matcher
                .CloseEmoteUi()
                .InstructionEnumeration();
        } 
    }
}