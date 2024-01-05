using Unity.Netcode;
using UnityEngine;
using Cinemachine;

namespace Utils
{
    //Change priority of Owner camera to 15
    //TODO: make camera priority static variable
    public class NetworkCamera : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase virtualCamera;

        private int _camPriority = 15;
    
        public override void OnNetworkSpawn()
        {
            if(!IsOwner) return;
            virtualCamera.Priority = _camPriority;
        }
    }
}
