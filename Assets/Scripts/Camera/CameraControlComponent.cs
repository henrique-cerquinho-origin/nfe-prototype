using Unity.Entities;
using Unity.Mathematics;

namespace Input
{
    public struct CameraControlComponent : IComponentData
    {
        public float2 LookDelta;
    }
}