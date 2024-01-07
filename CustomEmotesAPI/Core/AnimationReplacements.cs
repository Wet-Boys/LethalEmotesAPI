using System;
using UnityEngine;
using System.Security.Permissions;
using System.Text;
using GameNetcodeStuff;
using LethalEmotesAPI.Utils;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
internal static class AnimationReplacements
{
    internal static GameObject g;
    internal static void RunAll()
    {
        ChangeAnims();
    }
    internal static bool setup = false;
    internal static void Import(GameObject prefab, string skeleton, int[] pos, bool hidemesh = true)
    {
        Assets.Load<GameObject>(skeleton).GetComponent<Animator>().runtimeAnimatorController = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab")).GetComponent<Animator>().runtimeAnimatorController;
        AnimationReplacements.ApplyAnimationStuff(prefab, GameObject.Instantiate(Assets.Load<GameObject>(skeleton)), pos, hidemesh, revertBonePositions: true);
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
        if (meshes[0].bones.Length == 0)
        {
            sb.Append("No bones");
        }
        else
        {
            sb.Append("[");
            foreach (var bone in meshes[0].bones)
            {
                sb.Append($"'{bone.name}', ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");
        }
        sb.Append("\n\n");
        DebugClass.Log(sb.ToString());
    }
    internal static void ChangeAnims()
    {

    }
    internal static void ApplyAnimationStuff(GameObject bodyPrefab, string resource, int[] pos)
    {
        GameObject animcontroller = Assets.Load<GameObject>(resource);
        ApplyAnimationStuff(bodyPrefab, animcontroller, pos);
    }

    internal static void ApplyAnimationStuff(GameObject bodyPrefab, GameObject animcontroller, int[] pos, bool hidemeshes = true, bool jank = false, bool revertBonePositions = false)
    {
        try
        {
            if (!animcontroller.GetComponentInChildren<Animator>().avatar.isHuman)
            {
                DebugClass.Log($"{animcontroller}'s avatar isn't humanoid, please fix it in unity!");
                return;
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


        //since this game is jank and has A UNIQUE SKINNEDMESHRENDERER FOR EACH LOD, I am just going to enforce proper SMR labeling. This probably won't be that big of a deal since I imagine the need for people setting up their own emote skeletons will be FAR less than ROR2
        //try
        //{
        //    int matchingBones = 0;
        //    while (true)
        //    {
        //        foreach (var smr1bone in smr1.bones) //smr is SkinnedMeshRenderer
        //        {
        //            foreach (var smr2bone in smr2.bones)
        //            {
        //                if (smr1bone.name == smr2bone.name)
        //                {
        //                    matchingBones++;
        //                }
        //            }
        //        }
        //        if (matchingBones < 1 && pos + 1 < bodyPrefab.GetComponentsInChildren<SkinnedMeshRenderer>().Length)
        //        {
        //            pos++;
        //            smr2 = bodyPrefab.GetComponentsInChildren<SkinnedMeshRenderer>()[pos];
        //            matchingBones = 0;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}
        //catch (Exception e)
        //{
        //    DebugClass.Log($"Had issue while checking matching bones: {e}");
        //    throw;
        //}

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
            float banditScale = Vector3.Distance(nuts.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).position, nuts.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftFoot).position);
            float currScale = Vector3.Distance(animcontroller.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).position, animcontroller.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftFoot).position);
            test.scale = currScale / banditScale;
            test.h = bodyPrefab.GetComponentInChildren<PlayerControllerB>().health;
            test.model = modelTransform.gameObject;
        }
        catch (Exception e)
        {
            DebugClass.Log($"Had issue when setting up BoneMapper settings 2: {e}");
            throw;
        }
        test.revertTransform = revertBonePositions;

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