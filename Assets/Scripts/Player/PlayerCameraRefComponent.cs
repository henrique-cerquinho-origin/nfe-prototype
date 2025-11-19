using Unity.Entities;

namespace Player
{
    public struct PlayerCameraRefComponent : IComponentData
    {
        public Entity Camera;
    }
}