using Unity.Entities;

namespace Camera
{
    public struct CameraTargetComponent : IComponentData
    {
        public Entity LookAtEntity;
    }
}