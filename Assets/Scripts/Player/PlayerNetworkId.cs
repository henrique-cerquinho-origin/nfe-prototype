using Unity.Entities;
using Unity.NetCode;

namespace Player
{
    public struct PlayerNetworkId : IComponentData
    {
        [GhostField] public uint NetworkId;
    }
}