using BepInEx;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Globalization;
using BepInEx.Configuration;
using System.Collections;
using static UnityEngine.ParticleSystem.PlaybackState;
using GameNetcodeStuff;
using MonoMod.RuntimeDetour;
using System;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine.Experimental.Audio;
using HarmonyLib;
using LethalEmotesAPI;
using LethalEmotesAPI.Data;
using LethalEmotesApi.Ui;
using UnityEngine.InputSystem.Controls;
using LethalEmotesAPI.Utils;
using TMPro;
using UnityEngine.SocialPlatforms;

namespace EmotesAPI
{
    [BepInPlugin(PluginGUID, PluginName, VERSION)]
    public class CustomEmotesAPI : BaseUnityPlugin
    {
        public const string PluginGUID = "com.weliveinasociety.CustomEmotesAPI";

        public const string PluginName = "Custom Emotes API";

        public const string VERSION = "1.0.4";
        public struct NameTokenWithSprite
        {
            public string nameToken;
            public Sprite sprite;

        }
        public static List<NameTokenWithSprite> nameTokenSpritePairs = new List<NameTokenWithSprite>();
        public static bool CreateNameTokenSpritePair(string nameToken, Sprite sprite)
        {
            NameTokenWithSprite temp = new NameTokenWithSprite();
            temp.nameToken = nameToken;
            temp.sprite = sprite;
            if (nameTokenSpritePairs.Contains(temp))
            {
                return false;
            }
            nameTokenSpritePairs.Add(temp);
            return true;
        }
        void CreateBaseNameTokenPairs()
        {
            //CreateNameTokenSpritePair("HERETIC_BODY_NAME", Assets.Load<Sprite>("@CustomEmotesAPI_customemotespackage:assets/emotewheel/heretic.png"));
        }
        public static List<string> allClipNames = new List<string>();
        public static List<string> randomClipList = new List<string>();
        public static List<int> blacklistedClips = new List<int>();
        public static void BlackListEmote(string name)
        {
            for (int i = 0; i < allClipNames.Count; i++)
            {
                if (allClipNames[i] == name)
                {
                    blacklistedClips.Add(i);
                    return;
                }
            }
        }
        internal static void LoadResource(string resource)
        {
            Assets.AddBundle($"{resource}");
        }
        internal static bool GetKey(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKey(entry.Value.MainKey);
        }
        internal static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
        internal static float Actual_MSX = 69;
        public static CustomEmotesAPI instance;
        private void PlayerControllerStart(Action<PlayerControllerB> orig, PlayerControllerB self)
        {
            DebugClass.Log($"adding bone mapper to scav");
            AnimationReplacements.Import(self.gameObject, "assets/customstuff/scavEmoteSkeleton.prefab", new int[] { 0, 1, 2, 3 });
            orig(self);
            if (self.IsServer && EmoteNetworker.instance == null)
            {
                GameObject networker = Instantiate<GameObject>(emoteNetworker);
                networker.GetComponent<NetworkObject>().Spawn(true);
            }
        }
        private static Hook playerControllerStartHook;


        public static List<GameObject> networkedObjects = new List<GameObject>();
        private void NetworkManagerStart(Action<GameNetworkManager> orig, GameNetworkManager self)
        {
            orig(self);
            emoteNetworker = Assets.Load<GameObject>($"assets/customstuff/emoteNetworker.prefab");
            emoteNetworker.AddComponent<NetworkObject>();
            emoteNetworker.AddComponent<EmoteNetworker>();
            GameNetworkManager.Instance.GetComponent<NetworkManager>().AddNetworkPrefab(emoteNetworker);
            foreach (var item in networkedObjects)
            {
                GameNetworkManager.Instance.GetComponent<NetworkManager>().AddNetworkPrefab(item);
            }
        }
        private static Hook networkManagerStartHook;
        private void StartOfRoundUpdate(Action<StartOfRound> orig, StartOfRound self)
        {
            orig(self);
            AudioFunctions();
        }
        private static Hook startOfRoundUpdateHook;

        public static Animator hudAnimator;
        public static GameObject hudObject;
        public static GameObject baseHUDObject;
        public static GameObject selfRedHUDObject;
        public static TextMeshPro currentEmoteText;
        private void HUDManagerAwake(Action<HUDManager> orig, HUDManager self)
        {
            orig(self);
            Transform selfTransform = self.PlayerInfo.canvasGroup.transform.Find("Self");
            if (selfTransform is not null)
            {
                hudObject = GameObject.Instantiate(Assets.Load<GameObject>("assets/healthbarimage2.prefab"));
                hudObject.transform.SetParent(self.PlayerInfo.canvasGroup.transform);
                baseHUDObject = self.PlayerInfo.canvasGroup.transform.Find("Self").gameObject;
                selfRedHUDObject = self.PlayerInfo.canvasGroup.transform.Find("SelfRed").gameObject;
                CustomEmotesAPI.hudObject.transform.localPosition = baseHUDObject.transform.localPosition;
            }
            var emoteWheelController = Instantiate(Assets.Load<GameObject>("assets/emote ui.prefab"),
                self.PlayerInfo.canvasGroup.transform.parent.parent);
        }
        private static Hook hudManagerAwakeHook;

        private void HUDManagerUpdateHealthUI(Action<HUDManager, int, bool> orig, HUDManager self, int health, bool hurtPlayer = true)
        {
            orig(self, health, hurtPlayer);
            if (hudObject is not null)
            {
                hudObject.GetComponent<CanvasRenderer>().GetMaterial(0).SetFloat("_HealthPercentage", health / 100f);
            }
        }
        private static Hook hudManagerUpdateHealthUIHook;

        private void StartOfRoundOnPlayerDC(Action<StartOfRound, int, ulong> orig, StartOfRound self, int playerObjectNumber, ulong clientId)
        {
            PlayerControllerB component = self.allPlayerObjects[playerObjectNumber].GetComponent<PlayerControllerB>();
            PlayAnimation("none", component.GetComponentInChildren<BoneMapper>());
            orig(self, playerObjectNumber, clientId);
        }
        private static Hook startOfRoundOnPlayerDCHook;

        private void BeginUsingTerminal(Action<Terminal> orig, Terminal self)
        {
            orig(self);
            InTerminal();
        }
        private static Hook BeginUsingTerminalHook;

        private void TeleportPlayer(Action<PlayerControllerB, Vector3, bool, float, bool, bool> orig, PlayerControllerB self, Vector3 pos, bool withRotation = false, float rot = 0f, bool allowInteractTrigger = false, bool enableController = true)
        {
            BoneMapper ownerMapper = self.GetComponentInChildren<BoneMapper>();
            if (ownerMapper && ownerMapper.parentGameObject && ownerMapper.positionLock)
            {
                //ownerMapper.parentGameObject.transform.position += pos - self.transform.position;
                ownerMapper.preserveParent = false;
                PlayAnimation("none", ownerMapper);
            }
            orig(self, pos, withRotation, rot, allowInteractTrigger, enableController);
        }
        private static Hook TeleportPlayerHook;

        private static bool buttonLock = false;


        private static GameObject emoteNetworker;


        //Vector3 prevCamPosition = Vector3.zero;
        internal static void AutoWalking(PlayerControllerB player)
        {
            if (player == StartOfRound.Instance.localPlayerController)
            {
                if (localMapper)
                {
                    if (localMapper.autoWalkSpeed != 0)
                    {
                        player.moveInputVector = new Vector2(0, localMapper.autoWalkSpeed);
                    }
                }
            }
        }
        internal static void LocalVisor(Transform transform)
        {
            if (localMapper is not null && localMapper.currentClip is not null)
            {
                foreach (var item in localMapper.cameraConstraint)
                {
                    item.ActUponConstraints();
                }
            }
        }
        public void Awake()
        {
            instance = this;
            DebugClass.SetLogger(base.Logger);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);

            CustomEmotesAPI.LoadResource("customemotespackage");
            CustomEmotesAPI.LoadResource("fineilldoitmyself");
            CustomEmotesAPI.LoadResource("enemyskeletons");
            CustomEmotesAPI.LoadResource("scavbody");
            LoadResource("customemotes-ui");

            //if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.gemumoddo.MoistureUpset"))
            //{
            //}
            CustomEmotesAPI.LoadResource("moisture_animationreplacements"); // I don't remember what's in here that makes importing emotes work, don't @ me
            Settings.RunAll();
            BlacklistSettings.LoadExcludeListFromBepinSex(Settings.RandomEmoteBlacklist);

            var targetMethod = typeof(PlayerControllerB).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(PlayerControllerStart), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            playerControllerStartHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(GameNetworkManager).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(NetworkManagerStart), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            networkManagerStartHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(StartOfRound).GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(StartOfRoundUpdate), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startOfRoundUpdateHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(HUDManager).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(HUDManagerAwake), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hudManagerAwakeHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(HUDManager).GetMethod("UpdateHealthUI", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(HUDManagerUpdateHealthUI), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hudManagerUpdateHealthUIHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(StartOfRound).GetMethod("OnPlayerDC", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(StartOfRoundOnPlayerDC), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startOfRoundOnPlayerDCHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(Terminal).GetMethod("BeginUsingTerminal", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(BeginUsingTerminal), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            BeginUsingTerminalHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(PlayerControllerB).GetMethod("TeleportPlayer", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(TeleportPlayer), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            TeleportPlayerHook = new Hook(targetMethod, destMethod, this);

            AnimationReplacements.RunAll();

            CreateBaseNameTokenPairs();

            //TODO setup ui buttons somewhere early on?
            //if (allClipNames != null)
            //{
            //    ScrollManager.SetupButtons(allClipNames);
            //}


            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            AddCustomAnimation(new AnimationClipParams() { animationClip = new AnimationClip[] { Assets.Load<AnimationClip>($"@CustomEmotesAPI_fineilldoitmyself:assets/fineilldoitmyself/lmao.anim") }, looping = false, visible = false });
            AddNonAnimatingEmote("none");
            //AddCustomAnimation(new AnimationClipParams() { animationClip = new AnimationClip[] { Assets.Load<AnimationClip>($"assets/BayonettaTest.anim") }, looping = false, visible = false });

            // Scroll Functionality 
            var ScrollU = new InputAction("ScrollUP", binding: "<Mouse>/Scroll/Up");
            var ScrollD = new InputAction("ScrollDOWN", binding: "<Mouse>/Scroll/Down");
            ScrollU.Enable();
            ScrollD.Enable();

            EmotesInputSettings.Instance.RandomEmote.started += RandomEmote_performed;
            EmotesInputSettings.Instance.JoinEmote.started += JoinEmote_performed;
            EmotesInputSettings.Instance.EmoteWheel.performed += EmoteWheelInteracted;
            EmotesInputSettings.Instance.EmoteWheel.canceled += EmoteWheelInteracted;
            EmotesInputSettings.Instance.Left.started += ctx => EmoteUiManager.OnLeftWheel();
            EmotesInputSettings.Instance.Right.started += ctx => EmoteUiManager.OnRightWheel();
            ScrollU.started += ctx => EmoteUiManager.OnLeftWheel();
            ScrollD.started += ctx => EmoteUiManager.OnRightWheel();
            EmotesInputSettings.Instance.StopEmoting.started += StopEmoting_performed;
            EmoteUiManager.RegisterStateController(LethalEmotesUiState.Instance);
        }

        private void EmoteWheelInteracted(InputAction.CallbackContext ctx)
        {
            var btn = (ButtonControl)ctx.control;

            if (btn.wasPressedThisFrame)
            {
                EmoteUiManager.OpenEmoteWheels();
            }

            if (btn.wasReleasedThisFrame)
            {
                EmoteUiManager.CloseEmoteWheels();
            }
        }

        private void JoinEmote_performed(InputAction.CallbackContext obj)
        {
            if (!EmoteUiManager.CanOpenEmoteWheels())
                return;

            try
            {
                if (localMapper)
                {
                    if (localMapper.currentEmoteSpot || localMapper.reservedEmoteSpot)
                    {
                        localMapper.JoinEmoteSpot();
                    }
                    else
                    {
                        foreach (var mapper in BoneMapper.allMappers)
                        {
                            try
                            {
                                if (mapper != localMapper)
                                {
                                    if (!nearestMapper && (mapper.currentClip.syncronizeAnimation || mapper.currentClip.syncronizeAudio))
                                    {
                                        nearestMapper = mapper;
                                    }
                                    else if (nearestMapper)
                                    {
                                        if ((mapper.currentClip.syncronizeAnimation || mapper.currentClip.syncronizeAudio) && Vector3.Distance(localMapper.transform.position, mapper.transform.position) < Vector3.Distance(localMapper.transform.position, nearestMapper.transform.position))
                                        {
                                            nearestMapper = mapper;
                                        }
                                    }
                                }
                            }
                            catch (System.Exception)
                            {
                            }
                        }
                        if (nearestMapper)
                        {
                            PlayAnimation(nearestMapper.currentClip.clip[0].name);
                            Joined(nearestMapper.currentClip.clip[0].name, localMapper, nearestMapper); //this is not networked and only sent locally FYI
                        }
                        nearestMapper = null;
                    }
                }
            }
            catch (System.Exception e)
            {
                DebugClass.Log($"had issue while attempting to join an emote as a client: {e}\nNotable info: [nearestMapper: {nearestMapper}] [localMapper: {localMapper}]");
                try
                {
                    nearestMapper.currentClip.ToString();
                    DebugClass.Log($"[nearestMapper.currentClip: {nearestMapper.currentClip.ToString()}] [nearestMapper.currentClip.clip[0]: {nearestMapper.currentClip.clip[0]}]");
                }
                catch (System.Exception)
                {
                }
            }
        }

        private void RandomEmote_performed(InputAction.CallbackContext obj)
        {
            if (!EmoteUiManager.CanOpenEmoteWheels())
                return;

            int rand = UnityEngine.Random.Range(0, randomClipList.Count);
            PlayAnimation(randomClipList[rand]);
        }

        private void StopEmoting_performed(InputAction.CallbackContext obj)
        {
            PlayAnimation("none");
        }

        public static int RegisterWorldProp(GameObject worldProp, JoinSpot[] joinSpots)
        {
            worldProp.AddComponent<NetworkObject>();
            networkedObjects.Add(worldProp);
            worldProp.AddComponent<BoneMapper>().worldProp = true;
            var handler = worldProp.AddComponent<WorldPropSpawnHandler>();
            handler.propPos = BoneMapper.allWorldProps.Count;
            BoneMapper.allWorldProps.Add(new WorldProp(worldProp, joinSpots));
            return BoneMapper.allWorldProps.Count - 1;
        }
        public static GameObject SpawnWorldProp(int propPos)
        {
            BoneMapper.allWorldProps[propPos].prop.GetComponent<WorldPropSpawnHandler>().propPos = propPos;
            return GameObject.Instantiate(BoneMapper.allWorldProps[propPos].prop);
        }

        public static void AddNonAnimatingEmote(string emoteName, bool visible = true)
        {
            if (visible)
            {
                allClipNames.Add(emoteName);
                if (!BlacklistSettings.emotesExcludedFromRandom.Contains(emoteName))
                {
                    randomClipList.Add(emoteName);
                }
            }
            BoneMapper.animClips.Add(emoteName, null);
        }
        public static void AddCustomAnimation(AnimationClipParams animationClipParams)
        {
            if (BoneMapper.animClips.ContainsKey(animationClipParams.animationClip[0].name))
            {
                Debug.Log($"EmotesError: [{animationClipParams.animationClip[0].name}] is already defined as a custom emote but is trying to be added. Skipping");
                return;
            }
            if (!animationClipParams.animationClip[0].isHumanMotion)
            {
                Debug.Log($"EmotesError: [{animationClipParams.animationClip[0].name}] is not a humanoid animation!");
                return;
            }
            if (animationClipParams.rootBonesToIgnore == null)
                animationClipParams.rootBonesToIgnore = new HumanBodyBones[0];
            if (animationClipParams.soloBonesToIgnore == null)
                animationClipParams.soloBonesToIgnore = new HumanBodyBones[0];


            if (animationClipParams._primaryAudioClips == null)
                animationClipParams._primaryAudioClips = new AudioClip[] { null };
            if (animationClipParams._secondaryAudioClips == null)
                animationClipParams._secondaryAudioClips = new AudioClip[] { null };
            if (animationClipParams._primaryDMCAFreeAudioClips == null)
                animationClipParams._primaryDMCAFreeAudioClips = new AudioClip[] { null };
            if (animationClipParams._secondaryDMCAFreeAudioClips == null)
                animationClipParams._secondaryDMCAFreeAudioClips = new AudioClip[] { null };

            List<AudioClip> testClipList = new List<AudioClip>(animationClipParams._primaryDMCAFreeAudioClips);
            while (testClipList.Count != animationClipParams._primaryAudioClips.Length)
            {
                testClipList.Add(null);
            }
            animationClipParams._primaryDMCAFreeAudioClips = testClipList.ToArray();

            testClipList = new List<AudioClip>(animationClipParams._secondaryDMCAFreeAudioClips);
            while (testClipList.Count != animationClipParams._secondaryAudioClips.Length)
            {
                testClipList.Add(null);
            }
            animationClipParams._secondaryDMCAFreeAudioClips = testClipList.ToArray();

            if (animationClipParams.joinSpots == null)
                animationClipParams.joinSpots = new JoinSpot[0];
            CustomAnimationClip clip = new CustomAnimationClip(animationClipParams.animationClip, animationClipParams.looping, animationClipParams._primaryAudioClips, animationClipParams._secondaryAudioClips, animationClipParams.rootBonesToIgnore, animationClipParams.soloBonesToIgnore, animationClipParams.secondaryAnimation, animationClipParams.dimWhenClose, animationClipParams.stopWhenMove, animationClipParams.stopWhenAttack, animationClipParams.visible, animationClipParams.syncAnim, animationClipParams.syncAudio, animationClipParams.startPref, animationClipParams.joinPref, animationClipParams.joinSpots, animationClipParams.useSafePositionReset, animationClipParams.customName, animationClipParams.customPostEventCodeSync, animationClipParams.customPostEventCodeNoSync, animationClipParams.lockType, animationClipParams._primaryDMCAFreeAudioClips, animationClipParams._secondaryDMCAFreeAudioClips, animationClipParams.willGetClaimedByDMCA, animationClipParams.audioLevel);
            if (animationClipParams.visible)
            {
                if (animationClipParams.customName != "")
                {
                    allClipNames.Add(animationClipParams.customName);
                }
                else
                {
                    allClipNames.Add(animationClipParams.animationClip[0].name);
                }
                if (!BlacklistSettings.emotesExcludedFromRandom.Contains(animationClipParams.animationClip[0].name))
                {
                    randomClipList.Add(animationClipParams.animationClip[0].name);
                }
            }
            BoneMapper.animClips.Add(animationClipParams.animationClip[0].name, clip);
        }

        public static GameObject animationControllerHolder;
        public static void ImportArmature(GameObject bodyPrefab, GameObject rigToAnimate, bool jank, int[] meshPos, bool hideMeshes = true)
        {
            if (!animationControllerHolder)
            {
                animationControllerHolder = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab"));
            }
            rigToAnimate.GetComponent<Animator>().runtimeAnimatorController = animationControllerHolder.GetComponent<Animator>().runtimeAnimatorController;
            AnimationReplacements.ApplyAnimationStuff(bodyPrefab, rigToAnimate, meshPos, hideMeshes, jank);
        }
        public static void ImportArmature(GameObject bodyPrefab, GameObject rigToAnimate, int[] meshPos, bool hideMeshes = true)
        {
            if (!animationControllerHolder)
            {
                animationControllerHolder = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab"));
            }
            rigToAnimate.GetComponent<Animator>().runtimeAnimatorController = animationControllerHolder.GetComponent<Animator>().runtimeAnimatorController;
            AnimationReplacements.ApplyAnimationStuff(bodyPrefab, rigToAnimate, meshPos, hideMeshes);
        }
        public static void PlayAnimation(string animationName, int pos = -2)
        {
            if (BoneMapper.customNamePairs.ContainsKey(animationName))
            {
                animationName = BoneMapper.customNamePairs[animationName];
            }
            EmoteNetworker.instance.SyncEmote(StartOfRound.Instance.localPlayerController.GetComponent<NetworkObject>().NetworkObjectId, animationName, pos);
        }
        public static void PlayAnimation(string animationName, BoneMapper mapper, int pos = -2)
        {
            if (BoneMapper.customNamePairs.ContainsKey(animationName))
            {
                animationName = BoneMapper.customNamePairs[animationName];
            }
            EmoteNetworker.instance.SyncEmote(mapper.mapperBody.GetComponent<NetworkObject>().NetworkObjectId, animationName, pos);
        }
        public static BoneMapper localMapper = null;
        static BoneMapper nearestMapper = null;
        public static CustomAnimationClip GetLocalBodyCustomAnimationClip()
        {
            if (localMapper)
            {
                return localMapper.currentClip;
            }
            else
            {
                return null;
            }
        }
        public static BoneMapper[] GetAllBoneMappers()
        {
            return BoneMapper.allMappers.ToArray();
        }
        public delegate void AnimationChanged(string newAnimation, BoneMapper mapper);
        public static event AnimationChanged animChanged;
        internal static void Changed(string newAnimation, BoneMapper mapper) //is a neat game made by a developer who endorses nsfw content while calling it a fine game for kids
        {
            //DebugClass.Log($"Changed {mapper}'s animation to {newAnimation}");
            mapper.currentClipName = newAnimation;
            if (mapper == localMapper)
            {
                //TODO emote wheel continue playing button (TMPUGUI is the issue)
                //EmoteWheel.dontPlayButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Continue Playing Current Emote:\r\n{newAnimation}";
                if (hudObject is not null)
                {
                    if (newAnimation == "none")
                    {
                        hudAnimator.transform.localPosition = new Vector3(-822, -235, 1100);
                        baseHUDObject.SetActive(true);
                        selfRedHUDObject.SetActive(true);
                    }
                    else
                    {
                        hudAnimator.transform.localPosition = new Vector3(-822.5184f, -235.6528f, 1074.747f);
                        baseHUDObject.SetActive(false);
                        selfRedHUDObject.SetActive(false);
                    }
                }
            }
            foreach (var item in EmoteLocation.emoteLocations)
            {
                if (item.emoter == mapper)
                {
                    try
                    {
                        item.emoter = null;
                        item.SetVisible(true);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
            if (animChanged != null)
            {
                animChanged(newAnimation, mapper);
            }
            if (newAnimation != "none")
            {
                if (mapper == localMapper && Settings.HideJoinSpots.Value)
                {
                    EmoteLocation.HideAllSpots();
                }
                if (mapper.currentClip.lockType == AnimationClipParams.LockType.rootMotion)
                {
                    mapper.prevMapperPos = mapper.transform.position;
                    mapper.prevMapperRot = mapper.transform.eulerAngles;
                }
                mapper.positionBeforeRootMotion = mapper.mapperBody.transform.position;
                mapper.rotationBeforeRootMotion = mapper.mapperBody.transform.rotation;
                mapper.justSwitched = true;
            }
            else
            {
                if (mapper.local && hudObject is not null)
                {
                    CustomEmotesAPI.currentEmoteText.color = new Color(0, 0, 0, 0);
                }
                if (mapper == localMapper && Settings.HideJoinSpots.Value)
                {
                    EmoteLocation.ShowAllSpots();
                }
                mapper.transform.localPosition = Vector3.zero;
                mapper.transform.localEulerAngles = new Vector3(90, 0, 0);
            }
        }
        public delegate void JoinedEmoteSpotBody(GameObject emoteSpot, BoneMapper joiner, BoneMapper host);
        public static event JoinedEmoteSpotBody emoteSpotJoined_Body;
        internal static void JoinedBody(GameObject emoteSpot, BoneMapper joiner, BoneMapper host)
        {
            if (emoteSpotJoined_Body != null)
                emoteSpotJoined_Body(emoteSpot, joiner, host);
        }
        public delegate void JoinedEmoteSpotProp(GameObject emoteSpot, BoneMapper joiner, BoneMapper host);
        public static event JoinedEmoteSpotProp emoteSpotJoined_Prop;
        internal static void JoinedProp(GameObject emoteSpot, BoneMapper joiner, BoneMapper host)
        {
            if (emoteSpotJoined_Prop != null)
                emoteSpotJoined_Prop(emoteSpot, joiner, host);
        }
        public delegate void AnimationJoined(string joinedAnimation, BoneMapper joiner, BoneMapper host);
        public static event AnimationJoined animJoined;
        public static void Joined(string joinedAnimation, BoneMapper joiner, BoneMapper host)
        {
            if (animJoined != null)
                animJoined(joinedAnimation, joiner, host);
        }
        public delegate void BoneMapperCreated(BoneMapper mapper);
        public static event BoneMapperCreated boneMapperCreated;
        internal static void MapperCreated(BoneMapper mapper)
        {
            if (boneMapperCreated != null)
            {
                boneMapperCreated(mapper);
            }
        }
        public delegate void BoneMapperEnteredJoinSpot(BoneMapper mover, BoneMapper joinSpotOwner);
        public static event BoneMapperEnteredJoinSpot boneMapperEnteredJoinSpot;
        internal static void JoinSpotEntered(BoneMapper mover, BoneMapper joinSpotOwner)
        {
            if (boneMapperEnteredJoinSpot != null)
            {
                boneMapperEnteredJoinSpot(mover, joinSpotOwner);
            }
        }
        public delegate void EmoteWheelPulledUp(Sprite wheelSprite, BoneMapper localMapper);
        public static event EmoteWheelPulledUp emoteWheelPulledUp;
        internal static void EmoteWheelOpened(Sprite wheel)
        {
            if (emoteWheelPulledUp != null)
            {
                emoteWheelPulledUp(wheel, localMapper);
            }
        }
        public static void ReserveJoinSpot(GameObject joinSpot, BoneMapper mapper)
        {
            mapper.reservedEmoteSpot = joinSpot;
        }
        public static void AudioFunctions()
        {
            for (int i = 0; i < CustomAnimationClip.syncPlayerCount.Count; i++)
            {
                if (CustomAnimationClip.syncPlayerCount[i] != 0)
                {
                    CustomAnimationClip.syncTimer[i] += Time.deltaTime;
                }
            }
        }
        internal void wackActive(BoneMapper mapper)
        {
            StartCoroutine(wackActive2(mapper));
        }
        internal IEnumerator wackActive2(BoneMapper mapper)
        {
            DebugClass.Log($"{mapper.basePlayerModelAnimator.gameObject.name}");
            mapper.basePlayerModelAnimator.gameObject.SetActive(false);
            yield return new WaitForSeconds(5f);
            mapper.basePlayerModelAnimator.enabled = true;
            mapper.oneFrameAnimatorLeeWay = true;
            mapper.basePlayerModelAnimator.gameObject.SetActive(true);
            DebugClass.Log($"reenabling");

        }

        public static void InTerminal() // Ends emote when opening terminal
        {
            var localPlayer = GameNetworkManager.Instance.localPlayerController;
            PlayAnimation("none");
        }
    }
}