using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

namespace Camera
{
    public class CameraReferencesAuthoring : MonoBehaviour
    {
        [SerializeField] public CinemachineCamera AdventureCamera;
        [SerializeField] public CinemachineCamera AimingCamera;
    
        public class Baker : Baker<CameraReferencesAuthoring>
        {
            public override void Bake(CameraReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                var cameras = new Dictionary<CameraType, CinemachineCamera>
                {
                    { CameraType.Adventure, authoring.AdventureCamera },
                    { CameraType.Aiming, authoring.AimingCamera }
                };
                AddComponentObject(entity, new CameraReferencesComponent { Cameras = cameras });
            }
        }
    }
}