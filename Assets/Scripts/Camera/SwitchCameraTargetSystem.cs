using Input;
using Unity.Entities;
using Unity.NetCode;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.LocalSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct SwitchCameraTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCameraTargetComponent>();
            state.RequireForUpdate<PlayerInputComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var activeCameraTargetRef = SystemAPI.GetSingletonRW<ActiveCameraTargetComponent>();

            foreach (var (_, entity) in SystemAPI.Query<RefRO<PlayerInputComponent>>()
                .WithAll<GhostOwnerIsLocal, CameraTargetComponent>()
                .WithEntityAccess())
            {
                activeCameraTargetRef.ValueRW.Target = entity;
                break;
            }
        }
    }
}