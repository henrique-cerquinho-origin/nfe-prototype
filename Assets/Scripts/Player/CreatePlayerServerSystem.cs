using Input;
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
            foreach ((var networkIdRef, var entity) in SystemAPI.Query<RefRO<NetworkId>>()
                .WithNone<ConnectionPlayerRefComponent>()
                .WithEntityAccess())
            {
                Entity playerEntity = ecb.Instantiate(references.PlayerPrefab);
                ecb.SetComponent(playerEntity, new LocalTransform { Position = new float3(0, 3, 0) });
                ecb.AddComponent(playerEntity, new PlayerNetworkId { NetworkId = networkIdRef.ValueRO.Value });
                ecb.AddComponent(entity, new ConnectionPlayerRefComponent { Player = playerEntity });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}