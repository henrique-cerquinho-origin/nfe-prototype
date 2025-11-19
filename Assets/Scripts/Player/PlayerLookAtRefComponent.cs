using Unity.Entities;

namespace Player
{
    public struct PlayerLookAtRefComponent : IComponentData
    {
        public Entity LookAt;
    }
}