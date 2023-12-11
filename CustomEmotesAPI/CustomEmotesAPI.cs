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

namespace EmotesAPI
{
    [BepInPlugin(PluginGUID, PluginName, VERSION)]
    public class CustomEmotesAPI : BaseUnityPlugin
    {
        public const string PluginGUID = "com.weliveinasociety.CustomEmotesAPI";

        public const string PluginName = "Custom Emotes API";

        public const string VERSION = "1.0.0";
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

        private void PlayerControllerUpdate(Action<PlayerControllerB> orig, PlayerControllerB self)
        {
            orig(self);
            if (!(!self.isPlayerControlled || !((NetworkBehaviour)self).IsOwner))
            {
                if (buttonLock)
                {
                    if (!InputControlExtensions.IsPressed(Keyboard.current[Key.C]) && !InputControlExtensions.IsPressed(Keyboard.current[Key.G]) && !InputControlExtensions.IsPressed(Keyboard.current[Key.V]) && !InputControlExtensions.IsPressed(Keyboard.current[Key.Y]))
                    {
                        buttonLock = false;
                    }
                }
                else
                {
                    if (InputControlExtensions.IsPressed(Keyboard.current[Key.Y]))
                    {
                        buttonLock = true;
                        PlayAnimation("VirtualInsanity");
                    }
                    if (InputControlExtensions.IsPressed(Keyboard.current[Key.C]))
                    {
                        buttonLock = true;
                        PlayAnimation("none"); 
                    }
                    if (InputControlExtensions.IsPressed(Keyboard.current[Key.G]))
                    {
                        buttonLock = true;
                        int rand = UnityEngine.Random.Range(0, allClipNames.Count);
                        while (blacklistedClips.Contains(rand))
                        {
                            rand = UnityEngine.Random.Range(0, allClipNames.Count);
                        }
                        PlayAnimation(allClipNames[rand]);
                    }
                    if (InputControlExtensions.IsPressed(Keyboard.current[Key.V]))
                    {
                        buttonLock = true;
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
                                        DebugClass.Log($"{nearestMapper.currentClip.clip[0].name}");
                                        DebugClass.Log($"{localMapper}");
                                        DebugClass.Log($"{nearestMapper}");

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
                }
            }
            DebugClass.Log($"end of update inputdir: {self.moveInputVector}");
            
        }
        private static Hook playerControllerUpdateHook;

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

        private static bool buttonLock = false;


        private static GameObject emoteNetworker;


        //Vector3 prevCamPosition = Vector3.zero;
        internal static void AutoWalking(PlayerControllerB player)
        {
            DebugClass.Log($"{localMapper == player.GetComponentInChildren<BoneMapper>()}    {localMapper}    {player.GetComponentInChildren<BoneMapper>()}");
            if (localMapper == player.GetComponentInChildren<BoneMapper>())
            {
                DebugClass.Log($"{localMapper.autoWalkSpeed}");
                if (localMapper.autoWalkSpeed != 0)
                {
                    DebugClass.Log($"after {player.moveInputVector}");
                    player.moveInputVector = new Vector2(localMapper.autoWalkSpeed, 0);
                    DebugClass.Log($"before {player.moveInputVector}");
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

            //if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.gemumoddo.MoistureUpset"))
            //{
            //}
            CustomEmotesAPI.LoadResource("moisture_animationreplacements"); // I don't remember what's in here that makes importing emotes work, don't @ me
            Settings.RunAll();

            var targetMethod = typeof(PlayerControllerB).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(PlayerControllerStart), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            playerControllerStartHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(PlayerControllerB).GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(PlayerControllerUpdate), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            playerControllerUpdateHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(GameNetworkManager).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(NetworkManagerStart), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            networkManagerStartHook = new Hook(targetMethod, destMethod, this);

            targetMethod = typeof(StartOfRound).GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            destMethod = typeof(CustomEmotesAPI).GetMethod(nameof(StartOfRoundUpdate), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startOfRoundUpdateHook = new Hook(targetMethod, destMethod, this);


            AnimationReplacements.RunAll();


            float WhosSteveJobs = 69420;
            CreateBaseNameTokenPairs();
            if (Settings.DontTouchThis.Value < 101)
            {
                WhosSteveJobs = Settings.DontTouchThis.Value;
            }

            //TODO scene changed, so audio mainly
            //On.RoR2.SceneCatalog.OnActiveSceneChanged += (orig, self, scene) =>
            //{
            //    orig(self, scene);
            //    //TODO stuff
            //    //AkSoundEngine.SetRTPCValue("Volume_Emotes", Settings.EmotesVolume.Value);
            //    if (allClipNames != null)
            //    {
            //        ScrollManager.SetupButtons(allClipNames);
            //    }
            //    //TODO stuff
            //    //AkSoundEngine.SetRTPCValue("Volume_MSX", Actual_MSX);
            //    for (int i = 0; i < CustomAnimationClip.syncPlayerCount.Count; i++)
            //    {
            //        CustomAnimationClip.syncTimer[i] = 0;
            //        CustomAnimationClip.syncPlayerCount[i] = 0;
            //    }
            //    if (scene.name == "title" && WhosSteveJobs < 101)
            //    {
            //        //TODO stuff
            //        //AkSoundEngine.SetRTPCValue("Volume_MSX", WhosSteveJobs);
            //        Actual_MSX = WhosSteveJobs;
            //        WhosSteveJobs = 69420;
            //    }
            //    foreach (var item in BoneMapper.allMappers)
            //    {
            //        try
            //        {
            //            foreach (var thing in audioContainers)
            //            {
            //                //TODO stuff
            //                //AkSoundEngine.StopAll(thing);
            //            }
            //            if (item)
            //            {
            //                item.audioObjects[item.currentClip.syncPos].transform.localPosition = new Vector3(0, -10000, 0);
            //                //TODO stuff
            //                //AkSoundEngine.PostEvent(BoneMapper.stopEvents[item.currentClip.syncPos][item.currEvent], item.audioObjects[item.currentClip.syncPos]);
            //            }
            //        }
            //        catch (System.Exception e)
            //        {
            //            DebugClass.Log($"Error when cleaning up audio on scene exit: {e}");
            //        }
            //    }
            //    BoneMapper.allMappers.Clear();
            //    localMapper = null;
            //    EmoteLocation.visibile = true;
            //};
            //TODO base volume slider setting the actual_msx
            //On.RoR2.AudioManager.VolumeConVar.SetString += (orig, self, newValue) =>
            //{
            //    orig(self, newValue);
            //    //Volume_MSX
            //    if (self.GetFieldValue<string>("rtpcName") == "Volume_MSX" && WhosSteveJobs > 100)
            //    {
            //        Actual_MSX = float.Parse(newValue, CultureInfo.InvariantCulture);
            //        BoneMapper.Current_MSX = Actual_MSX;
            //        Settings.DontTouchThis.Value = float.Parse(newValue, CultureInfo.InvariantCulture);
            //    }
            //};
            //On.RoR2.PlayerCharacterMasterController.FixedUpdate += (orig, self) =>
            //{
            //    orig(self);
            //    if (CustomEmotesAPI.GetKey(Settings.EmoteWheel))
            //    {
            //        if (self.hasEffectiveAuthority && self.GetFieldValue<InputBankTest>("bodyInputs"))
            //        {
            //            bool newState = false;
            //            bool newState2 = false;
            //            bool newState3 = false;
            //            bool newState4 = false;
            //            LocalUser localUser;
            //            Rewired.Player player;
            //            CameraRigController cameraRigController;
            //            bool doIt = false;
            //            if (!self.networkUser)
            //            {
            //                localUser = null;
            //                player = null;
            //                cameraRigController = null;
            //                doIt = false;
            //            }
            //            else
            //            {
            //                localUser = self.networkUser.localUser;
            //                player = self.networkUser.inputPlayer;
            //                cameraRigController = self.networkUser.cameraRigController;
            //                doIt = localUser != null && player != null && cameraRigController && !localUser.isUIFocused && cameraRigController.isControlAllowed;
            //            }
            //            if (doIt)
            //            {
            //                newState = player.GetButton(7) && !CustomEmotesAPI.GetKey(Settings.EmoteWheel); //left click
            //                newState2 = player.GetButton(8) && !CustomEmotesAPI.GetKey(Settings.EmoteWheel); //right click
            //                newState3 = player.GetButton(9);
            //                newState4 = player.GetButton(10);
            //                self.GetFieldValue<InputBankTest>("bodyInputs").skill1.PushState(newState);
            //                self.GetFieldValue<InputBankTest>("bodyInputs").skill2.PushState(newState2);
            //                BoneMapper.attacking = newState || newState2 || newState3 || newState4;
            //                BoneMapper.moving = self.GetFieldValue<InputBankTest>("bodyInputs").moveVector != Vector3.zero || player.GetButton(4);
            //            }
            //        }
            //    }
            //};


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
            DebugClass.Log($"emote instance is {CustomEmotesAPI.instance}");

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
        //TODO standard shader
        //internal static Shader standardShader = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion().GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
        public static GameObject SpawnWorldProp(int propPos)
        {
            BoneMapper.allWorldProps[propPos].prop.GetComponent<WorldPropSpawnHandler>().propPos = propPos;
            return GameObject.Instantiate(BoneMapper.allWorldProps[propPos].prop);
        }

        public static void AddNonAnimatingEmote(string emoteName, bool visible = true)
        {
            if (visible)
                allClipNames.Add(emoteName);
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
            if (animationClipParams.joinSpots == null)
                animationClipParams.joinSpots = new JoinSpot[0];
            CustomAnimationClip clip = new CustomAnimationClip(animationClipParams.animationClip, animationClipParams.looping, animationClipParams._primaryAudioClips, animationClipParams._secondaryAudioClips, animationClipParams.rootBonesToIgnore, animationClipParams.soloBonesToIgnore, animationClipParams.secondaryAnimation, animationClipParams.dimWhenClose, animationClipParams.stopWhenMove, animationClipParams.stopWhenAttack, animationClipParams.visible, animationClipParams.syncAnim, animationClipParams.syncAudio, animationClipParams.startPref, animationClipParams.joinPref, animationClipParams.joinSpots, animationClipParams.useSafePositionReset, animationClipParams.customName, animationClipParams.customPostEventCodeSync, animationClipParams.customPostEventCodeNoSync, animationClipParams.lockFPSHead);
            if (animationClipParams.visible)
                allClipNames.Add(animationClipParams.animationClip[0].name);
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
        //TODO console command
        //[ConCommand(commandName = "PlayEmote", flags = ConVarFlags.None, helpText = "Plays emote in first argument (case sensitive)")]
        //private static void SlowmoCommand(ConCommandArgs args)
        //{
        //    PlayAnimation(args[0]);
        //}
        public static void PlayAnimation(string animationName, int pos = -2)
        {
            EmoteNetworker.instance.SyncEmote(StartOfRound.Instance.localPlayerController.GetComponent<NetworkObject>().NetworkObjectId, animationName, pos);
        }
        public static void PlayAnimation(string animationName, BoneMapper mapper, int pos = -2)
        {
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
            //TODO recalculate stats (probably can remove?)
            //mapper.mapperBody.RecalculateStats();
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
                if (mapper.transform.name == "templar")
                {
                    mapper.transform.parent.Find("ClayBruiserCannonMesh").gameObject.SetActive(false);
                }
            }
            else
            {
                if (mapper == localMapper && Settings.HideJoinSpots.Value)
                {
                    EmoteLocation.ShowAllSpots();
                }
                if (mapper.transform.name == "templar")
                {
                    mapper.transform.parent.Find("ClayBruiserCannonMesh").gameObject.SetActive(true);
                }
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

        void Update()
        {
            if (GetKeyPressed(Settings.RandomEmote))
            {
                int rand = UnityEngine.Random.Range(0, allClipNames.Count);
                while (blacklistedClips.Contains(rand))
                {
                    rand = UnityEngine.Random.Range(0, allClipNames.Count);
                }
                //foreach (var item in BoneMapper.allMappers)
                //{
                //    PlayAnimation(allClipNames[rand], item);
                //}
                DebugClass.Log($"attempting to play animation: {allClipNames[rand]}");
                PlayAnimation(allClipNames[rand]);
            }
            if (GetKeyPressed(Settings.JoinEmote))
            {
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
            DebugClass.Log($"{mapper.a1.gameObject.name}");
            mapper.a1.gameObject.SetActive(false);
            yield return new WaitForSeconds(5f);
            mapper.a1.enabled = true;
            mapper.oneFrameAnimatorLeeWay = true;
            mapper.a1.gameObject.SetActive(true);
            DebugClass.Log($"reenabling");

        }
    }
}