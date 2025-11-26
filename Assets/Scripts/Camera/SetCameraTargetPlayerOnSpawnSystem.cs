using Player;
using Unity.Entities;
using Unity.NetCode;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.LocalSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct SetCameraTargetPlayerOnSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCameraTargetComponent>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder().WithAll<GhostOwnerIsLocal, CameraTargetComponent>().Build()
            );
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var activeCameraTargetRef = SystemAPI.GetSingletonRW<ActiveCameraTargetComponent>();
            if (activeCameraTargetRef.ValueRO.Target != Entity.Null) return;

            foreach (var (_, entity) in SystemAPI.Query<RefRO<PlayerNetworkId>>()
                .WithAll<GhostOwnerIsLocal, CameraTargetComponent>()
                .WithEntityAccess())
            {
                activeCameraTargetRef.ValueRW.Target = entity;
                break;
            }
        }
    }
}