using EmotesAPI;
using LethalEmotesAPI.Core;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LethalEmotesAPI.Utils
{
    internal class DebugCommands
    {
        private void QuickMenuManagerStart(Action<QuickMenuManager> orig, QuickMenuManager self)
        {
            quickMenuManagerInstance = self;
            DebugClass.Log($"setting self to {self}");
            orig(self);
            DebugClass.Log($"enemy count: {self.testAllEnemiesLevel.Enemies.Count}     {self.testAllEnemiesLevel.OutsideEnemies.Count}   {self.testAllEnemiesLevel.DaytimeEnemies.Count}");
            foreach (var item in self.testAllEnemiesLevel.Enemies)
            {
                RegisterEnemyType(item.enemyType);
            }
            foreach (var item in self.testAllEnemiesLevel.OutsideEnemies)
            {
                RegisterEnemyType(item.enemyType);
            }
            foreach (var item in self.testAllEnemiesLevel.DaytimeEnemies)
            {
                RegisterEnemyType(item.enemyType);
            }
        }
        internal static EnemyType flowerman;
        internal static EnemyType crawler;
        internal static EnemyType hoardingbug;
        internal static EnemyType centipede;
        internal static EnemyType spider;
        internal static EnemyType puffer;
        internal static EnemyType jester;
        internal static EnemyType blob;
        internal static EnemyType child;
        internal static EnemyType spring;
        internal static EnemyType nutcracker;
        internal static EnemyType masked;
        internal static EnemyType dog;
        internal static EnemyType worm;
        internal static EnemyType giant;
        internal static EnemyType baboonhawk;
        internal static EnemyType manticoil;
        internal static void RegisterEnemyType(EnemyType type)
        {
            switch (type.enemyName)
            {
                case "Flowerman":
                    flowerman = type;
                    break;
                case "Crawler":
                    crawler = type;
                    break;
                case "Hoarding bug":
                    hoardingbug = type;
                    break;
                case "Centipede":
                    centipede = type;
                    break;
                case "Bunker Spider":
                    spider = type;
                    break;
                case "Puffer":
                    puffer = type;
                    break;
                case "Jester":
                    jester = type;
                    break;
                case "Blob":
                    blob = type;
                    break;
                case "Girl":
                    child = type;
                    break;
                case "Spring":
                    spring = type;
                    break;
                case "Nutcracker":
                    nutcracker = type;
                    break;
                case "Masked":
                    masked = type;
                    break;
                case "MouthDog":
                    dog = type;
                    break;
                case "Earth Leviathan":
                    worm = type;
                    break;
                case "ForestGiant":
                    giant = type;
                    break;
                case "Baboon hawk":
                    baboonhawk = type;
                    break;
                case "Manticoil":
                    manticoil = type;
                    break;
                default:
                    break;
            }
        }

        private static Hook QuickMenuManagerStartHook;
        internal static void Debugcommands()
        {
            CustomEmotesAPI.instance.SetupHook(typeof(QuickMenuManager), typeof(DebugCommands), "Start", BindingFlags.NonPublic, nameof(QuickMenuManagerStart), QuickMenuManagerStartHook);

            CustomEmotesAPI.AddNonAnimatingEmote("spawn_shrimp");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_spider");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_loot bug");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_brackussy");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_thumper");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_slime");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_girl");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_lizard");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_nutcracker");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_coil head");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_jester");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_masked");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_dog");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_giant");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_worm");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_baboon hawk thing");
            CustomEmotesAPI.AddNonAnimatingEmote("spawn_manticoil");
            CustomEmotesAPI.AddNonAnimatingEmote("enemies test dance");
            CustomEmotesAPI.AddNonAnimatingEmote("enemies random dance");
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
        }
        private static QuickMenuManager quickMenuManagerInstance;
        private static void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            switch (newAnimation)
            {
                case "spawn_shrimp":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, centipede);
                    break;
                case "spawn_spider":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, spider);
                    break;
                case "spawn_loot bug":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, hoardingbug);
                    break;
                case "spawn_brackussy":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, flowerman);
                    break;
                case "spawn_thumper":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, crawler);
                    break;
                case "spawn_slime":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, blob);
                    break;
                case "spawn_girl":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, child);
                    break;
                case "spawn_lizard":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, puffer);
                    break;
                case "spawn_nutcracker":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, nutcracker);
                    break;
                case "spawn_coil head":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, spring);
                    break;
                case "spawn_jester":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, jester);
                    break;
                case "spawn_masked":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, masked);
                    break;
                case "spawn_dog":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, dog);
                    break;
                case "spawn_giant":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, giant);
                    break;
                case "spawn_worm":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, worm);
                    break;
                case "spawn_baboon hawk thing":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, baboonhawk);
                    break;
                case "spawn_manticoil":
                    RoundManager.Instance.SpawnEnemyGameObject(mapper.transform.position, 0, 1, manticoil);
                    break;
                case "enemies test dance":
                    foreach (var item in BoneMapper.allMappers)
                    {
                        if (item.isEnemy)
                        {
                            CustomEmotesAPI.PlayAnimation("JJK Lost in Paradise", item);
                        }
                    }
                    break;
                case "enemies random dance":
                    foreach (var item in BoneMapper.allMappers)
                    {
                        if (item.isEnemy)
                        {
                            int rand = UnityEngine.Random.Range(0, CustomEmotesAPI.randomClipList.Count);
                            CustomEmotesAPI.PlayAnimation(CustomEmotesAPI.randomClipList[rand], item);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
