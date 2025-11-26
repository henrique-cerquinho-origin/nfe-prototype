using Unity.Entities;

namespace Camera
{
    public struct CameraStateComponent : IComponentData
    {
        public CameraType Current;
        public CameraType Desired;
        public CameraTargetComponent Target;
    }
}