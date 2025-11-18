using Unity.Entities;

namespace Network
{
    public struct ConnectionCameraRefComponent : IComponentData
    {
        public Entity Camera;
    }
}