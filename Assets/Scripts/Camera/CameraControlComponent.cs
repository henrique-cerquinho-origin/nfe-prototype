using Unity.Entities;
using Unity.Mathematics;

namespace Camera
{
    public struct CameraControlComponent : IComponentData
    {
        public float2 LookDelta;
    }
}