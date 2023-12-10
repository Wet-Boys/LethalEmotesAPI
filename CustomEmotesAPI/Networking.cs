using EmotesAPI;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

//Figured out/copied basic networking calls from x753's Mimic code, thanks!
//Double thanks to Evaisa's network patch which actually lets us use unity's networking
public class EmoteNetworker : NetworkBehaviour
{
    public static EmoteNetworker instance;
    private void Awake()
    {
        name = "Bigma Lalls";
        instance = this;
    }
    public void SyncEmote(ulong playerId, string animation, int pos)
    {
        if (IsOwner && IsServer)
        {
            SyncEmoteToClients(playerId, animation, pos);
        }
        else
        {
            SyncEmoteToServerRpc(playerId, animation, pos);
        }
    }
    public void SyncEmoteToClients(ulong playerId, string animation, int pos)
    {
        GameObject bodyObject = GetNetworkObject(playerId).gameObject;
        if (!bodyObject)
        {
            DebugClass.Log($"Body is null!!!");
        }

        //DebugClass.Log($"Recieved message to play {animation} on client. Playing on {bodyObject}");

        int eventNum = -1;
        CustomAnimationClip clip = BoneMapper.animClips[animation];
        try
        {
            clip.clip[0].ToString();
            eventNum = UnityEngine.Random.Range(0, BoneMapper.primaryAudioClips[clip.syncPos].Length);
        }
        catch (Exception)
        {
        }
        SyncEmoteToClientRpc(playerId, animation, pos, eventNum);
    }
    [ClientRpc]
    public void SyncEmoteToClientRpc(ulong playerId, string animation, int pos, int eventNum)
    {
        GameObject bodyObject = GetNetworkObject(playerId).gameObject;
        if (!bodyObject)
        {
            DebugClass.Log($"Body is null!!!");
        }

        BoneMapper map = bodyObject.GetComponentInChildren<BoneMapper>();
        DebugClass.Log($"Recieved message to play {animation} on client. Playing on {bodyObject}");

        map.PlayAnim(animation, pos, eventNum);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncEmoteToServerRpc(ulong playerId, string animation, int pos)
    {
        SyncEmoteToClients(playerId, animation, pos);
    }










    public void SyncJoinSpot(ulong playerId, ulong joinSpotId, bool worldProp, int posInArray)
    {
        if (IsOwner && IsServer)
        {
            SyncJoinSpotToClientRpc(playerId, joinSpotId, worldProp, posInArray);
        }
        else
        {
            SyncJoinSpotToServerRpc(playerId, joinSpotId, worldProp, posInArray);
        }
    }
    [ClientRpc]
    public void SyncJoinSpotToClientRpc(ulong playerId, ulong joinSpotId, bool worldProp, int posInArray)
    {
        DebugClass.Log($"joining spot               1");

        GameObject bodyObject = GetNetworkObject(playerId).gameObject;
        GameObject spotObject = GetNetworkObject(joinSpotId).gameObject;
        if (!bodyObject)
        {
            DebugClass.Log($"Body is null!!!");
        }
        if (!spotObject)
        {
            DebugClass.Log($"spotObject is null!!!");
        }
        DebugClass.Log($"joining spot               2");

        BoneMapper joinerMapper = bodyObject.GetComponentInChildren<BoneMapper>();
        DebugClass.Log($"joining spot               3");

        joinerMapper.PlayAnim("none", 0);
        DebugClass.Log($"joining spot               4");

        joinerMapper.currentEmoteSpot = spotObject.GetComponentsInChildren<EmoteLocation>()[posInArray].gameObject;
        DebugClass.Log($"joining spot               5");

        if (worldProp)
        {
            CustomEmotesAPI.JoinedProp(joinerMapper.currentEmoteSpot, joinerMapper, joinerMapper.currentEmoteSpot.GetComponent<EmoteLocation>().owner);
        }
        else
        {
            CustomEmotesAPI.JoinedBody(joinerMapper.currentEmoteSpot, joinerMapper, joinerMapper.currentEmoteSpot.GetComponent<EmoteLocation>().owner);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncJoinSpotToServerRpc(ulong playerId, ulong joinSpotId, bool worldProp, int posInArray)
    {
        SyncJoinSpotToClientRpc(playerId, joinSpotId, worldProp, posInArray);
    }

}
