using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EmotesAPI
{
    class WorldPropSpawnHandler : MonoBehaviour
    {
        public int propPos;
        void Start()
        {
            BoneMapper mapper = GetComponent<BoneMapper>();
            foreach (var item in BoneMapper.allWorldProps[propPos].joinSpots)
            {
                mapper.SpawnJoinSpot(item);
            }
        }
    }
}
