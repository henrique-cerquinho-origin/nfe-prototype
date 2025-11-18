using Input;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Camera
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PlayerMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<PhysicsVelocity, PlayerInputComponent, PlayerCameraRefComponent, LocalTransform>()
                    .Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((var physicsVelocityRef, var inputRef, var playerCameraRef, var localTransformRef) in SystemAPI
                .Query<RefRW<PhysicsVelocity>, RefRO<PlayerInputComponent>, RefRO<PlayerCameraRefComponent>,
                    RefRW<LocalTransform>>())
            {
                var camera = SystemAPI.GetComponent<ThirdPersonCameraComponent>(playerCameraRef.ValueRO.Camera);
                physicsVelocityRef.ValueRW.Linear = math.mul(
                    quaternion.Euler(0, (camera.CurrentTheta + 180) * math.TORADIANS, 0),
                    new float3(
                        inputRef.ValueRO.MoveDelta.x * 10,
                        physicsVelocityRef.ValueRW.Linear.y,
                        inputRef.ValueRO.MoveDelta.y * 10
                    )
                );
                physicsVelocityRef.ValueRW.Angular = float3.zero;
                localTransformRef.ValueRW.Rotation = quaternion.identity;
            }
        }
    }
}