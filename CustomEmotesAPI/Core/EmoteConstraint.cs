using LethalEmotesAPI.Core;
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
    public bool localTransforms { get; private set; } = false;
    public bool forceGlobalTransforms = false;
    internal bool needToFix = true;
    internal float constraintPower = 0;
    internal bool interpolates = false;
    Vector3 posBeforeStopping;
    Quaternion rotBeforeStopping;
    public bool SetLocalTransforms(bool input)
    {
        localTransforms = !forceGlobalTransforms && input;
        return localTransforms;
    }
    void LateUpdate()
    {
        ActUponConstraints();
    }
    const float multiplier = .5f;
    public void ActUponConstraints()
    {
        if (constraintActive)
        {
            if (constraintPower < 1)
            {
                constraintPower += Time.deltaTime * multiplier;
                if (constraintPower > 1)
                {
                    constraintPower = 1;
                }
            }
            AnimateBone();
        }
        else
        {
            if (constraintPower > 0)
            {
                constraintPower -= Time.deltaTime * multiplier;
                if (constraintPower < 0)
                {
                    constraintPower = 0;
                }
                RevertBone();
            }
        }

    }
    public void AnimateBone()//yes it's kinda duplicate code, but it's in favor of doing less checks per frame since a lot of items do these steps
    {
        if (localTransforms)
        {
            if (onlyY)
            {
                originalBone.localPosition = new Vector3(originalBone.localPosition.x, Mathf.Lerp(originalBone.localPosition.y, emoteBone.localPosition.y, constraintPower), originalBone.localPosition.z);
            }
            else
            {
                originalBone.localPosition = Vector3.Lerp(originalBone.localPosition, emoteBone.localPosition, constraintPower);
                originalBone.localRotation = Quaternion.Lerp(originalBone.localRotation, emoteBone.localRotation, constraintPower);
            }
        }
        else
        {
            if (onlyY)
            {
                originalBone.position = new Vector3(originalBone.position.x, Mathf.Lerp(originalBone.position.y, emoteBone.position.y, constraintPower), originalBone.position.z);
            }
            else
            {
                originalBone.position = Vector3.Lerp(originalBone.position, emoteBone.position, constraintPower);
                originalBone.rotation = Quaternion.Lerp(originalBone.rotation, emoteBone.rotation, constraintPower);
            }
        }
        posBeforeStopping = emoteBone.localPosition;
        rotBeforeStopping = emoteBone.localRotation;
    }
    public void RevertBone() //yes it's kinda duplicate code, but it's in favor of doing less checks per frame since a lot of items do these steps
    {
        if (onlyY)
        {
            originalBone.localPosition = new Vector3(originalPosition.x, Mathf.Lerp(originalPosition.y, posBeforeStopping.y, constraintPower), originalPosition.z);
        }
        else
        {
            originalBone.localPosition = Vector3.Lerp(originalPosition, posBeforeStopping, constraintPower);
            originalBone.localRotation = Quaternion.Lerp(originalRotation, rotBeforeStopping, constraintPower);
        }
    }
    public void ActivateConstraints()
    {
        if (!constraintActive && emoteBone is not null)
        {
            //Debug.Log($"{localTransforms}   {transform.name}");
            constraintPower = interpolates ? 0 : 1;
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
        if (emoteBone.GetComponent<BoneRef>() is not null)
        {
            emoteBone.GetComponent<BoneRef>().target = originalBone;
        }
        else
        {
            emoteBone.gameObject.AddComponent<BoneRef>().target = originalBone;
        }
    }
    internal void AddSource(Transform originalBone, Transform emoteBone)
    {
        this.originalBone = originalBone;
        this.emoteBone = emoteBone;
        if (emoteBone.GetComponent<BoneRef>() is not null)
        {
            emoteBone.GetComponent<BoneRef>().target = originalBone;
        }
        else
        {
            emoteBone.gameObject.AddComponent<BoneRef>().target = originalBone;
        }
    }
    internal static EmoteConstraint AddConstraint(GameObject gameObject, BoneMapper mapper, Transform target, bool needToFix)
    {
        EmoteConstraint constraint = gameObject.AddComponent<EmoteConstraint>();
        constraint.AddSource(gameObject.transform, target);
        constraint.revertTransform = mapper.revertTransform;
        constraint.needToFix = needToFix;
        return constraint;
    }

    void Start()
    {
        StartCoroutine(FixConstraints());
    }
    IEnumerator FixConstraints()
    {
        yield return new WaitForEndOfFrame();
        if (needToFix)
        {
            ActivateConstraints();
            yield return new WaitForEndOfFrame();
            DeactivateConstraints();
        }
    }
}