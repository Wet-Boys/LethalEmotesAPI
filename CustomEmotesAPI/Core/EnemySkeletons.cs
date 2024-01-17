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

        private void EnemyAiStart(Action<ForestGiantAI> orig, ForestGiantAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/giant5.prefab", [0]);
            orig(self);
        }
        private static Hook EnemyAiStartHook;

        private void BlobAIEnemyAiStart(Action<BlobAI> orig, BlobAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/blob1.prefab", [0]);
            orig(self);
        }
        private static Hook BlobAIEnemyAiStartHook;


        private void BaboonBirdAIStart(Action<BaboonBirdAI> orig, BaboonBirdAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/baboonhawk1.prefab", [0]);
            orig(self);
        }
        private static Hook BaboonBirdAIStartHook;

        private void MouthDogAIStart(Action<MouthDogAI> orig, MouthDogAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/mouthdog3.prefab", [0]);
            orig(self);
        }
        private static Hook MouthDogAIStartHook;
        private void CentipedeAIStart(Action<CentipedeAI> orig, CentipedeAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/centipede1.prefab", [0]);
            orig(self);
        }
        private static Hook CentipedeAIStartHook;
        private void SandSpiderAIStart(Action<SandSpiderAI> orig, SandSpiderAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/sandspider4.prefab", [0]).scale = 1.6f;
            orig(self);
        }
        private static Hook SandSpiderAIStartHook;
        private void HoarderBugAIStart(Action<HoarderBugAI> orig, HoarderBugAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/hoarderbug.prefab", [0]).scale = 1.1f;
            orig(self);
        }
        private static Hook HoarderBugAIStartHook;
        private void FlowermanAIStart(Action<FlowermanAI> orig, FlowermanAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/flowerman1.prefab", [0]).scale = 1.6f;
            orig(self);
        }
        private static Hook FlowermanAIStartHook;
        private void CrawlerAIStart(Action<CrawlerAI> orig, CrawlerAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/crawler3.prefab", [0]);
            orig(self);
        }
        private static Hook CrawlerAIStartHook;
        private void DressGirlAIStart(Action<DressGirlAI> orig, DressGirlAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/dressgirl.prefab", [0]);
            orig(self);
        }
        private static Hook DressGirlAIStartHook;
        private void PufferAIStart(Action<PufferAI> orig, PufferAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/pufferenemy.prefab", [0]);
            orig(self);
        }
        private static Hook PufferAIStartHook;
        private void NutcrackerEnemyAIStart(Action<NutcrackerEnemyAI> orig, NutcrackerEnemyAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/nutcracker3.prefab", [0]);
            orig(self);
        }
        private static Hook NutcrackerEnemyAIStartHook;
        private void SpringManAIStart(Action<SpringManAI> orig, SpringManAI self)
        {
            if (self.enemyType.enemyName == "Spring")
            {
                AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/springman4.prefab", [0]);
                BoneMapper b = self.GetComponentInChildren<BoneMapper>();
                Transform head = self.transform.Find("SpringManModel").Find("Head");
                CustomEmotesAPI.localMapper.StartCoroutine(AttachCoilHeadSpring(head,b));
            }
            orig(self);
        }
        internal IEnumerator AttachCoilHeadSpring(Transform head, BoneMapper b)
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
        private static Hook SpringManAIStartHook;
        private void JesterAIStart(Action<JesterAI> orig, JesterAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/jester2.prefab", [0, 1, 2]).scale = 1.3f;
            orig(self);
        }
        private static Hook JesterAIStartHook;
        private void SandWormAIStart(Action<SandWormAI> orig, SandWormAI self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/enemyskeletons/sandworm1.prefab", [0]);
            orig(self);
        }
        private static Hook SandWormAIStartHook;
        private void MaskedPlayerEnemyStart(Action<MaskedPlayerEnemy> orig, MaskedPlayerEnemy self)
        {
            AnimationReplacements.Import(self.gameObject, "assets/customstuff/scavEmoteSkeleton.prefab", [0]);
            orig(self);
        }
        private static Hook MaskedPlayerEnemyStartHook;













        internal static void SetupEnemyHooks()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(ForestGiantAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(EnemyAiStart), EnemyAiStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(BlobAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(BlobAIEnemyAiStart), BlobAIEnemyAiStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(BaboonBirdAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(BaboonBirdAIStart), BaboonBirdAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(MouthDogAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(MouthDogAIStart), MouthDogAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(CentipedeAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(CentipedeAIStart), CentipedeAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(SandSpiderAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(SandSpiderAIStart), SandSpiderAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(HoarderBugAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(HoarderBugAIStart), HoarderBugAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(FlowermanAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(FlowermanAIStart), FlowermanAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(CrawlerAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(CrawlerAIStart), CrawlerAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(DressGirlAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(DressGirlAIStart), DressGirlAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(PufferAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(PufferAIStart), PufferAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(NutcrackerEnemyAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(NutcrackerEnemyAIStart), NutcrackerEnemyAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(JesterAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(JesterAIStart), JesterAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(SandWormAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(SandWormAIStart), SandWormAIStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(MaskedPlayerEnemy), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(MaskedPlayerEnemyStart), MaskedPlayerEnemyStartHook);
            CustomEmotesAPI.instance.SetupHook(typeof(SpringManAI), typeof(EnemySkeletons), "Start", BindingFlags.Public, nameof(SpringManAIStart), SpringManAIStartHook);
        }
    }
}
