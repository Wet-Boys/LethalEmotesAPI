using LethalCompanyInputUtils.Api;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace LethalEmotesAPI
{
    public class EmotesInputSettings : LcInputActions
    {
        public static readonly EmotesInputSettings Instance = new();

        [InputAction("<Keyboard>/c", Name = "Open Emote Wheel", ActionType = InputActionType.Value)]
        public InputAction EmoteWheel { get; set; }

        [InputAction("<Mouse>/leftButton", Name = "Cycle Wheel Left")]
        public InputAction Left {  get; set; }

        [InputAction("<Mouse>/rightButton", Name = "Cycle Wheel Right")]
        public InputAction Right { get; set; }

        [InputAction("<Keyboard>/f", Name = "Play Random Emote")]
        public InputAction RandomEmote {  get; set; }

        [InputAction("<Keyboard>/v", Name = "Join Emote")]
        public InputAction JoinEmote { get; set; }

        [InputAction("<Keyboard>/b", Name = "Bind Currently Playing Emote To Current Selection")]
        public InputAction SetCurrentEmoteToWheel { get; set; }

        [InputAction("<Keyboard>/b", Name = "Fuck you")]
        public InputAction TestButton { get; set; }

        [InputAction("<Keyboard>/u", Name = "Fuck you")]
        public InputAction TestButton2 { get; set; }

        [InputAction("<Keyboard>/i", Name = "Fuck you")]
        public InputAction TestButton3 { get; set; }

        [InputAction("<Keyboard>/j", Name = "Fuck you")]
        public InputAction TestButton4 { get; set; }
        [InputAction("<Keyboard>/k", Name = "Fuck you")]
        public InputAction TestButton5 { get; set; }
        [InputAction("<Keyboard>/l", Name = "Fuck you")]
        public InputAction TestButton6 { get; set; }
        [InputAction("<Keyboard>/m", Name = "Fuck you")]
        public InputAction TestButton7 { get; set; }
    }
}
