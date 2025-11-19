using Unity.Entities;

namespace Camera
{
    public struct CameraLookAtComponent : IComponentData
    {
        public Entity LookAt;
    }
}