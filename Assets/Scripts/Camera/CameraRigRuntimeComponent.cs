using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Camera
{
    public class CameraRigRuntimeComponent : IComponentData
    {
        public Transform LookAt;
        public Dictionary<CameraType, CinemachineCamera> Cameras;
    }
}