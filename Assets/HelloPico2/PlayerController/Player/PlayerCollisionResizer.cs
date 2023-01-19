using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.PlayerController.Player
{
    public class PlayerCollisionResizer : MonoBehaviour
    {
        public CapsuleCollider _CapsuleCollider;
        
        private void Update()
        {
            ResizeCapsule();
        }
        private void ResizeCapsule() { 
            var camPos = HelloPico2.Singleton.GameManagerHelloPico.Instance._MainCamera.transform.position;
            var floorDist = camPos.y;
            _CapsuleCollider.transform.position = new Vector3(
                camPos.x,
                floorDist/2,
                camPos.z
                );
            _CapsuleCollider.height = floorDist;
        }
    }
}
