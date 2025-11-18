using Input;
using Player;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct CreateCameraClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ReferencesComponent>();
            state.RequireForUpdate<NetworkId>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<PlayerNetworkId>()
                    .WithNone<PlayerCameraRefComponent>()
                    .Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            var references = SystemAPI.GetSingleton<ReferencesComponent>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            int localNetworkId = SystemAPI.GetSingleton<NetworkId>().Value;

            foreach ((var playerNetworkId, var playerEntity) in SystemAPI.Query<RefRO<PlayerNetworkId>>()
                .WithNone<PlayerCameraRefComponent>()
                .WithEntityAccess())
            {
                if (playerNetworkId.ValueRO.NetworkId != localNetworkId) continue;
                
                Entity cameraEntity = ecb.Instantiate(references.CameraPrefab);
                ecb.AddComponent(playerEntity, new PlayerCameraRefComponent { Camera = cameraEntity });
                break;
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}
