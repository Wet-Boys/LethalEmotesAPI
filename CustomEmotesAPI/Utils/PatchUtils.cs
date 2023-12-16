using System.Reflection.Emit;
using HarmonyLib;
using LethalEmotesAPI.Patches;
using LethalEmotesApi.Ui;

namespace LethalEmotesAPI.Utils;

internal static class PatchUtils
{
    internal static CodeMatcher ReturnIfInEmoteUi(this CodeMatcher matcher, bool returnABool = false)
    {
        matcher.Advance(1);
        matcher.CreateLabel(out var origLocLabel);
        
        matcher
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(PatchUtils), nameof(InEmoteUi))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, origLocLabel));

        if (returnABool)
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1));
        
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ret))
            .AddLabels(new []{ origLocLabel });

        return matcher;
    }

    private static bool InEmoteUi()
    {
        return EmoteWheelManager.InteractionHandler is not null && EmoteWheelManager.InteractionHandler.InEmoteUi();
    }
}