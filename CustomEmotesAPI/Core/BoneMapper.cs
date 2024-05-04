﻿using BepInEx.Bootstrap;
using EmotesAPI;
using GameNetcodeStuff;
using LethalEmotesAPI;
using LethalEmotesAPI.Core;
using LethalEmotesAPI.Patches;
using LethalEmotesAPI.Patches.ModCompat;
using LethalEmotesAPI.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BoneMapper : MonoBehaviour
{
    public static List<AudioClip[]> primaryAudioClips = new List<AudioClip[]>();
    public static List<AudioClip[]> secondaryAudioClips = new List<AudioClip[]>();
    public static List<AudioClip[]> primaryDMCAFreeAudioClips = new List<AudioClip[]>();
    public static List<AudioClip[]> secondaryDMCAFreeAudioClips = new List<AudioClip[]>();
    public GameObject audioObject;
    public SkinnedMeshRenderer emoteSkeletonSMR;
    public SkinnedMeshRenderer[] basePlayerModelSMR;
    public Animator basePlayerModelAnimator, emoteSkeletonAnimator;
    public int h;
    public List<BonePair> pairs = new List<BonePair>();
    public float timer = 0;
    public GameObject model;
    bool twopart = false;
    public static Dictionary<string, CustomAnimationClip> animClips = new Dictionary<string, CustomAnimationClip>();
    public CustomAnimationClip currentClip = null;
    public string currentClipName = "none";
    public string prevClipName = "none";
    public CustomAnimationClip prevClip = null;
    internal static float Current_MSX = 69;
    internal static List<BoneMapper> allMappers = new List<BoneMapper>();
    internal static List<WorldProp> allWorldProps = new List<WorldProp>();
    public bool local = false;
    internal static bool moving = false;
    internal static bool attacking = false;
    public bool jank = false;
    public List<GameObject> props = new List<GameObject>();
    public float scale = 1.0f;
    internal int desiredEvent = 0;
    public int currEvent = 0;
    public float autoWalkSpeed = 0;
    public bool overrideMoveSpeed = false;
    public bool autoWalk = false;
    public GameObject currentEmoteSpot = null;
    public GameObject reservedEmoteSpot = null;
    public bool worldProp = false;
    public bool ragdolling = false;
    public GameObject bodyPrefab;
    public int uniqueSpot = -1;
    public bool preserveProps = false;
    public bool preserveParent = false;
    internal bool useSafePositionReset = false;
    public List<EmoteLocation> emoteLocations = new List<EmoteLocation>();
    List<string> dontAnimateUs = new List<string>();
    public bool enableAnimatorOnDeath = true;
    public bool revertTransform = false;
    public bool oneFrameAnimatorLeeWay = false;
    public GameObject mapperBody;
    public PlayerControllerB playerController;
    public EnemyAI enemyController;
    public Transform mapperBodyTransform;
    public static bool firstMapperSpawn = true;
    public static List<List<AudioSource>> listOfCurrentEmoteAudio = new List<List<AudioSource>>();
    public List<EmoteConstraint> cameraConstraints = new List<EmoteConstraint>();
    public List<EmoteConstraint> itemHolderConstraints = new List<EmoteConstraint>();
    public Transform itemHolderPosition;
    public List<EmoteConstraint> additionalConstraints = new List<EmoteConstraint>();
    public EmoteConstraint thirdPersonConstraint;
    public static Dictionary<string, string> customNamePairs = new Dictionary<string, string>();
    public Vector3 positionBeforeRootMotion = new Vector3(69, 69, 69);
    public Quaternion rotationBeforeRootMotion = Quaternion.identity;
    public float currentAudioLevel = 0;
    public TempThirdPerson temporarilyThirdPerson = TempThirdPerson.none;
    internal int originalCullingMask;
    internal bool needToTurnOffRenderingThing = false;
    public BoneMapper currentlyLockedBoneMapper;
    public static Dictionary<GameObject, BoneMapper> playersToMappers = new Dictionary<GameObject, BoneMapper>();
    public AudioSource personalAudioSource;
    public InteractTrigger personalTrigger;
    public string currentJoinButton;
    public bool isServer = false;
    public int networkId;
    public bool joined = false;
    public bool canThirdPerson = true;
    internal bool canEmote = false;
    public bool isValidPlayer = false;
    internal bool canStop = true;
    internal List<EmoteConstraint> cosmeticConstraints = new List<EmoteConstraint>();
    internal GameObject originalCosmeticPosition;
    public static string GetRealAnimationName(string animationName)
    {
        if (customNamePairs.ContainsKey(animationName))
        {
            return customNamePairs[animationName];
        }
        return animationName;
    }
    IEnumerator lockBonesAfterAFrame()
    {
        yield return new WaitForEndOfFrame();
        LockBones();
    }
    public void PlayAnim(string s, int pos, int eventNum)
    {
        desiredEvent = eventNum;
        s = GetRealAnimationName(s);
        PlayAnim(s, pos);
    }
    public void PlayAnim(string s, int pos)
    {
        ranSinceLastAnim = false;
        s = GetRealAnimationName(s);
        prevClipName = currentClipName;
        if (s != "none")
        {
            if (!animClips.ContainsKey(s))
            {
                DebugClass.Log($"No emote bound to the name [{s}]");
                return;
            }
            if (animClips[s] is null || !animClips[s].animates)
            {
                CustomEmotesAPI.Changed(s, this);
                return;
            }
        }
        emoteSkeletonAnimator.enabled = true;

        dontAnimateUs.Clear();
        try
        {
            currentClip.clip[0].ToString();
            try
            {
                if (currentClip.syncronizeAnimation || currentClip.syncronizeAudio)
                {
                    CustomAnimationClip.syncPlayerCount[currentClip.syncPos]--;
                }
                audioObject.GetComponent<AudioManager>().Stop();
            }
            catch (Exception e)
            {
                DebugClass.Log($"had issue turning off audio before new audio played step 1: {e}");
            }
            try
            {
                if (primaryAudioClips[currentClip.syncPos][currEvent] != null && currentClip.syncronizeAudio)
                {
                    try
                    {
                        listOfCurrentEmoteAudio[currentClip.syncPos].Remove(audioObject.GetComponent<AudioSource>());
                    }
                    catch (Exception e)
                    {
                        DebugClass.Log($"had issue turning off audio before new audio played step 2: {e}");
                        try
                        {
                            DebugClass.Log($"{prevClip.syncPos}");
                            DebugClass.Log($"{currentClip.syncPos}");
                            DebugClass.Log(listOfCurrentEmoteAudio[currentClip.syncPos]);
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            DebugClass.Log($"going to try a brute force method to avoid audio desync issues");
                            foreach (var item in listOfCurrentEmoteAudio)
                            {
                                if (item.Contains(audioObject.GetComponent<AudioSource>()))
                                {
                                    item.Remove(audioObject.GetComponent<AudioSource>());
                                }
                            }
                        }
                        catch (Exception e2)
                        {
                            DebugClass.Log($"wow {e2}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugClass.Log($"had issue turning off audio before new audio played step 3: {primaryAudioClips[currentClip.syncPos]} {currentClip.syncPos} {currEvent} {e}");
            }
            try
            {
                if (uniqueSpot != -1 && CustomAnimationClip.uniqueAnimations[currentClip.syncPos][uniqueSpot])
                {
                    CustomAnimationClip.uniqueAnimations[currentClip.syncPos][uniqueSpot] = false;
                    uniqueSpot = -1;
                }
            }
            catch (Exception e)
            {
                DebugClass.Log($"had issue turning off audio before new audio played step 4: {e}");
            }


        }
        catch (Exception)
        {

        }

        currEvent = 0;
        currentClipName = s;
        if (s == "none")
        {
            emoteSkeletonAnimator.Play("none", -1, 0f);
            twopart = false;
            prevClip = currentClip;
            currentClip = null;
            NewAnimation(null);
            CustomEmotesAPI.Changed(s, this);

            return;
        }
        if (BlacklistSettings.emotesDisabled.Contains(s))
        {
            return;
        }
        if (s != "none")
        {
            prevClip = currentClip;
            currentClip = animClips[s];
            try
            {
                currentClip.clip[0].ToString();
            }
            catch (Exception)
            {
                return;
            }
            if (pos == -2)
            {
                if (CustomAnimationClip.syncPlayerCount[animClips[s].syncPos] == 0)
                {
                    pos = animClips[s].startPref;
                }
                else
                {
                    pos = animClips[s].joinPref;
                }
            }
            if (pos == -2)
            {
                for (int i = 0; i < CustomAnimationClip.uniqueAnimations[currentClip.syncPos].Count; i++)
                {
                    if (!CustomAnimationClip.uniqueAnimations[currentClip.syncPos][i])
                    {
                        pos = i;
                        uniqueSpot = pos;
                        CustomAnimationClip.uniqueAnimations[currentClip.syncPos][uniqueSpot] = true;
                        break;
                    }
                }
                if (uniqueSpot == -1)
                {
                    pos = -1;
                }
            }
            if (pos == -1)
            {
                pos = UnityEngine.Random.Range(0, currentClip.clip.Length);
            }
            StartCoroutine(lockBonesAfterAFrame());
        }
        AnimatorOverrideController animController = new AnimatorOverrideController(emoteSkeletonAnimator.runtimeAnimatorController);
        if (currentClip.syncronizeAnimation || currentClip.syncronizeAudio)
        {
            CustomAnimationClip.syncPlayerCount[currentClip.syncPos]++;
            if (CustomAnimationClip.syncPlayerCount[currentClip.syncPos] == 1)
            {
                CustomAnimationClip.syncTimer[currentClip.syncPos] = 0;
            }
        }
        if (primaryAudioClips[currentClip.syncPos][currEvent] != null)
        {
            if (CustomAnimationClip.syncPlayerCount[currentClip.syncPos] == 1 && currentClip.syncronizeAudio)
            {
                if (desiredEvent != -1)
                    currEvent = desiredEvent;
                else
                    currEvent = UnityEngine.Random.Range(0, primaryAudioClips[currentClip.syncPos].Length);
                foreach (var item in allMappers)
                {
                    item.currEvent = currEvent;
                }
                if (currentClip.customPostEventCodeSync != null)
                {
                    currentClip.customPostEventCodeSync.Invoke(this);
                }
            }
            else if (!currentClip.syncronizeAudio)
            {
                currEvent = UnityEngine.Random.Range(0, primaryAudioClips[currentClip.syncPos].Length);
                if (currentClip.customPostEventCodeNoSync != null)
                {
                    currentClip.customPostEventCodeNoSync.Invoke(this);
                }
            }
            currentAudioLevel = currentClip.audioLevel;
            audioObject.GetComponent<AudioManager>().Play(currentClip.syncPos, currEvent, currentClip.looping, currentClip.syncronizeAudio, currentClip.willGetClaimed);
            if (currentClip.syncronizeAudio && primaryAudioClips[currentClip.syncPos][currEvent] != null)
            {
                listOfCurrentEmoteAudio[currentClip.syncPos].Add(audioObject.GetComponent<AudioSource>());
            }
        }
        SetAnimationSpeed(1);
        StartAnimations(animController, pos, emoteSkeletonAnimator);
        if (local && CustomEmotesAPI.hudObject is not null)
        {


            if (currentClip.displayName != "")
            {
                CustomEmotesAPI.currentEmoteText.text = currentClip.displayName;
            }
            else if (currentClip.customInternalName != "")
            {
                CustomEmotesAPI.currentEmoteText.text = currentClip.customInternalName;
            }
            else
            {
                if (!currentClip.visibility)
                {
                    CustomEmotesAPI.currentEmoteText.text = "";
                }
                else
                {
                    CustomEmotesAPI.currentEmoteText.text = currentClipName;
                }
            }
        }
        twopart = false;
        NewAnimation(currentClip.joinSpots);

        if (currentClip.usesNewImportSystem)
        {
            CustomEmotesAPI.Changed(currentClip.customInternalName, this);
        }
        else
        {
            CustomEmotesAPI.Changed(s, this);
        }
    }
    public void StartAnimations(AnimatorOverrideController animController, int pos, Animator animator)
    {
        if (currentClip.secondaryClip != null && currentClip.secondaryClip.Length != 0)
        {
            if (true)
            {
                if (CustomAnimationClip.syncTimer[currentClip.syncPos] > currentClip.clip[pos].length)
                {
                    animController["Floss"] = currentClip.secondaryClip[pos];
                    animator.runtimeAnimatorController = animController;
                    animator.Play("Loop", -1, (((CustomAnimationClip.syncTimer[currentClip.syncPos] - currentClip.clip[pos].length/* + .25f*/) % currentClip.secondaryClip[pos].length) / currentClip.secondaryClip[pos].length)/* - 0.0815217f*/);
                }
                else
                {
                    animController["Dab"] = currentClip.clip[pos];
                    animController["nobones"] = currentClip.secondaryClip[pos];
                    animator.runtimeAnimatorController = animController;
                    animator.Play("PoopToLoop", -1, (CustomAnimationClip.syncTimer[currentClip.syncPos] % currentClip.clip[pos].length) / currentClip.clip[pos].length);
                }
            }
        }
        else if (currentClip.clip[0].isLooping)
        {
            animController["Floss"] = currentClip.clip[pos];
            animator.runtimeAnimatorController = animController;
            if (currentClip.clip[pos].length != 0)
            {
                animator.Play("Loop", -1, (CustomAnimationClip.syncTimer[currentClip.syncPos] % currentClip.clip[pos].length) / currentClip.clip[pos].length);
            }
            else
            {
                animator.Play("Loop", -1, 0);
            }
        }
        else
        {
            animController["Default Dance"] = currentClip.clip[pos];
            animator.runtimeAnimatorController = animController;
            animator.Play("Poop", -1, (CustomAnimationClip.syncTimer[currentClip.syncPos] % currentClip.clip[pos].length) / currentClip.clip[pos].length);
        }
    }
    public static void PreviewAnimations(Animator animator, string animation)
    {
        AnimatorOverrideController animController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animation = GetRealAnimationName(animation);
        if (!animClips.ContainsKey(animation))
            return;
        CustomAnimationClip customClip = animClips[animation];
        if (customClip is null || customClip.clip is null || customClip.clip[0] is null)
        {
            return;
        }
        int pos = 0;
        if (customClip.secondaryClip != null && customClip.secondaryClip.Length != 0)
        {
            animController["Dab"] = customClip.clip[pos];
            animController["nobones"] = customClip.secondaryClip[pos];
            animator.runtimeAnimatorController = animController;
            animator.Play("PoopToLoop", -1, 0);
        }
        else if (customClip.clip[0].isLooping)
        {
            animController["Floss"] = customClip.clip[pos];
            animator.runtimeAnimatorController = animController;
            animator.Play("Loop", -1, 0);
        }
        else
        {
            animController["Default Dance"] = customClip.clip[pos];
            animator.runtimeAnimatorController = animController;
            animator.Play("Poop", -1, 0);
        }
    }
    public void SetAnimationSpeed(float speed)
    {
        emoteSkeletonAnimator.speed = speed;
    }
    internal void NewAnimation(JoinSpot[] locations)
    {
        try
        {
            try
            {
                if (local)
                {
                    itemHolderPosition.gameObject.GetComponent<EmoteConstraint>().DeactivateConstraints();
                }
            }
            catch (Exception e)
            {
            }
            try
            {
                emoteLocations.Clear();
                autoWalkSpeed = 0;
                autoWalk = false;
                overrideMoveSpeed = false;
                if (parentGameObject && !preserveParent)
                {
                    parentGameObject = null;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                useSafePositionReset = currentClip.useSafePositionReset;
            }
            catch (Exception)
            {
                useSafePositionReset = true;
            }
            try
            {
                if (preserveParent)
                {
                    preserveParent = false;
                }
                else
                {
                    mapperBody.gameObject.transform.localEulerAngles = new Vector3(0, mapperBody.gameObject.transform.localEulerAngles.y, 0);
                    if (ogScale != new Vector3(-69, -69, -69))
                    {
                        mapperBody.transform.localScale = ogScale;
                        if (local)
                        {
                            playerController.localVisor.localScale = ogVisorScale;
                        }
                        ogScale = new Vector3(-69, -69, -69);
                    }
                    foreach (var item in mapperBody.GetComponentsInChildren<Collider>())
                    {
                        item.enabled = true;
                    }
                    if (mapperBody.GetComponent<CharacterController>())
                    {
                        mapperBody.GetComponent<CharacterController>().enabled = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            if (preserveProps)
            {
                preserveProps = false;
            }
            else
            {
                foreach (var item in props)
                {
                    if (item)
                        GameObject.Destroy(item);
                }
                props.Clear();
            }
            if (locations != null)
            {
                for (int i = 0; i < locations.Length; i++)
                {
                    SpawnJoinSpot(locations[i]);
                }
            }
        }
        catch (Exception e)
        {
            DebugClass.Log($"error during new animation: {e}");
        }
    }
    public void ScaleProps()
    {
        foreach (var item in props)
        {
            if (item)
            {
                Transform t = item.transform.parent;
                item.transform.SetParent(null);
                item.transform.localScale = new Vector3(scale * 1.15f, scale * 1.15f, scale * 1.15f);
                item.transform.SetParent(t);
            }
        }
    }

    public void UpdateHoverTip(string emoteName)
    {
        if (personalTrigger is not null)
        {
            currentJoinButton = InputControlPath.ToHumanReadableString(
                EmotesInputSettings.Instance.JoinEmote.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
            personalTrigger.hoverTip = $"Press [{currentJoinButton}] to join {emoteName}";
        }
    }

    internal IEnumerator preventEmotesInSpawnAnimation() //this is a hacky fix, but for some reason if you emote while spawning the log will just fucking die, need to come back to this but it's not really a big deal
    {
        yield return new WaitForSeconds(3);
        foreach (var item in cosmeticConstraints)
        {
            item.ActivateConstraints();
        }
        canEmote = true;
    }
    void Start()
    {
        if (worldProp)
        {
            return;
        }
        if (transform.GetComponentInParent<PlayerControllerB>())
        {
            mapperBody = transform.GetComponentInParent<PlayerControllerB>().gameObject;
            networkId = (int)mapperBody.GetComponent<PlayerControllerB>().NetworkObjectId;
        }
        else if (transform.GetComponentInParent<EnemyAI>())
        {
            mapperBody = transform.GetComponentInParent<EnemyAI>().gameObject;
            //networkId = (int)mapperBody.GetComponent<EnemyAI>().NetworkObjectId;
            networkId = -1; //just leaving this here, if anyone ever makes enemies have cosmetics I don't want it to break, I'll fix it after this hypothetical scenario happens
            isEnemy = CustomEmotesAPI.localMapper.isServer;
        }
        else
        {
            networkId = -1;
            mapperBody = gameObject;
        }
        playerController = mapperBody.GetComponent<PlayerControllerB>();
        if (playerController is null)
        {
            enemyController = mapperBody.GetComponent<EnemyAI>();

        }
        isValidPlayer = playerController is not null;
        playersToMappers.Add(mapperBody, this);
        mapperBodyTransform = mapperBody.transform;
        allMappers.Add(this);

        // Tool Tip Handling
        if (playerController is not null)
        {
            GameObject trigObject = mapperBody.gameObject.transform.Find("PlayerPhysicsBox").gameObject;
            trigObject.tag = "InteractTrigger";
            //trigObject.layer = LayerMask.NameToLayer("InteractableObject");
            personalTrigger = trigObject.AddComponent<InteractTrigger>();
            personalTrigger.interactable = false;
            personalTrigger.hoverIcon = Sprite.Instantiate(Assets.Load<Sprite>("assets/fineilldoitmyself/nothing.png"));
            personalTrigger.disabledHoverIcon = Sprite.Instantiate(Assets.Load<Sprite>("assets/fineilldoitmyself/nothing.png"));
            personalTrigger.disabledHoverTip = "";
            UpdateHoverTip("none");
        }


        GameObject obj = GameObject.Instantiate(Assets.Load<GameObject>("assets/source1.prefab"));
        obj.name = $"{name}_AudioObject";
        obj.transform.SetParent(mapperBody.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.AddComponent<SphereCollider>().radius = .01f;
        obj.layer = 6;
        personalAudioSource = obj.GetComponent<AudioSource>();
        obj.AddComponent<AudioManager>().Setup(personalAudioSource, this);
        personalAudioSource.playOnAwake = false;
        personalAudioSource.volume = Settings.EmotesVolume.Value / 100f;
        audioObject = obj;

        int offset = 0;
        bool nuclear = true;
        if (nuclear)
        {
            foreach (var smr in basePlayerModelSMR)
            {
                int startingXPoint = 0;
                for (int i = 0; i < emoteSkeletonSMR.bones.Length; i++)
                {
                    for (int x = startingXPoint; x < smr.bones.Length; x++)
                    {
                        //DebugClass.Log($"comparing:    {emoteSkeletonSMR.bones[i].name} from {emoteSkeletonSMR}   to  {smr.bones[x].name} from {smr}");
                        //DebugClass.Log($"--------------  {smrbone.gameObject.name}   {smr1bone.gameObject.name}      {smrbone.GetComponent<ParentConstraint>()}");
                        if (emoteSkeletonSMR.bones[i].name == smr.bones[x].name && !smr.bones[x].gameObject.GetComponent<EmoteConstraint>())
                        {
                            startingXPoint = x;
                            //DebugClass.Log($"they are equal!");
                            //DebugClass.Log($"{smr.name}--- i is equal to {x}  ------ {smr.bones[x].name}");
                            EmoteConstraint e = smr.bones[x].gameObject.AddComponent<EmoteConstraint>();
                            e.AddSource(ref smr.bones[x], ref emoteSkeletonSMR.bones[i]);
                            e.revertTransform = revertTransform;
                            e.interpolates = true;
                            break;
                        }
                        if (x == startingXPoint - 1)
                        {
                            break;
                        }
                        if (startingXPoint > 0 && x == smr.bones.Length - 1)
                        {
                            x = -1;
                        }
                    }
                }
            }
        }
        if (jank)
        {
            foreach (var smr in basePlayerModelSMR)
            {
                for (int i = 0; i < smr.bones.Length; i++)
                {
                    try
                    {
                        if (smr.bones[i].gameObject.GetComponent<EmoteConstraint>())
                        {
                            //DebugClass.Log($"-{i}---------{smr2.bones[i].gameObject}");
                            smr.bones[i].gameObject.GetComponent<EmoteConstraint>().ActivateConstraints();
                        }
                    }
                    catch (Exception e)
                    {
                        DebugClass.Log($"{e}");
                    }
                }
            }
            //a1.enabled = false;
        }
        transform.localPosition = Vector3.zero;
        CustomEmotesAPI.MapperCreated(this);
        if (playerController is not null)
        {
            StartCoroutine(SetupHandConstraint());
        }
        StartCoroutine(preventEmotesInSpawnAnimation());
        StartCoroutine(GetLocal());

        GameObject g = new GameObject();
        g.name = "BoneMapperHolder";
        g.transform.SetParent(mapperBody.transform);
        g.transform.localEulerAngles = transform.localEulerAngles;
        g.transform.position = transform.position;
        transform.SetParent(g.transform);
        //transform.localEulerAngles = Vector3.zero;
        //transform.localPosition = Vector3.zero;
        g.transform.localPosition = Vector3.zero;
    }
    public IEnumerator SetupHandConstraint()
    {
        while (!CustomEmotesAPI.localMapper)
        {
            yield return new WaitForEndOfFrame();
        }
        itemHolderPosition = this.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand).Find("ServerItemHolder");
        itemHolderConstraints.Add(EmoteConstraint.AddConstraint(playerController.serverItemHolder.gameObject, this, itemHolderPosition, true));
        itemHolderConstraints.Add(EmoteConstraint.AddConstraint(playerController.localItemHolder.gameObject, this, itemHolderPosition, true));
        itemHolderPosition.gameObject.AddComponent<EmoteConstraint>();
    }
    public GameObject parentGameObject;
    public bool positionLock, rotationLock, scaleLock;
    public void AssignParentGameObject(GameObject youAreTheFather, bool lockPosition, bool lockRotation, bool lockScale, bool scaleAsScavenger = true, bool disableCollider = true)
    {
        if (parentGameObject)
        {
            NewAnimation(null);
        }
        ogScale = mapperBody.transform.localScale;
        if (local)
        {
            ogVisorScale = playerController.localVisor.localScale;
        }
        if (scaleAsScavenger)
            scaleDiff = ogScale / scale;
        else
            scaleDiff = ogScale;

        parentGameObject = youAreTheFather;
        positionLock = lockPosition;
        rotationLock = lockRotation;
        scaleLock = lockScale;

        foreach (var item in mapperBody.GetComponentsInChildren<Collider>())
        {
            item.enabled = !disableCollider;
        }
        if (mapperBody.GetComponent<CharacterController>())
        {
            mapperBody.GetComponent<CharacterController>().enabled = !disableCollider;
        }
        if (disableCollider && currentEmoteSpot)
        {
            if (currentEmoteSpot.GetComponent<EmoteLocation>().validPlayers != 0)
            {
                currentEmoteSpot.GetComponent<EmoteLocation>().validPlayers--;
            }
            currentEmoteSpot.GetComponent<EmoteLocation>().SetColor();
            currentEmoteSpot = null;
        }
    }
    Vector3 ogScale = new Vector3(-69, -69, -69);
    Vector3 ogVisorScale = Vector3.zeroVector;
    Vector3 scaleDiff = Vector3.one;
    void LocalFunctions()
    {
        if (emoteSkeletonAnimator.enabled)
        {
            try
            {
                if (moving && currentClip.stopOnMove)
                {
                    CustomEmotesAPI.PlayAnimation("none");
                }
            }
            catch (Exception)
            {
            }
        }
    }
    public GameObject rotationPoint;
    public GameObject desiredCameraPos;
    public GameObject realCameraPos;
    IEnumerator GetLocal()
    {
        yield return new WaitForEndOfFrame();
        try
        {
            if (!CustomEmotesAPI.localMapper)
            {
                if (playerController == StartOfRound.Instance.localPlayerController)
                {
                    CustomEmotesAPI.localMapper = this;
                    local = true;
                    playerController.gameplayCamera.nearClipPlane = 0.0005f;
                    originalCosmeticPosition = new GameObject();
                    originalCosmeticPosition.transform.parent = playerController.headCostumeContainerLocal.parent;
                    originalCosmeticPosition.transform.position = playerController.headCostumeContainerLocal.position;
                    originalCosmeticPosition.transform.localEulerAngles = playerController.headCostumeContainerLocal.localEulerAngles;
                    EmoteConstraint e = playerController.headCostumeContainerLocal.gameObject.AddComponent<EmoteConstraint>();
                    e.AddSource(playerController.headCostumeContainerLocal, emoteSkeletonAnimator.GetBoneTransform(HumanBodyBones.Head));
                    cosmeticConstraints.Add(e);
                    gameObject.AddComponent<NearestEmoterChecker>().self = this;
                    isServer = playerController.IsServer && playerController.IsOwner;
                    HealthbarAnimator.Setup(this);
                    FixLocalArms();

                    Camera c = playerController.gameplayCamera;
                    if (c is not null)
                    {
                        rotationPoint = new GameObject();
                        rotationPoint.transform.SetParent(c.transform.parent.parent.parent.parent);
                        rotationPoint.transform.localPosition = new Vector3(0, .8f, 0);
                        rotationPoint.transform.localEulerAngles = Vector3.zero;

                        desiredCameraPos = new GameObject();
                        desiredCameraPos.transform.SetParent(rotationPoint.transform);
                        desiredCameraPos.transform.localPosition = new Vector3(0.3f, 1.0f, -3f);
                        desiredCameraPos.transform.localEulerAngles = Vector3.zero;
                        realCameraPos = new GameObject();
                        realCameraPos.transform.SetParent(desiredCameraPos.transform);
                        realCameraPos.transform.localPosition = Vector3.zero;
                        realCameraPos.transform.localEulerAngles = Vector3.zero;
                        thirdPersonConstraint = EmoteConstraint.AddConstraint(c.transform.parent.gameObject, this, realCameraPos.transform, false);
                        thirdPersonConstraint.debug = true;

                        if (basePlayerModelSMR[0].bones[32].name == "spine.004")//probably scavenger
                        {
                            GameObject camHolder = new GameObject();
                            camHolder.name = "EmotesAPICameraHolderThing";
                            camHolder.transform.parent = basePlayerModelSMR[0].bones[32];
                            camHolder.transform.localEulerAngles = Vector3.zero;
                            camHolder.transform.position = c.transform.parent.position;
                            camHolder.transform.localPosition += new Vector3(0, .045f, 0);
                            cameraConstraints.Add(EmoteConstraint.AddConstraint(c.transform.parent.gameObject, this, camHolder.transform, false));
                        }
                        else//not scavenger or someone broke the bone order :(
                        {
                            cameraConstraints.Add(EmoteConstraint.AddConstraint(c.transform.parent.gameObject, this, emoteSkeletonAnimator.GetBoneTransform(HumanBodyBones.Head), false));
                        }

                        GameObject cameraRotationObjectLmao = new GameObject();
                        cameraRotationObjectLmao.transform.SetParent(c.transform);
                        cameraRotationObjectLmao.transform.localPosition = new Vector3(0.01f, -0.048f, -0.053f);
                        cameraRotationObjectLmao.transform.localEulerAngles = new Vector3(270f, 0, 0);

                        cameraConstraints.Add(EmoteConstraint.AddConstraint(StartOfRound.Instance.localPlayerController.localVisor.gameObject, this, cameraRotationObjectLmao.transform, false));
                    }
                }
            }
        }
        catch (Exception e)
        {
            DebugClass.Log(e);
        }
        if (!CustomEmotesAPI.localMapper)
        {
            StartCoroutine(GetLocal());
        }
    }
    internal void FixLocalArms()
    {
        int x = 0;
        for (int i = 0; i < basePlayerModelSMR[1].bones.Length; i++)
        {
            EmoteConstraint e = basePlayerModelSMR[1].bones[i].GetComponent<EmoteConstraint>();
            if (e is not null)
            {
                int startX = x;
                for (; x < basePlayerModelSMR[0].bones.Length; x++)
                {
                    if (basePlayerModelSMR[1].bones[i].name == basePlayerModelSMR[0].bones[x].name)
                    {
                        e.AddSource(basePlayerModelSMR[1].bones[i], basePlayerModelSMR[0].bones[x]);
                        //e.forceGlobalTransforms = true;
                        break;
                    }
                    if (x == startX - 1)
                    {
                        break;
                    }
                    if (startX > 0 && x == basePlayerModelSMR[0].bones.Length - 1)
                    {
                        x = -1;
                    }
                }
            }
        }
    }
    bool ranSinceLastAnim = false; //this is probably really jank but it's been 2 years since I touched this part and I'm afraid to break something, I should come back to this later though...
    void TwoPartThing()
    {
        if (!ranSinceLastAnim)
        {
            if (emoteSkeletonAnimator.GetCurrentAnimatorStateInfo(0).IsName("none"))
            {
                if (!twopart)
                {
                    twopart = true;
                }
                else
                {
                    ranSinceLastAnim = true;
                    if (emoteSkeletonAnimator.enabled)
                    {
                        if (!jank)
                        {
                            UnlockBones();
                        }
                    }
                    //DebugClass.Log($"----------{a1}");
                    if (!ragdolling)
                    {
                        basePlayerModelAnimator.enabled = true;
                        oneFrameAnimatorLeeWay = true;
                    }
                    emoteSkeletonAnimator.enabled = false;
                    try
                    {
                        currentClip.clip.ToString();
                        CustomEmotesAPI.Changed("none", this);
                        NewAnimation(null);
                        if (currentClip.syncronizeAnimation || currentClip.syncronizeAudio)
                        {
                            CustomAnimationClip.syncPlayerCount[currentClip.syncPos]--;
                        }
                        if (primaryAudioClips[currentClip.syncPos][currEvent] != null)
                        {
                            audioObject.GetComponent<AudioManager>().Stop();
                            if (primaryAudioClips[currentClip.syncPos][currEvent] != null && currentClip.syncronizeAudio)
                            {
                                listOfCurrentEmoteAudio[currentClip.syncPos].Remove(audioObject.GetComponent<AudioSource>());
                            }
                        }
                        prevClip = currentClip;
                        currentClip = null;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                //a1.enabled = false;
                twopart = false;
            }
        }
    }
    void Health()
    {
        if (isValidPlayer)
        {
            if (playerController.isPlayerDead && local && currentClip is not null)
            {
                UnlockBones();
                CustomEmotesAPI.PlayAnimation("none");
                foreach (var item in props)
                {
                    if (item)
                        GameObject.Destroy(item);
                }
                props.Clear();
            }
        }
        //todo health
        //if (h <= 0)
        //{
        //    if (local)
        //    {
        //        CustomEmotesAPI.PlayAnimation("none");
        //    }
        //    UnlockBones(enableAnimatorOnDeath);
        //    //GameObject.Destroy(gameObject);
        //    if (CustomEmotesAPI.hudObject is not null)
        //    {
        //        CustomEmotesAPI.hudObject.GetComponent<CanvasRenderer>().GetMaterial(0).SetFloat("_HealthPercentage", 100f / 100f);
        //    }
        //}
    }
    void WorldPropAndParent()
    {
        if (parentGameObject)
        {
            if (positionLock)
            {
                mapperBody.gameObject.transform.position = parentGameObject.transform.position + new Vector3(0, 1, 0);
                mapperBody.transform.position = parentGameObject.transform.position;
                playerController?.ResetFallGravity();
            }
            if (rotationLock)
            {
                //mapperBody.transform.eulerAngles = parentGameObject.transform.eulerAngles + new Vector3(90, 0, 0);
                mapperBody.transform.rotation = parentGameObject.transform.rotation;
            }
            if (scaleLock)
            {
                mapperBody.transform.localScale = new Vector3(parentGameObject.transform.localScale.x * scaleDiff.x, parentGameObject.transform.localScale.y * scaleDiff.y, parentGameObject.transform.localScale.z * scaleDiff.z);
                if (local)
                {
                    playerController.localVisor.localScale = ogVisorScale * (mapperBody.transform.localScale.x / ogScale.x);
                }
            }
        }
    }
    void Update()
    {
        if (worldProp)
        {
            return;
        }
        WorldPropAndParent();
        if (local)
        {
            LocalFunctions();
        }
        TwoPartThing();
        Health();
        SetDeltaPosition();
        RootMotion();
        CameraControls();
    }
    internal bool ThirdPersonCheck()
    {
        bool yes =
            !CustomEmotesAPI.LCThirdPersonPresent
            && currentClip is not null
            && (((currentClip.thirdPerson || Settings.thirdPersonType.Value == ThirdPersonType.All) && Settings.thirdPersonType.Value != ThirdPersonType.None) || temporarilyThirdPerson == TempThirdPerson.on)
            && canThirdPerson
            && temporarilyThirdPerson != TempThirdPerson.off;
        return yes;
    }
    public void CameraControls()
    {
        if (local && isInThirdPerson)
        {
            //just copying this from the unity docs/spectator camera KEKW
            Vector3 rayPos = mapperBodyTransform.position + new Vector3(0, 1.75f * scale, 0);
            Ray ray = new Ray(rayPos, desiredCameraPos.transform.position - rayPos);
            RaycastHit hit;//                       v PlayerControlerB.walkableSurfacesNoPlayersMask... but it's private and I don't feel like publicizing it lmao
            if (Physics.Raycast(ray, out hit, 10f, 268437761, QueryTriggerInteraction.Ignore))
            {
                realCameraPos.transform.position = ray.GetPoint(hit.distance - 0.25f);
            }
            else
            {
                realCameraPos.transform.position = ray.GetPoint(10.0f);
            }
            if (Vector3.Distance(realCameraPos.transform.position, rayPos) > Vector3.Distance(desiredCameraPos.transform.position, rayPos))
            {
                realCameraPos.transform.position = desiredCameraPos.transform.position;
            }
        }
    }
    public Vector3 deltaPos = new Vector3(0, 0, 0);
    public Quaternion deltaRot = Quaternion.identity;
    public Vector3 prevPosition = Vector3.zero;
    public Quaternion prevRotation = Quaternion.identity;
    private void SetDeltaPosition()
    {
        deltaPos = transform.position - prevPosition;
        deltaRot = transform.rotation * Quaternion.Inverse(prevRotation);
        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    public Vector3 prevMapperPos = new Vector3(69, 69, 69);
    public Vector3 prevMapperRot = new Vector3();
    public bool justSwitched = false;
    public bool isEnemy = false;
    public void RootMotion()
    {
        try
        {
            //just skip it all if we aren't playing anything
            if (!emoteSkeletonAnimator.enabled)
            {
                return;
            }
            if (justSwitched)
            {
                //this is kinda stupid but it works
                justSwitched = false;
                return;
            }
            //only do this if current animation applies root motion
            if (currentClip.lockType == AnimationClipParams.LockType.rootMotion)
            {
                //owner of the bonemapper
                //todo also let the server control root motion of EnemyAi dudes
                if (local || isEnemy)
                {
                    if (Settings.rootMotionType.Value != RootMotionType.None || isEnemy)
                    {
                        //grab current BoneMapper position
                        Vector3 tempPos = transform.position;
                        Quaternion tempRot = transform.rotation;

                        //move player body
                        mapperBody.transform.position = new Vector3(emoteSkeletonSMR.rootBone.position.x, mapperBody.transform.position.y, emoteSkeletonSMR.rootBone.position.z);
                        if (isEnemy || !isInThirdPerson)
                        {
                            mapperBody.transform.eulerAngles = new Vector3(mapperBody.transform.eulerAngles.x, emoteSkeletonAnimator.GetBoneTransform(HumanBodyBones.Head).eulerAngles.y, mapperBody.transform.eulerAngles.z);
                        }

                        //revert self to current BoneMapper position from earlier
                        transform.position = tempPos;
                        transform.rotation = tempRot;
                        basePlayerModelAnimator.transform.parent.position = tempPos;
                        //basePlayerModelAnimator.transform.parent.rotation = tempRot;
                        if (positionBeforeRootMotion != new Vector3(69, 69, 69))
                        {
                            mapperBody.transform.position = positionBeforeRootMotion;
                            mapperBody.transform.rotation = rotationBeforeRootMotion;

                            positionBeforeRootMotion = new Vector3(69, 69, 69);
                        }
                    }
                    if (deltaPos != Vector3.zero || deltaRot != Quaternion.identity)
                    {
                        //tell server where we are now
                        if (isEnemy)
                        {
                            EmoteNetworker.instance.SyncBoneMapperPos(enemyController.NetworkObjectId, transform.position, transform.eulerAngles);
                        }
                        else
                        {
                            EmoteNetworker.instance.SyncBoneMapperPos(playerController.NetworkObjectId, transform.position, transform.eulerAngles);
                        }
                    }
                }
                else
                {
                    //move bonemapper to last synced position
                    transform.position = prevMapperPos;
                    transform.eulerAngles = prevMapperRot;
                    basePlayerModelAnimator.transform.parent.position = prevMapperPos;
                }
            }
        }
        catch (Exception)
        {
        }
    }
    public int SpawnJoinSpot(JoinSpot joinSpot)
    {
        props.Add(GameObject.Instantiate(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/emotejoiner/JoinVisual.prefab")));
        props[props.Count - 1].transform.SetParent(transform);
        //Vector3 scal = transform.lossyScale;
        //props[props.Count - 1].transform.localPosition = new Vector3(joinSpot.position.x / scal.x, joinSpot.position.y / scal.y, joinSpot.position.z / scal.z);
        //props[props.Count - 1].transform.localEulerAngles = joinSpot.rotation;
        //props[props.Count - 1].transform.localScale = new Vector3(joinSpot.scale.x / scal.x, joinSpot.scale.y / scal.y, joinSpot.scale.z / scal.z);
        props[props.Count - 1].name = joinSpot.name;
        //foreach (var rend in props[props.Count - 1].GetComponentsInChildren<SkinnedMeshRenderer>())
        //{
        //    rend.material.shader = CustomEmotesAPI.standardShader;
        //}
        EmoteLocation location = props[props.Count - 1].AddComponent<EmoteLocation>();
        location.joinSpot = joinSpot;
        location.owner = this;
        emoteLocations.Add(location);
        return props.Count - 1;
    }
    public void JoinEmoteSpot()
    {
        if (reservedEmoteSpot)
        {
            if (currentEmoteSpot)
            {
                if (currentEmoteSpot.GetComponent<EmoteLocation>().validPlayers != 0)
                {
                    currentEmoteSpot.GetComponent<EmoteLocation>().validPlayers--;
                }
                currentEmoteSpot.GetComponent<EmoteLocation>().SetColor();

            }
            currentEmoteSpot = reservedEmoteSpot;
            reservedEmoteSpot = null;
        }
        int spot = 0;
        for (; spot < currentEmoteSpot.transform.parent.GetComponentsInChildren<EmoteLocation>().Length; spot++)
        {
            if (currentEmoteSpot.transform.parent.GetComponentsInChildren<EmoteLocation>()[spot] == currentEmoteSpot.GetComponent<EmoteLocation>())
            {
                break;
            }
        }

        EmoteNetworker.instance.SyncJoinSpot(mapperBody.GetComponent<NetworkObject>().NetworkObjectId, currentEmoteSpot.GetComponentInParent<NetworkObject>().NetworkObjectId, currentEmoteSpot.GetComponent<EmoteLocation>().owner.worldProp, spot);
    }
    public void RemoveProp(int propPos)
    {
        GameObject.Destroy(props[propPos]);
    }
    public void SetAutoWalk(float speed, bool overrideBaseMovement, bool autoWalk)
    {
        autoWalkSpeed = speed;
        overrideMoveSpeed = overrideBaseMovement;
        this.autoWalk = autoWalk;
    }
    public void SetAutoWalk(float speed, bool overrideBaseMovement)
    {
        autoWalkSpeed = speed;
        overrideMoveSpeed = overrideBaseMovement;
        autoWalk = true;
    }
    internal IEnumerator waitForTwoFramesThenDisableA1()
    {
        yield return new WaitForEndOfFrame(); //haha we only wait for one frame lmao
        //basePlayerModelAnimator.enabled = false;
    }
    void OnDestroy()
    {
        if (playerController is not null)
        {
            playersToMappers.Remove(playerController.gameObject);
            if (worldProp)
            {
                return;
            }
            playersToMappers.Remove(mapperBody);
            try
            {
                currentClip.clip[0].ToString();
                NewAnimation(null);
                if (currentClip.syncronizeAnimation || currentClip.syncronizeAudio)
                {
                    if (CustomAnimationClip.syncPlayerCount[currentClip.syncPos] > 0)
                    {
                        CustomAnimationClip.syncPlayerCount[currentClip.syncPos]--;
                    }
                }
                if (primaryAudioClips[currentClip.syncPos][currEvent] != null)
                {
                    audioObject.GetComponent<AudioManager>().Stop();
                    if (currentClip.syncronizeAudio)
                    {
                        listOfCurrentEmoteAudio[currentClip.syncPos].Remove(audioObject.GetComponent<AudioSource>());
                    }
                }
                if (uniqueSpot != -1 && CustomAnimationClip.uniqueAnimations[currentClip.syncPos][uniqueSpot])
                {
                    CustomAnimationClip.uniqueAnimations[currentClip.syncPos][uniqueSpot] = false;
                    uniqueSpot = -1;
                }
                BoneMapper.allMappers.Remove(this);
                prevClip = currentClip;
                currentClip = null;
            }
            catch (Exception e)
            {
                //DebugClass.Log($"Had issues when destroying bonemapper: {e}");
                BoneMapper.allMappers.Remove(this);
            }
        }
    }
    public void UnlockBones(bool animatorEnabled = true)
    {
        transform.localPosition = Vector3.zero;
        transform.eulerAngles = bodyPrefab.transform.eulerAngles;
        //transform.localEulerAngles = new Vector3(90, 0, 0);
        foreach (var smr in basePlayerModelSMR)
        {
            for (int i = 0; i < smr.bones.Length; i++)
            {
                try
                {
                    if (smr.bones[i].gameObject.GetComponent<EmoteConstraint>())
                    {
                        smr.bones[i].gameObject.GetComponent<EmoteConstraint>().DeactivateConstraints();
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
        foreach (var item in cameraConstraints)
        {
            item.DeactivateConstraints();
        }
        foreach (var item in itemHolderConstraints)
        {
            item.DeactivateConstraints();
        }
        foreach (var item in additionalConstraints)
        {
            item.DeactivateConstraints();
        }
        if (thirdPersonConstraint is not null)
        {
            thirdPersonConstraint.DeactivateConstraints();
        }
        DeThirdPerson();
        //basePlayerModelAnimator.enabled = animatorEnabled;
    }
    public void LockBones()
    {
        UnlockBones();
        transform.localPosition = Vector3.zero;
        if (currentClip is not null)
        {
            foreach (var item in currentClip.soloIgnoredBones)
            {
                if (emoteSkeletonAnimator.GetBoneTransform(item))
                    dontAnimateUs.Add(emoteSkeletonAnimator.GetBoneTransform(item).name);
            }
            foreach (var item in currentClip.rootIgnoredBones)
            {
                if (emoteSkeletonAnimator.GetBoneTransform(item))
                    dontAnimateUs.Add(emoteSkeletonAnimator.GetBoneTransform(item).name);
                foreach (var bone in emoteSkeletonAnimator.GetBoneTransform(item).GetComponentsInChildren<Transform>())
                {
                    dontAnimateUs.Add(bone.name);
                }
            }
        }
        if (!jank)
        {
            emoteSkeletonSMR.enabled = true;
            var first = basePlayerModelSMR.First();
            foreach (var smr in basePlayerModelSMR)
            {
                for (int i = 0; i < smr.bones.Length; i++)
                {
                    try
                    {
                        EmoteConstraint ec = smr.bones[i].gameObject.GetComponent<EmoteConstraint>();
                        if (ec is not null)
                        {
                            if (!dontAnimateUs.Contains(smr.bones[i].name))
                            {
                                //DebugClass.Log($"-{i}---------{smr.bones[i].gameObject}");
                                if (smr == first)
                                {
                                    if (currentClip is not null)
                                    {
                                        ec.interpolates = currentClip.interpolation;
                                        ec.SetLocalTransforms(smr.bones[i] == smr.rootBone ? false : true);
                                    }
                                }
                                ec.ActivateConstraints(); //this is like, 99% of fps loss right here. Unfortunate
                            }
                            else if (dontAnimateUs.Contains(smr.bones[i].name))
                            {
                                //DebugClass.Log($"dontanimateme-{i}---------{smr.bones[i].gameObject}");
                                smr.bones[i].gameObject.GetComponent<EmoteConstraint>().DeactivateConstraints();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugClass.Log($"{e}");
                    }
                }
            }
            foreach (var item in itemHolderConstraints)
            {
                item.ActivateConstraints();
            }
            foreach (var item in additionalConstraints)
            {
                item.ActivateConstraints();
            }
            LockCameraStuff(local && ThirdPersonCheck());
        }
        else
        {
            //a1.enabled = false;

            StartCoroutine(waitForTwoFramesThenDisableA1());
        }
    }
    public bool isInThirdPerson = false;
    public int originalLayer = -1;
    public void LockCameraStuff(bool thirdPersonLock)
    {
        if (thirdPersonLock)
        {
            TurnOnThirdPerson();
        }
        else
        {
            if (currentClip is null)
            {
                return;
            }
            if (Settings.rootMotionType.Value != RootMotionType.None &&
(currentClip.lockType == AnimationClipParams.LockType.rootMotion || Settings.rootMotionType.Value == RootMotionType.All || currentClip.lockType == AnimationClipParams.LockType.lockHead))
            {
                foreach (var item in cameraConstraints)
                {
                    item.ActivateConstraints();
                }
            }
            else if (currentClip.lockType == AnimationClipParams.LockType.headBobbing && Settings.rootMotionType.Value != RootMotionType.None)
            {
                foreach (var item in cameraConstraints)
                {
                    item.ActivateConstraints();
                    if (item != cameraConstraints[cameraConstraints.Count - 1])
                    {
                        item.onlyY = true; //activateconstraints turns this off automatically so make sure to do this after we turn them on
                    }
                }
            }
        }
    }
    public void UnlockCameraStuff()
    {
        foreach (var item in cameraConstraints)
        {
            item.DeactivateConstraints();
        }
        thirdPersonConstraint.DeactivateConstraints();
        DeThirdPerson();
    }
    internal bool needToTurnOffShadows = true;
    internal bool needToTurnOffCosmetics = true;
    public void TurnOnThirdPerson()
    {
        playerController.localVisor.gameObject.SetActive(false);
        if (playerController.thisPlayerModel.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On)
        {
            needToTurnOffShadows = false;
        }
        playerController.thisPlayerModel.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //playerController.thisPlayerModelArms.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        if (originalLayer == -1)
        {
            originalLayer = playerController.thisPlayerModel.gameObject.layer;
            originalCullingMask = playerController.gameplayCamera.cullingMask;
        }
        playerController.thisPlayerModel.gameObject.layer = 1;
        playerController.grabDistance = 5.65f;
        playerController.gameplayCamera.cullingMask = StartOfRound.Instance.spectateCamera.cullingMask;//if you break the spectator culling mask, don't, stop, get some help
        thirdPersonConstraint.ActivateConstraints();
        isInThirdPerson = true;
        foreach (var item in cosmeticConstraints)
        {
            item.emoteBone = playerController.playerGlobalHead;
        }
        if (CustomEmotesAPI.MoreCompanyPresent)
        {
            try
            {
                needToTurnOffCosmetics = true;
                MoreCompanyCompat.TurnOnCosmetics(this);
            }
            catch (Exception e)
            {
                DebugClass.Log($"couldn't turn on cosmetics: {e}");
            }
        }
        if (CustomEmotesAPI.BetterEmotesPresent)
        {
            Transform t = playerController.transform.Find("ScavengerModel/LEGS");
            if (t is not null)
            {
                t.gameObject.layer = 31;
            }
        }
    }
    public void DeThirdPerson()
    {
        if (isInThirdPerson)
        {
            playerController.gameplayCamera.cullingMask = originalCullingMask;
            if (needToTurnOffShadows)
            {
                playerController.thisPlayerModel.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            needToTurnOffShadows = true;
            //playerController.thisPlayerModelArms.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            playerController.localVisor.gameObject.SetActive(true);
            playerController.thisPlayerModel.gameObject.layer = originalLayer;
            playerController.grabDistance = 3f;
            isInThirdPerson = false;
            foreach (var item in cosmeticConstraints)
            {
                item.emoteBone = originalCosmeticPosition.transform;
            }
            if (CustomEmotesAPI.MoreCompanyPresent && needToTurnOffCosmetics)
            {
                try
                {
                    MoreCompanyCompat.TurnOffCosmetics(this);
                }
                catch (Exception e)
                {
                    DebugClass.Log($"couldn't clear cosmetics: {e}");
                }
            }
            if (CustomEmotesAPI.BetterEmotesPresent)
            {
                Transform t = playerController.transform.Find("ScavengerModel/LEGS");
                if (t is not null)
                {
                    t.gameObject.layer = 0;
                }
            }
        }
    }
    public void AttachItemHolderToTransform(Transform target)
    {
        itemHolderPosition.gameObject.GetComponent<EmoteConstraint>().AddSource(itemHolderPosition, target);
        itemHolderPosition.gameObject.GetComponent<EmoteConstraint>().ActivateConstraints();
    }
}
