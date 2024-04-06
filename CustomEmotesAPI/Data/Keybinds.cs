using EmotesAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalEmotesAPI.Data
{
    public static class Keybinds
    {
        private static InputActionAsset _inputActionAsset;
        private static readonly InputActionMap ActionMap = new("LethalEmotesAPI Emote Keybinds");
        
        public static Dictionary<string, string> keyBindOverrideStorage = new();//stores all emotes that have ever been loaded with their corresponding input path thing
        private static readonly Dictionary<string, InputActionReference> InputRefs = new(); // Cache refs for faster look-ups
        
        public static void LoadKeybinds()
        {
            if (_inputActionAsset is null)
            {
                _inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
                _inputActionAsset.AddActionMap(ActionMap);
            }
            
            keyBindOverrideStorage = (Dictionary<string, string>)JsonConvert.DeserializeObject(Settings.EmoteKeyBinds.Value, typeof(Dictionary<string, string>));
            if (keyBindOverrideStorage is null)
                return;

            foreach (var (key, bindingOverride) in keyBindOverrideStorage)
            {
                if (!InputRefs.TryGetValue(key, out var inputRef))
                    continue;
                
                inputRef.action.RemoveAllBindingOverrides();
                inputRef.action.ApplyBindingOverride(bindingOverride);
            }
        }
        
        public static void SaveKeybinds()
        {
            foreach (var (actionId, inputRef) in InputRefs)
            {
                keyBindOverrideStorage[actionId] = inputRef.action.bindings[0].effectivePath;
            }

            keyBindOverrideStorage = keyBindOverrideStorage.Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Settings.EmoteKeyBinds.Value = JsonConvert.SerializeObject(keyBindOverrideStorage);
        }
        
        public static void FullReloadKeybinds()
        {
            LoadKeybinds();
            foreach (var action in ActionMap.actions)
            {
                action.RemoveAllBindingOverrides();
                if (keyBindOverrideStorage.ContainsKey(action.name))
                {
                    action.ApplyBindingOverride(keyBindOverrideStorage[action.name]);
                }
            }
        }

        internal static InputActionReference GetOrCreateInputRef(string actionId)
        {
            if (InputRefs.TryGetValue(actionId, out var inputRef))
                return inputRef;
            
            var action = ActionMap.AddAction(actionId);
            action.AddBinding("");
            
            inputRef = InputActionReference.Create(action);
            InputRefs[actionId] = inputRef;

            return inputRef;
        }

        internal static void EnableKeybinds() => ActionMap.Enable();
        
        internal static void DisableKeybinds() => ActionMap.Disable();

        internal static string[] GetEmoteKeysForBindPath(string bindPath)
        {
            return keyBindOverrideStorage.Where(kvp => string.Equals(kvp.Value, bindPath, StringComparison.InvariantCultureIgnoreCase))
                .Select(kvp => kvp.Key)
                .ToArray();
        }
    }
}
