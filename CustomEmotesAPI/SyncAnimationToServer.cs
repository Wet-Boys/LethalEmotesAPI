
//TODO
//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;

//public class SyncAnimationToServer : INetMessage
//{
//    NetworkInstanceId netId;
//    string animation;
//    int position;

//    public SyncAnimationToServer()
//    {

//    }

//    public SyncAnimationToServer(NetworkInstanceId netId, string animation, int pos)
//    {
//        this.netId = netId;
//        this.animation = animation;
//        this.position = pos;
//    }

//    public void Deserialize(NetworkReader reader)
//    {
//        //DebugClass.Log($"POSITION: {reader.Position}, SIZE: {reader.Length}");

//        netId = reader.ReadNetworkId();
//        animation = reader.ReadString();
//        position = reader.ReadInt32();
//    }

//    public void OnReceived()
//    {
//        if (!NetworkServer.active)
//            return;

//        GameObject bodyObject = Util.FindNetworkObject(netId);
//        if (!bodyObject)
//        {
//            DebugClass.Log($"Body is null!!!");
//        }

//        //DebugClass.Log($"Recieved message to play {animation} on client. Playing on {bodyObject.GetComponent<ModelLocator>().modelTransform}");

//        //bodyObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>().PlayAnim(animation, position);
//        BoneMapper map = Util.FindNetworkObject(netId).GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();
//        int eventNum = -1;
//        CustomAnimationClip clip = BoneMapper.animClips[animation];
//        try
//        {
//            clip.clip[0].ToString();
//            eventNum = UnityEngine.Random.Range(0, BoneMapper.startEvents[clip.syncPos].Length);
//        }
//        catch (Exception)
//        {
//        }
//        new SyncAnimationToClients(netId, animation, position, eventNum).Send(R2API.Networking.NetworkDestination.Clients);
//    }

//    public void Serialize(NetworkWriter writer)
//    {
//        writer.Write(netId);
//        writer.Write(animation);
//        writer.Write(position);
//    }
//}
