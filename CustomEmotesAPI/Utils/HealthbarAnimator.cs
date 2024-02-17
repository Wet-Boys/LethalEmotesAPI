using EmotesAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace LethalEmotesAPI.Utils
{
    [DefaultExecutionOrder(-1)]
    public class HealthbarAnimator : MonoBehaviour
    {
        internal static int activeRequests = 0;
        internal static List<EmoteConstraint> healthbarConstraints = new List<EmoteConstraint>();
        internal static Transform targetPoint;
        internal static bool permaOn = false;
        internal static bool setupComplete = false;
        internal static GameObject healthBarCameraObject;
        internal static void Setup(BoneMapper mapper)
        {
            if (CustomEmotesAPI.hudObject is not null && CustomEmotesAPI.hudAnimator == null)
            {
                healthBarCameraObject = GameObject.Instantiate(Assets.Load<GameObject>("assets/healthbarcamera.prefab"));
                healthBarCameraObject.AddComponent<HealthbarAnimator>();
                healthBarCameraObject.transform.SetParent(mapper.mapperBody.transform.parent);
                CustomEmotesAPI.hudAnimator = healthBarCameraObject.GetComponentInChildren<Animator>();
                CustomEmotesAPI.hudCamera = healthBarCameraObject.GetComponentInChildren<Camera>();
                CustomEmotesAPI.hudAnimator.transform.localEulerAngles = new Vector3(0, 180, 180);
                CustomEmotesAPI.hudAnimator.transform.localPosition = new Vector3(-822.5184f, -235.6528f, 1100);
                CustomEmotesAPI.hudObject.transform.localScale = new Vector3(1.175f, 1.175f, 1.175f);
                CustomEmotesAPI.hudObject.transform.localPosition = new Vector3(-425.0528f, 245.3589f, -0.0136f);
                GameObject g = GameObject.Instantiate<GameObject>(Assets.Load<GameObject>("@CustomEmotesAPI_customemotespackage:assets/animationreplacements/commando.prefab"));
                CustomEmotesAPI.hudAnimator.runtimeAnimatorController = g.GetComponent<Animator>().runtimeAnimatorController;
                CustomEmotesAPI.currentEmoteText = healthBarCameraObject.GetComponentInChildren<TextMeshPro>();
                targetPoint = CustomEmotesAPI.hudAnimator.transform.Find("ScavengerModel/spine");
                SkinnedMeshRenderer smr = CustomEmotesAPI.hudAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
                int startingXPoint = 0;
                healthbarConstraints.Clear();
                for (int i = 0; i < mapper.basePlayerModelSMR[0].bones.Length; i++)
                {
                    for (int x = startingXPoint; x < smr.bones.Length; x++)
                    {
                        if (mapper.basePlayerModelSMR[0].bones[i].name == smr.bones[x].name && !smr.bones[x].gameObject.GetComponent<EmoteConstraint>())
                        {
                            startingXPoint = x;
                            EmoteConstraint e = smr.bones[x].gameObject.AddComponent<EmoteConstraint>();
                            e.AddSource(ref smr.bones[x], ref mapper.basePlayerModelSMR[0].bones[i]);
                            e.revertTransform = mapper.revertTransform;
                            e.SetLocalTransforms(true);
                            healthbarConstraints.Add(e);
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
                SetHealthbarPosition();
                setupComplete = true;
            }

        }
        public static void StartHealthbarAnimateRequest()
        {
            activeRequests++;
            SetHealthbarPosition();
        }
        public static void FinishHealthbarAnimateRequest()
        {
            activeRequests--;
            if (activeRequests < 0)
            {
                activeRequests = 0;
            }
            SetHealthbarPosition();
        }
        internal static void SetHealthbarPosition()
        {
            if (activeRequests != 0 || permaOn)
            {
                if (CustomEmotesAPI.hudObject is not null)
                {
                    healthBarCameraObject.SetActive(true);
                    CustomEmotesAPI.hudAnimator.transform.localPosition = new Vector3(-822.5184f, -235.6528f, 1074.747f);
                    CustomEmotesAPI.baseHUDObject.SetActive(false);
                    CustomEmotesAPI.selfRedHUDObject.SetActive(false);
                }
            }
            else
            {
                if (CustomEmotesAPI.hudObject is not null)
                {
                    CustomEmotesAPI.hudAnimator.transform.localPosition = new Vector3(-822, -235, 1100);
                    CustomEmotesAPI.baseHUDObject.SetActive(true);
                    CustomEmotesAPI.selfRedHUDObject.SetActive(true);
                    healthBarCameraObject.SetActive(false);
                }
            }

        }
        void Start()
        {
            StartCoroutine(FixPosition());
        }
        IEnumerator FixPosition()
        {
            yield return new WaitForEndOfFrame();
            CustomEmotesAPI.hudObject.transform.localPosition = CustomEmotesAPI.baseHUDObject.transform.localPosition;
            CustomEmotesAPI.currentEmoteText.color = new Color(.5f, .5f, .5f, .5f);
            CustomEmotesAPI.currentEmoteText.text = "";
        }
        void LateUpdate()
        {
            if (setupComplete && (activeRequests != 0 || permaOn))
            {
                foreach (var item in healthbarConstraints)
                {
                    try
                    {
                        item.constraintActive = true;
                        item.ActUponConstraints();
                        item.constraintActive = false;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
