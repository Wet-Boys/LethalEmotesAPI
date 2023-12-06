//TODO networking
//using EmotesAPI;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;


//class SyncSpotJoinedToHost : INetMessage
//{
//    NetworkInstanceId netId;
//    NetworkInstanceId spot;
//    bool worldProp;
//    int posInArray;
//    public SyncSpotJoinedToHost()
//    {

//    }

//    public SyncSpotJoinedToHost(NetworkInstanceId netId, NetworkInstanceId spot, bool worldProp, int posInArray)
//    {
//        this.netId = netId;
//        this.spot = spot;
//        this.worldProp = worldProp;
//        this.posInArray = posInArray;
//    }

//    public void Deserialize(NetworkReader reader)
//    {
//        netId = reader.ReadNetworkId();
//        spot = reader.ReadNetworkId();
//        worldProp = reader.ReadBoolean();
//        posInArray = reader.ReadInt32();
//    }

//    public void OnReceived()
//    {
//        new SyncSpotJoinedToClient(netId, spot, worldProp, posInArray).Send(R2API.Networking.NetworkDestination.Clients);
//        //DebugClass.Log($"received: {netId} | {spot} | {worldProp} | {posInArray}");
//        //GameObject bodyObject = Util.FindNetworkObject(netId);
//        //GameObject spotObject = Util.FindNetworkObject(spot);
//        //if (!bodyObject)
//        //{
//        //    DebugClass.Log($"Body is null!!!");
//        //}
//        //if (!spotObject)
//        //{
//        //    DebugClass.Log($"spotObject is null!!!");
//        //}
//        //BoneMapper joinerMapper = bodyObject.GetComponent<ModelLocator>().modelTransform.GetComponentInChildren<BoneMapper>();
//        //joinerMapper.currentEmoteSpot = spotObject.GetComponentsInChildren<EmoteLocation>()[posInArray].gameObject;
//        //if (worldProp)
//        //{
//        //    CustomEmotesAPI.JoinedProp(joinerMapper.currentEmoteSpot, joinerMapper, joinerMapper.currentEmoteSpot.GetComponent<EmoteLocation>().owner);
//        //}
//        //else
//        //{
//        //    CustomEmotesAPI.JoinedBody(joinerMapper.currentEmoteSpot, joinerMapper, joinerMapper.currentEmoteSpot.GetComponent<EmoteLocation>().owner);
//        //}
//    }

//    public void Serialize(NetworkWriter writer)
//    {
//        writer.Write(netId);
//        writer.Write(spot);
//        writer.Write(worldProp);
//        writer.Write(posInArray);
//    }
//}
