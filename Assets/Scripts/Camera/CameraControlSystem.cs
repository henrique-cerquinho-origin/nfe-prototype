using Input;
using Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    // [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerInputSystem))]
    public partial struct CameraControlSystem : ISystem
    {
        private ComponentLookup<ThirdPersonCameraComponent> _cameraLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<PlayerInputComponent, PlayerCameraRefComponent>()
                    .Build()
            );
            _cameraLookup = state.GetComponentLookup<ThirdPersonCameraComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _cameraLookup.Update(ref state);
            var job = new Job { CameraLookup = _cameraLookup };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        private partial struct Job : IJobEntity
        {
            [NativeDisableParallelForRestriction]
            public ComponentLookup<ThirdPersonCameraComponent> CameraLookup;
            
            public void Execute(ref PlayerInputComponent playerInput, in PlayerCameraRefComponent playerCameraRef)
            {
                ThirdPersonCameraComponent camera = CameraLookup[playerCameraRef.Camera];
                camera.CurrentTheta += playerInput.LookDelta.x;
                if (camera.CurrentTheta >= 360) camera.CurrentTheta -= 360;
                if (camera.CurrentTheta < 0) camera.CurrentTheta += 360;
                
                camera.CurrentPhi += playerInput.LookDelta.y;
                camera.CurrentPhi = math.clamp(
                    camera.CurrentPhi,
                    -89.999f,
                    89.999f
                );
                CameraLookup[playerCameraRef.Camera] = camera;

                playerInput.CurrentCameraAngle = camera.CurrentTheta;
            }
        }
    }
}