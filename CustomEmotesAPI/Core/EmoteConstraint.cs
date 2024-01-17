using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[DefaultExecutionOrder(-2)]
public class EmoteConstraint : MonoBehaviour
{
    public Transform originalBone;
    public Transform emoteBone;
    Vector3 originalPosition;
    Quaternion originalRotation;
    public bool constraintActive = false;
    public bool revertTransform;
    bool firstTime = true;
    bool firstTime2 = true;
    bool hasEverActivatedConstraints = false;
    public bool onlyY = false;
    public bool debug = false;
    public bool localTransforms = false;
    void LateUpdate()
    {
        ActUponConstraints();
    }
    public void ActUponConstraints()
    {
        if (constraintActive)
        {
            if (localTransforms)
            {
                if (onlyY)
                {
                    originalBone.localPosition = new Vector3(originalBone.localPosition.x, emoteBone.localPosition.y, originalBone.localPosition.z);
                }
                else
                {
                    originalBone.localPosition = emoteBone.localPosition;
                    originalBone.localRotation = emoteBone.localRotation;
                }
            }
            else
            {
                if (onlyY)
                {
                    originalBone.position = new Vector3(originalBone.position.x, emoteBone.position.y, originalBone.position.z);
                }
                else
                {
                    originalBone.position = emoteBone.position;
                    originalBone.rotation = emoteBone.rotation;
                }
            }
        }
    }
    public void ActivateConstraints()
    {
        if (!constraintActive)
        {
            if (firstTime2)
            {
                firstTime2 = false;
                gameObject.GetComponent<MonoBehaviour>().StartCoroutine(FirstTimeActiveFix(this));
            }
            else
            {
                originalPosition = originalBone.localPosition;
                originalRotation = originalBone.localRotation;
                hasEverActivatedConstraints = true;
                constraintActive = true;
                onlyY = false;
            }
        }
    }
    internal IEnumerator FirstTimeActiveFix(EmoteConstraint e)//this is used for some enemies that just don't like to have their emote constraints work unless I do this? Not really sure why but it's not a huge deal tbh
    {
        e.enabled = false;
        yield return new WaitForEndOfFrame();
        e.enabled = true;
        if (e.onlyY)
        {
            e.ActivateConstraints();
            e.onlyY = true;
        }
        else
        {
            e.ActivateConstraints();
        }
    }
    public void DeactivateConstraints()
    {
        constraintActive = false;
        if (firstTime || !revertTransform || !hasEverActivatedConstraints)
        {
            firstTime = false;
        }
        else
        {
            originalBone.localPosition = originalPosition;
            originalBone.localRotation = originalRotation;
        }
    }
    internal void AddSource(ref Transform originalBone, ref Transform emoteBone)
    {
        this.originalBone = originalBone;
        this.emoteBone = emoteBone;
    }
    internal void AddSource(Transform originalBone, Transform emoteBone)
    {
        this.originalBone = originalBone;
        this.emoteBone = emoteBone;
    }
    internal static EmoteConstraint AddConstraint(GameObject gameObject, BoneMapper mapper, Transform target)
    {
        EmoteConstraint constraint = gameObject.AddComponent<EmoteConstraint>();
        constraint.AddSource(gameObject.transform, target);
        constraint.revertTransform = mapper.revertTransform;
        return constraint;
    }

    void Start()
    {
        ActivateConstraints();
        DeactivateConstraints();
    }
}