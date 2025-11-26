using Input;
using Unity.Entities;
using Unity.NetCode;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.LocalSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [UpdateAfter(typeof(PlayerInputSystem))]
    public partial struct SwitchCameraTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCameraTargetComponent>();
            state.RequireForUpdate<PlayerInputComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var cameraStateRef = SystemAPI.GetSingletonRW<CameraStateComponent>();

            foreach (var inputRef in SystemAPI.Query<RefRO<PlayerInputComponent>>()
                .WithAll<GhostOwnerIsLocal, CameraTargetComponent>())
            {
                if (inputRef.ValueRO.SwitchCamera.IsSet)
                {
                    cameraStateRef.ValueRW.Desired = cameraStateRef.ValueRO.Current == CameraType.Adventure
                        ? CameraType.Aiming
                        : CameraType.Adventure;
                }
                return;
            }
        }
    }
}