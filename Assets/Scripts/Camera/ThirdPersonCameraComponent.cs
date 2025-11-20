using Unity.Entities;
using Unity.Mathematics;

namespace Camera
{
    public struct ThirdPersonCameraComponent : IComponentData, IEnableableComponent
    {
        public float CurrentPhi;
        public float CurrentTheta;
        public float CurrentDistance;
        public float3 CurrentLookAt;
    }
}