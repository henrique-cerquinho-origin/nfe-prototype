using Unity.Mathematics;
using Unity.NetCode;

namespace Input
{
    public struct PlayerInputComponent : IInputComponentData
    {
        public float2 MoveDelta;
        public float CurrentCameraAngle;
    }
}