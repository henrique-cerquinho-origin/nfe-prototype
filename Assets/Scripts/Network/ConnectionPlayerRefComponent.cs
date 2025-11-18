using Unity.Entities;

namespace Network
{
    public struct ConnectionPlayerRefComponent : IComponentData
    {
        public Entity Player;
    }
}