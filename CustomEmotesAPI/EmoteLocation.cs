using EmotesAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class EmoteLocation : MonoBehaviour
{
    public static List<EmoteLocation> emoteLocations = new List<EmoteLocation>();
    public static bool visibile = true;
    public int spot;
    public int validPlayers = 0;
    public BoneMapper owner;
    public BoneMapper emoter;
    public JoinSpot joinSpot;

    public static void HideAllSpots()
    {
        visibile = false;
        foreach (var item in emoteLocations)
        {
            try
            {
                foreach (var item2 in item.GetComponentsInChildren<Renderer>())
                {
                    item2.enabled = false;
                }
                foreach (var item2 in item.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    item2.enabled = false;
                }
            }
            catch (Exception)
            {
            }
        }
    }
    public static void ShowAllSpots()
    {
        visibile = true;
        foreach (var item in emoteLocations)
        {
            try
            {
                foreach (var item2 in item.GetComponentsInChildren<Renderer>())
                {
                    item2.enabled = true;
                }
                foreach (var item2 in item.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    item2.enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }
    }
    void Start()
    {
        SetColor();
        //spot = emoteLocations.Count;
        emoteLocations.Add(this);
        StartCoroutine(setScale());
        if (!visibile)
        {
            foreach (var item2 in GetComponentsInChildren<Renderer>())
            {
                item2.enabled = false;
            }
            foreach (var item2 in GetComponentsInChildren<ParticleSystemRenderer>())
            {
                item2.enabled = false;
            }
        }
    }
    void OnDestroy()
    {
        emoteLocations.Remove(this);
    }
    public void SetEmoterAndHideLocation(BoneMapper boneMapper)
    {
        if (!emoter)
        {
            emoter = boneMapper;
            SetVisible(false);
        }
    }
    public IEnumerator setScale()
    {
        yield return new WaitForSeconds(.1f);
        Vector3 scal = Vector3.one;
        if (owner.smr1)
        {
            scal = owner.transform.parent.lossyScale;
        }
        else
        {
            scal = owner.transform.lossyScale;
        }
        transform.localPosition = new Vector3(joinSpot.position.x / scal.x, joinSpot.position.y / scal.y, joinSpot.position.z / scal.z);
        transform.localEulerAngles = joinSpot.rotation;
        transform.localScale = new Vector3(joinSpot.scale.x / scal.x, joinSpot.scale.y / scal.y, joinSpot.scale.z / scal.z);
    }
    internal void SetVisible(bool visibility)
    {
        if (visibility)
            gameObject.transform.localPosition += new Vector3(5000, 5000, 5000);
        else
            gameObject.transform.localPosition -= new Vector3(5000, 5000, 5000);
    }

    void OnTriggerEnter(Collider other)
    {
        //TODO on trigger enter joinSpot
        //if (other.GetComponent<ModelLocator>() && other.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>() && other.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>() != owner)
        //{
        //    BoneMapper mapper = other.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();
        //    if (mapper)
        //    {
        //        validPlayers++;
        //        SetColor();
        //        //new SyncCurrentEmoteSpot(other.GetComponent<NetworkIdentity>().netId, gameObject.GetComponent<NetworkIdentity>().netId).Send(R2API.Networking.NetworkDestination.Clients);
        //        mapper.currentEmoteSpot = this.gameObject;
        //        CustomEmotesAPI.JoinSpotEntered(mapper, owner);
        //    }
        //}
    }
    void OnTriggerExit(Collider other)
    {
        //TODO on trigger exit joinSpot
        //if (other.GetComponent<ModelLocator>() && other.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>() && other.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>() != owner)
        //{
        //    BoneMapper mapper = other.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();
        //    if (mapper)
        //    {
        //        if (validPlayers != 0)
        //        {
        //            validPlayers--;
        //        }
        //        SetColor();
        //        if (mapper.currentEmoteSpot == this.gameObject)
        //        {
        //            mapper.currentEmoteSpot = null;
        //        }
        //    }
        //}
    }
    internal void SetColor()
    {
        if (validPlayers > 0)
        {
            GetComponentsInChildren<Renderer>()[GetComponentsInChildren<Renderer>().Length - 1].material.color = Color.green;
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.material.SetColor("_EmissionColor", Color.green);
            }
            foreach (var item in GetComponentsInChildren<ParticleSystemRenderer>())
            {
                item.material.SetColor("_EmissionColor", Color.green);
            }
            foreach (var item in GetComponentsInChildren<ParticleSystem>())
            {
                var trails = item.trails;
                trails.colorOverTrail = Color.green;
            }
        }
        else
        {
            GetComponentsInChildren<Renderer>()[GetComponentsInChildren<Renderer>().Length - 1].material.color = new Color(1f / 255f, 156f / 255f, 190f / 255f);
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.material.SetColor("_EmissionColor", new Color(1f / 255f, 156f / 255f, 190f / 255f));
            }
            foreach (var item in GetComponentsInChildren<ParticleSystemRenderer>())
            {
                item.material.SetColor("_EmissionColor", new Color(1f / 255f, 156f / 255f, 190f / 255f));
            }
            foreach (var item in GetComponentsInChildren<ParticleSystem>())
            {
                var trails = item.trails;
                trails.colorOverTrail = new Color(1f / 255f, 156f / 255f, 190f / 255f);
            }
        }
    }
}
