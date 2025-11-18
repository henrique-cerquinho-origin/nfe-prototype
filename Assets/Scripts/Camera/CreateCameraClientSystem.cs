using Input;
using Network;
using Player;
using Unity.Collections;
using Unity.Entities;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateAfter(typeof(CreatePlayerServerSystem))]
    public partial struct CreateCameraClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ReferencesComponent>();
            // state.RequireForUpdate(
            //     SystemAPI.QueryBuilder()
            //         .WithAll<ConnectionPlayerRefComponent>()
            //         .WithNone<ConnectionCameraRefComponent>()
            //         .Build()
            // );
        }

        public void OnUpdate(ref SystemState state)
        {
            var references = SystemAPI.GetSingleton<ReferencesComponent>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach ((var connectionPlayerRef, var entity) in SystemAPI.Query<RefRO<ConnectionPlayerRefComponent>>()
                .WithNone<ConnectionCameraRefComponent>()
                .WithEntityAccess())
            {
                Entity cameraEntity = ecb.Instantiate(references.CameraPrefab);
                ecb.AddComponent(
                    connectionPlayerRef.ValueRO.Player,
                    new PlayerCameraRefComponent { Camera = cameraEntity }
                );
                ecb.AddComponent(entity, new ConnectionCameraRefComponent { Camera = cameraEntity });
                ecb.AddComponent(
                    cameraEntity,
                    new ThirdPersonCameraComponent
                    {
                        CurrentPhi = -22,
                        CurrentTheta = 45,
                        CurrentDistance = 5
                    }
                );
                ecb.AddComponent(
                    connectionPlayerRef.ValueRO.Player,
                    new PlayerCameraRefComponent { Camera = cameraEntity }
                );
                ecb.AddComponent(cameraEntity, new CameraControlComponent());
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}