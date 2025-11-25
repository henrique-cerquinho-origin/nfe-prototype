using Unity.Entities;

namespace Camera
{
    public struct ActiveCameraTargetComponent : IComponentData
    {
        public Entity Target;
    }
}