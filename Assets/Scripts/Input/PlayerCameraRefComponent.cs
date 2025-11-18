using Unity.Entities;

namespace Input
{
    public struct PlayerCameraRefComponent : IComponentData
    {
        public Entity Camera;
    }
}