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

        [InputAction("<Keyboard>/c", Name = "CustomEmotesAPI: Open Emote Wheel", ActionType = InputActionType.Value)]
        public InputAction EmoteWheel { get; set; }

        [InputAction("<Mouse>/leftButton", Name = "CustomEmotesAPI: Cycle Wheel Left")]
        public InputAction Left {  get; set; }

        [InputAction("<Mouse>/rightButton", Name = "CustomEmotesAPI: Cycle Wheel Right")]
        public InputAction Right { get; set; }

        [InputAction("<Keyboard>/f", Name = "CustomEmotesAPI: Play Random Emote")]
        public InputAction RandomEmote {  get; set; }

        [InputAction("<Keyboard>/v", Name = "CustomEmotesAPI: Join Emote")]
        public InputAction JoinEmote { get; set; }

        [InputAction("", Name = "Stop emoting")]
        public InputAction StopEmoting { get; set; }

        [InputAction("<Mouse>/middleButton", Name = "CustomEmotesAPI: Third Person Toggle")]
        public InputAction ThirdPersonToggle { get; set; }
        //[InputAction("<Keyboard>/p", Name = "Fuck you")]
        //public InputAction ligmaballs { get; set; }
    }
}
