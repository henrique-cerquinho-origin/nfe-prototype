using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Network
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInGameServerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder().WithAll<GoInGameRequestRPC>().Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach ((var receiveRpcCommandRequestRef, var entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
                .WithAll<GoInGameRequestRPC>()
                .WithEntityAccess())
            {
                ecb.AddComponent<NetworkStreamInGame>(receiveRpcCommandRequestRef.ValueRO.SourceConnection);
                ecb.DestroyEntity(entity);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}