using System;
using System.Collections.Generic;
using System.Text;

namespace LethalEmotesAPI.ImportV2
{
    public class EmoteDatabase
    {
        //CustomAnimationClip needs a bool to check if it was imported with the new system, otherwise I think we can keep using it fine?
        //public static Dictionary<string, CustomAnimationClip> importedEmotes = new Dictionary<string, CustomAnimationClip>();
        //what an emote database needs

        //dictionary of identifiers to CustomAnimationClips
        //an identifier is like so: importingModGUID_EmoteName




        /*
        Key differences in how the normal flow works:


        As a safety feature, upon loading, the saved wheels should be checked for items that don't have the GUID/modname noted and if so, find the first emote with the base name and append whatever mod name to it.
        The UI needs to be setup to display the mod name of any emote
        When the UI calls play emote, it needs to call for importingModGUID_EmoteName, and then from backend, if importingModGUID_EmoteName doesn't exist in the emote list, try just EmoteName

        when CustomEmotesAPI.Changed would be called, check if the CustomAnimationClip is using the new system, if so, pass in importingModGUID_EmoteName instead of just EmoteName
         */
    }
}
