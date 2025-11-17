using Unity.Entities;

namespace Camera
{
    public struct ThirdPersonCameraComponent : IComponentData, IEnableableComponent
    {
        public float CurrentPhi;
        public float CurrentTheta;
        public float CurrentDistance;
        public Entity LookAt;
    }
}