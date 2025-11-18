using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Network
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInGameClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder().WithAll<NetworkId>().WithNone<NetworkStreamInGame>().Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach ((_, var entity) in SystemAPI.Query<RefRO<NetworkId>>()
                .WithNone<NetworkStreamInGame>()
                .WithEntityAccess())
            {
                ecb.AddComponent<NetworkStreamInGame>(entity);
                Entity rpcEntity = ecb.CreateEntity();
                ecb.AddComponent<GoInGameRequestRPC>(rpcEntity);
                ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}