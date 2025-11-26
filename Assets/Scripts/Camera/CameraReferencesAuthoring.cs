using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Camera
{
    public class CameraReferencesAuthoring : MonoBehaviour
    {
        [SerializeField] public List<CameraConfig> Cameras;
    
        public class Baker : Baker<CameraReferencesAuthoring>
        {
            public override void Bake(CameraReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                var cameras = new Dictionary<CameraType, CinemachineCamera>();
                foreach (CameraConfig configs in authoring.Cameras)
                {
                    cameras.Add(configs.Type, configs.Camera);
                }
                AddComponentObject(entity, new CameraReferencesComponent { Cameras = cameras });
            }
        }

        [Serializable]
        public struct CameraConfig
        {
            public CameraType Type;
            public CinemachineCamera Camera;
        }
    }
}