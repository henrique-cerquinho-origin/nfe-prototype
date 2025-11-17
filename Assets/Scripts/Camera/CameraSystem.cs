using Input;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Camera
{
    public partial struct CameraSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<PhysicsWorldSingleton>();
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
                var targetTransform = SystemAPI.GetComponent<LocalTransform>(thirdPersonCameraRef.ValueRO.LookAt);
                float3 cameraTargetUp = math.mul(targetTransform.Rotation, math.up());
                quaternion spherePos = quaternion.Euler(
                    thirdPersonCameraRef.ValueRO.CurrentPhi,
                    thirdPersonCameraRef.ValueRO.CurrentTheta,
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
                cameraTransformRef.ValueRW.Rotation = quaternion.LookRotationSafe(-cameraVector, cameraTargetUp);
            }
        }
    }
}