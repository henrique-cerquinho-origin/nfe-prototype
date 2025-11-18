using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Camera
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct CameraSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainCameraEntityComponent>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<ThirdPersonCameraComponent>()
                    .Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            Entity mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraEntityComponent>();
            foreach (var thirdPersonCameraRef in SystemAPI.Query<RefRW<ThirdPersonCameraComponent>>())
            {
                var targetTransform = SystemAPI.GetComponent<LocalToWorld>(thirdPersonCameraRef.ValueRO.LookAt);
                quaternion spherePos = quaternion.Euler(
                    thirdPersonCameraRef.ValueRO.CurrentPhi * math.TORADIANS,
                    thirdPersonCameraRef.ValueRO.CurrentTheta * math.TORADIANS,
                    0
                );
                float3 cameraVector = math.mul(
                    spherePos,
                    math.forward() * thirdPersonCameraRef.ValueRO.CurrentDistance
                );
                
                RefRW<LocalTransform> cameraTransformRef = SystemAPI.GetComponentRW<LocalTransform>(mainCameraEntity);
                cameraTransformRef.ValueRW.Position = targetTransform.Position + math.mul(
                    spherePos,
                    math.forward() * thirdPersonCameraRef.ValueRO.CurrentDistance
                );
                cameraTransformRef.ValueRW.Rotation = quaternion.LookRotationSafe(-cameraVector, math.up());
            }
        }
    }
}