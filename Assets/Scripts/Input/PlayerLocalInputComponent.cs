using Unity.Entities;
using Unity.Mathematics;

namespace Input
{
    public struct PlayerLocalInputComponent : IComponentData
    {
        public float2 LookDelta;
    }
}