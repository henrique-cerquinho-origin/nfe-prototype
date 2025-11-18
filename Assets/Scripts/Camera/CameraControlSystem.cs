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
            foreach ((var thirdPersonCameraRef, var controlRef) in SystemAPI
                .Query<RefRW<ThirdPersonCameraComponent>, RefRO<CameraControlComponent>>())
            {
                thirdPersonCameraRef.ValueRW.CurrentTheta += controlRef.ValueRO.LookDelta.x;
                if (thirdPersonCameraRef.ValueRW.CurrentTheta >= 360) thirdPersonCameraRef.ValueRW.CurrentTheta -= 360;
                if (thirdPersonCameraRef.ValueRW.CurrentTheta < 0) thirdPersonCameraRef.ValueRW.CurrentTheta += 360;
                
                thirdPersonCameraRef.ValueRW.CurrentPhi += controlRef.ValueRO.LookDelta.y;
                thirdPersonCameraRef.ValueRW.CurrentPhi = math.clamp(
                    thirdPersonCameraRef.ValueRW.CurrentPhi,
                    -89.999f,
                    89.999f
                );
            }
        }
    }
}