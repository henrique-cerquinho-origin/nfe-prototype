using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Input
{
    public struct PlayerInputComponent : IComponentData
    {
        [GhostField] public float2 MoveDelta;
        [GhostField] public float2 LookDelta;
    }
}