using Unity.Entities;

namespace Player
{
    public struct PlayerNetworkId : IComponentData
    {
        public int NetworkId;
    }
}