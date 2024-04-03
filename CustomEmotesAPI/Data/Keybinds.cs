using EmotesAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.InputSystem;

namespace LethalEmotesAPI.Data
{
    public static class Keybinds
    {
        public static Dictionary<string, string> keyBindOverrideStorage = new Dictionary<string, string>();//stores all emotes that have ever been loaded with their corresponding input path thing
        internal static Dictionary<string, InputActionReference> inputRefs = new Dictionary<string, InputActionReference>();//all the currently loaded emotes have an InputActionReference which you can get here
        public static void LoadKeybinds()
        {
            keyBindOverrideStorage = (Dictionary<string, string>)JsonConvert.DeserializeObject(Settings.EmoteKeyBinds.Value, typeof(Dictionary<string, string>));
        }
        public static void SaveKeybinds()
        {
            foreach (var inputRefKey in inputRefs.Keys)
            {
                if (keyBindOverrideStorage.ContainsKey(inputRefKey))
                {
                    keyBindOverrideStorage[inputRefKey] = inputRefs[inputRefKey].action.name;
                }
            }
            Settings.EmoteKeyBinds.Value = JsonConvert.SerializeObject(keyBindOverrideStorage.Select(kvp => !string.IsNullOrEmpty(kvp.Value)));
        }
        public static void FullReloadKeybinds()
        {
            LoadKeybinds();
            foreach (var inputRef in inputRefs.Values)
            {
                inputRef.action.RemoveAllBindingOverrides();
                if (keyBindOverrideStorage.ContainsKey(inputRef.action.name))
                {
                    inputRef.action.ApplyBindingOverride(keyBindOverrideStorage[inputRef.action.name]);
                }
            }
        }
    }
}
