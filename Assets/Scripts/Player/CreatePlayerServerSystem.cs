using Network;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Player
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct CreatePlayerServerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ReferencesComponent>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder().WithAll<NetworkId>().WithNone<ConnectionPlayerRefComponent>().Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            var references = SystemAPI.GetSingleton<ReferencesComponent>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (networkIdRef, connectionEntity) in SystemAPI.Query<RefRO<NetworkId>>()
                .WithNone<ConnectionPlayerRefComponent>()
                .WithEntityAccess())
            {
                Entity playerEntity = ecb.Instantiate(references.PlayerPrefab);
                ecb.SetComponent(playerEntity, LocalTransform.FromPosition(new float3(0, 3, 0)));
                ecb.SetComponent(playerEntity, new PlayerNetworkId { NetworkId = (uint)networkIdRef.ValueRO.Value });
                ecb.AddComponent(playerEntity, new GhostOwner { NetworkId = networkIdRef.ValueRO.Value });
                
                ecb.AddComponent(connectionEntity, new ConnectionPlayerRefComponent { Player = playerEntity });
                ecb.AddComponent(connectionEntity, new CommandTarget { targetEntity = playerEntity });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}