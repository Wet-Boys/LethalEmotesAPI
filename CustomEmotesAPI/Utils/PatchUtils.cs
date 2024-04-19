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
        
        matcher
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(PatchUtils), nameof(EmoteWheelsOpen))));

        matcher.Instruction.MoveLabelsTo(matcher.InstructionAt(-1));
        matcher.CreateLabel(out var origLocLabel);
        
        matcher
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, origLocLabel))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EmoteUiManager), nameof(EmoteUiManager.CloseUiGracefully))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ret));

        return matcher;
    }

    private static bool EmoteWheelsOpen()
    {
        return EmoteUiManager.IsEmoteUiOpen();
    }

    /// <summary>
    /// Helper method that prints out the IL instructions currently present in the CodeMatcher
    /// </summary>
    /// <param name="matcher"></param>
    /// <returns></returns>
    internal static CodeMatcher WriteToLog(this CodeMatcher matcher)
    {
        var pos = matcher.Pos;

        foreach (var code in matcher.InstructionEnumeration())
            DebugClass.Log(code.ToString());

        matcher.Start();
        matcher.Advance(pos);

        return matcher;
    }
}