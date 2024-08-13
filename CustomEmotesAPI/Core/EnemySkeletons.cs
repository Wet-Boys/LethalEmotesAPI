using AdvancedCompany.Game;
using EmotesAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Core
{
    internal class EnemySkeletons
    {
        internal static IEnumerator AttachCoilHeadSpring(Transform head, BoneMapper b)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Transform t = b.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).Find("springBone.002").Find("springBone.002_end");
            GameObject g = new GameObject();
            g.transform.SetParent(t);
            g.transform.localPosition = Vector3.zero;
            g.transform.localEulerAngles = new Vector3(-90, 0, 0);
            b.additionalConstraints.Add(EmoteConstraint.AddConstraint(head.gameObject, b, g.transform, true));

        }
        internal static IEnumerator SetupMaskedEmoteSkeletonAfterAFewFrames(MaskedPlayerEnemy self)
        {
            //Advanced company does something with the bones and it doesn't appreciate my bones already existing so we wait
            yield return new WaitForEndOfFrame();
            AnimationReplacements.Import(self.gameObject, "assets/customstuff/scavEmoteSkeleton.prefab", [0]);
        }
        internal static IEnumerator SetupSkeletonAfterFrame(GameObject g, string prefab, int[] pos, int frames = 1)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            try
            {
                AnimationReplacements.Import(g, prefab, pos);
            }
            catch (Exception e)
            {
                DebugClass.Log($"had issue importing {g}\n\n {e}");
            }
        }
        private void MaskedPlayerEnemyStart(Action<MaskedPlayerEnemy> orig, MaskedPlayerEnemy self)
        {
            CustomEmotesAPI.localMapper.StartCoroutine(SetupMaskedEmoteSkeletonAfterAFewFrames(self));
            orig(self);
        }
        private static Hook MaskedPlayerEnemyStartHook;
        internal static IEnumerator SetupSkeleton(EnemyAI self)
        {
            try
            {
                switch (self.enemyType.enemyName)
                {
                    case "Bunker Spider":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/sandspider4.prefab", [0]).scale = 1.6f;
                        break;
                    case "Hoarding bug":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/hoarderbug.prefab", [0]).scale = 1.1f;
                        break;
                    case "Manticoil":
                        //probably should add an emote skeleton here eventually.....
                        break;
                    case "Earth Leviathan":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/sandworm1.prefab", [0]);
                        break;
                    case "Crawler":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/crawler3.prefab", [0]);
                        break;
                    case "Blob":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/blob1.prefab", [0]);
                        break;
                    case "Centipede":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/centipede1.prefab", [0]);
                        break;
                    case "Nutcracker":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/nutcracker3.prefab", [0]);
                        break;
                    case "Baboon hawk":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/baboonhawk1.prefab", [0]);
                        break;
                    case "Puffer":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/pufferenemy.prefab", [0]);
                        break;
                    case "Spring":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/springman4.prefab", [0]);
                        BoneMapper b = self.GetComponentInChildren<BoneMapper>();
                        Transform head = self.transform.Find("SpringManModel").Find("Head");
                        CustomEmotesAPI.localMapper.StartCoroutine(AttachCoilHeadSpring(head, b));
                        break;
                    case "Jester":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/jester2.prefab", [0, 1, 2]).scale = 1.3f;
                        break;
                    case "Flowerman":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/flowerman1.prefab", [0]).scale = 1.6f;
                        break;
                    case "Girl":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/dressgirl.prefab", [0]);
                        break;
                    case "MouthDog":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/mouthdog3.prefab", [0]);
                        break;
                    case "ForestGiant":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/giant5.prefab", [0]);
                        break;
                    case "Why Does The Masked Not Call The Base Start Function :(":
                        break;
                    case "Shy guy":
                        AnimationReplacements.Import(self.gameObject, "assets/fineilldoitmyself/shyguy.prefab", [0]);
                        break;
                    case "SkibidiToilet":
                        AnimationReplacements.Import(self.gameObject, "assets/fineilldoitmyself/skibidi.prefab", [0]);
                        break;
                    case "Demogorgon":
                        AnimationReplacements.Import(self.gameObject, "assets/fineilldoitmyself/DemoGorgon13.prefab", [0, 1, 2, 3]);
                        break;
                    case "Peeper":
                        AnimationReplacements.Import(self.gameObject, "assets/fineilldoitmyself/Peeper2.prefab", [0]);
                        break;
                    case "RadMech":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/Mech1.prefab", [0]);
                        break;
                    case "Butler":
                        AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/Butler.prefab", [0]);
                        break;
                    case "Tulip Snake":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/enemyskeletons/TulipSnake.prefab", [3]));
                        break;


                    case "HarpGhost":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Ghost1.prefab", [0]));
                        break;
                    case "EnforcerGhost":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Ghost1.prefab", [0]));
                        break;
                    case "BagpipeGhost":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Ghost1.prefab", [0]));
                        break;
                    case "SlendermanEnemy":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Slender3.prefab", [0]));
                        break;
                    case "RedWoodGiant":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/XuGiant.prefab", [10]));
                        break;
                    case "DriftWoodGiant":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Driftwood.prefab", [0]));
                        break;
                    case "Foxy":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Foxy4.prefab", [0]));
                        break;
                    case "The Fiend":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Fiend1.prefab", [0]));
                        break;
                    case "Siren Head":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/SirenHead2.prefab", [0]));
                        break;
                    case "Football":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Football.prefab", [0], 0));
                        break;
                    case "Sentinel":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/fineilldoitmyself/NewSet/Sentinel.prefab", [0]));
                        break;
                    case "Bush Wolf":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/enemyskeletons/bushfox.prefab", [0]));
                        break;
                    case "Clay Surgeon":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.gameObject, "assets/enemyskeletons/claysurgeon.prefab", [0]));
                        break;
                    case "InternNPC":
                        CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeletonAfterFrame(self.transform.parent.gameObject, "assets/customstuff/scavEmoteSkeleton.prefab", [0]));
                        break;
                    default:
                        //DebugClass.Log($"enemy name: {self.enemyType.enemyName}");
                        break;
                }
            }
            catch (Exception)
            {
                DebugClass.Log($"couldn't setup an enemy?");
            }
            yield return new WaitForEndOfFrame();
        }


        private void EnemyAIStart(Action<EnemyAI> orig, EnemyAI self)
        {

            if (CustomEmotesAPI.localMapper is not null)
            {
                CustomEmotesAPI.localMapper.StartCoroutine(SetupSkeleton(self));
            }
            orig(self);
        }
        private static Hook EnemyAIStartHook;

        internal static void SetupEnemyHooks()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(EnemyAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(EnemyAIStart), EnemyAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(MaskedPlayerEnemy), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(MaskedPlayerEnemyStart), MaskedPlayerEnemyStartHook);
        }
    }
}
