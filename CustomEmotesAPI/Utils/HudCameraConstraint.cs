using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalEmotesAPI.Utils
{
    [DefaultExecutionOrder(-1)]

    public class HudCameraConstraint : MonoBehaviour
    {
        public Transform target;
        public bool constraintActive = true;
        void LateUpdate()
        {
            if (constraintActive)
            {
                //transform.position = new Vector3(target.position.x, target.position.y - 6f, target.position.z);
                //transform.position = new Vector3(target.position.x, target.position.y, target.position.z/*241.6527f*/);
                transform.position = new Vector3(target.position.x, 1076, target.position.z - 6);

                //transform.position = new Vector3(-822.5241f, target.position.y - 6f, 1075.908f);
            }
        }
    }
}
