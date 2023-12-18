using System.Reflection.Emit;
using HarmonyLib;
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
                AccessTools.Method(typeof(PatchUtils), nameof(EmoteWheelsOpen))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, origLocLabel));

        if (returnABool)
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1));
        
        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ret));

        return matcher;
    }
    
    internal static CodeMatcher CloseEmoteUi(this CodeMatcher matcher)
    {
        matcher.Advance(1);
        matcher.CreateLabel(out var origLocLabel);
        
        matcher
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(PatchUtils), nameof(EmoteWheelsOpen))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, origLocLabel))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EmoteUiManager), nameof(EmoteUiManager.CloseUiGracefully))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ret));

        return matcher;
    }

    private static bool EmoteWheelsOpen()
    {
        return EmoteUiManager.IsEmoteWheelsOpen() || EmoteUiManager.IsCustomizePanelOpen();
    }
}