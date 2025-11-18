using Unity.Entities;
using Unity.Mathematics;

namespace Camera
{
    [UpdateBefore(typeof(CameraSystem))]
    public partial struct CameraControlSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<ThirdPersonCameraComponent, CameraControlComponent>()
                    .Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            var job = new Job();
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        private partial struct Job : IJobEntity
        {
            public void Execute(ref ThirdPersonCameraComponent camera, in CameraControlComponent cameraControl)
            {
                camera.CurrentTheta += cameraControl.LookDelta.x;
                if (camera.CurrentTheta >= 360) camera.CurrentTheta -= 360;
                if (camera.CurrentTheta < 0) camera.CurrentTheta += 360;
                
                camera.CurrentPhi += cameraControl.LookDelta.y;
                camera.CurrentPhi = math.clamp(
                    camera.CurrentPhi,
                    -89.999f,
                    89.999f
                );
            }
        }
    }
}