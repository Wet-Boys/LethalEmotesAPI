using System;
using UnityEngine;
using System.Security.Permissions;
using System.Text;
using GameNetcodeStuff;
using LethalEmotesAPI.Utils;
using EmotesAPI;
using UnityEngine.UIElements;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
internal static class AnimationReplacements
{
    internal static GameObject g;
    internal static void RunAll()
    {
        ChangeAnims();
    }
    internal static bool setup = false;
    internal static BoneMapper Import(GameObject prefab, string skeleton, int[] pos, bool hidemesh = true)
    {
        GameObject g = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab"));
        GameObject emoteSkeleton = GameObject.Instantiate(Assets.Load<GameObject>(skeleton));
        emoteSkeleton.GetComponent<Animator>().runtimeAnimatorController = g.GetComponent<Animator>().runtimeAnimatorController;
        BoneMapper b = AnimationReplacements.ApplyAnimationStuff(prefab, emoteSkeleton, pos, hidemesh, revertBonePositions: true);
        g.transform.SetParent(emoteSkeleton.transform);
        return b;
    }
    public static void DebugBones(GameObject fab)
    {
        var meshes = fab.GetComponentsInChildren<SkinnedMeshRenderer>();
        StringBuilder sb = new StringBuilder();
        sb.Append($"rendererererer: {meshes[0]}\n");
        sb.Append($"bone count: {meshes[0].bones.Length}\n");
        sb.Append($"mesh count: {meshes.Length}\n");
        sb.Append($"root bone: {meshes[0].rootBone.name}\n");
        sb.Append($"{fab.ToString()}:\n");
        foreach (var item in meshes)
        {
            if (item.bones.Length == 0)
            {
                sb.Append("No bones");
            }
            else
            {
                sb.Append("[");
                foreach (var bone in item.bones)
                {
                    sb.Append($"'{bone.name}', ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append("]");
            }
            sb.Append("\n\n");
            DebugClass.Log(sb.ToString());
        }

    }
    internal static void ChangeAnims()
    {

    }
    internal static void ApplyAnimationStuff(GameObject bodyPrefab, string resource, int[] pos)
    {
        GameObject animcontroller = Assets.Load<GameObject>(resource);
        ApplyAnimationStuff(bodyPrefab, animcontroller, pos);
    }

    internal static BoneMapper ApplyAnimationStuff(GameObject bodyPrefab, GameObject animcontroller, int[] pos, bool hidemeshes = true, bool jank = false, bool revertBonePositions = false)
    {
        try
        {
            if (!animcontroller.GetComponentInChildren<Animator>().avatar.isHuman)
            {
                DebugClass.Log($"{animcontroller}'s avatar isn't humanoid, please fix it in unity!");
                return null;
            }
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had issue checking if avatar was humanoid: {e}");
            throw;
        }
        try
        {
            if (hidemeshes)
            {
                foreach (var item in animcontroller.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    item.sharedMesh = null;
                }
                foreach (var item in animcontroller.GetComponentsInChildren<MeshFilter>())
                {
                    item.sharedMesh = null;
                }
            }
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had trouble while hiding meshes: {e}");
            throw;
        }

        Transform modelTransform;
        modelTransform = bodyPrefab.GetComponentInChildren<Animator>().transform;


        SkinnedMeshRenderer smr1;
        SkinnedMeshRenderer[] smr2 = new SkinnedMeshRenderer[pos.Length];
        try
        {
            smr1 = animcontroller.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had trouble setting emote skeletons SkinnedMeshRenderer: {e}");
            throw;
        }
        try
        {
            for (int i = 0; i < pos.Length; i++)
            {
                smr2[i] = bodyPrefab.GetComponentsInChildren<SkinnedMeshRenderer>()[pos[i]];
            }
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had trouble setting the original skeleton's skinned mesh renderer: {e}");
            throw;
        }

        var test = animcontroller.AddComponent<BoneMapper>();
        try
        {
            test.jank = jank;
            test.emoteSkeletonSMR = smr1;
            test.basePlayerModelSMR = smr2;
            test.bodyPrefab = bodyPrefab;
            test.basePlayerModelAnimator = modelTransform.GetComponentInChildren<Animator>();
            test.emoteSkeletonAnimator = animcontroller.GetComponentInChildren<Animator>();
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had issue when setting up BoneMapper settings 1: {e}");
            throw;
        }
        try
        {
            var nuts = Assets.Load<GameObject>("assets/customstuff/scavEmoteSkeleton.prefab");
            nuts.transform.localScale = new Vector3(1.1216f, 1.1216f, 1.1216f);
            float banditScale = Vector3.Distance(nuts.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).position, nuts.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftFoot).position);
            float currScale = Vector3.Distance(animcontroller.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).position, animcontroller.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftFoot).position);
            test.scale = currScale / banditScale;
            //todo health
            //test.h = bodyPrefab.GetComponentInChildren<PlayerControllerB>().health;
            test.model = modelTransform.gameObject;
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had issue when setting up BoneMapper settings 2: {e}");
            throw;
        }
        try
        {
            animcontroller.transform.parent = modelTransform;
            animcontroller.transform.localPosition = Vector3.zero;
            animcontroller.transform.eulerAngles = bodyPrefab.transform.eulerAngles;
            //animcontroller.transform.localEulerAngles = new Vector3(90, 0, 0);
            animcontroller.transform.localScale = Vector3.one;
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had trouble setting emote skeletons parent: {e}");
            throw;
        }
        test.revertTransform = revertBonePositions;
        return test;
    }
}

public struct WorldProp
{
    internal GameObject prop;
    internal JoinSpot[] joinSpots;
    public WorldProp(GameObject _prop, JoinSpot[] _joinSpots)
    {
        prop = _prop;
        joinSpots = _joinSpots;
    }
}
public enum TempThirdPerson
{
    none,
    on,
    off
}