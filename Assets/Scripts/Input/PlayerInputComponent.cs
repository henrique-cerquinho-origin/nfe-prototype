using Unity.Entities;
using Unity.Mathematics;

namespace Input
{
    public struct PlayerInputComponent : IComponentData
    {
        public float2 LookDelta;

        public Entity ControllingCamera;
    }
}