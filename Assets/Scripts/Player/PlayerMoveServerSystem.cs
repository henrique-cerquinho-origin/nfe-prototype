using Input;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Player
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PlayerMoveServerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<PhysicsVelocity, PlayerInputComponent, LocalTransform>()
                    .Build()
            );
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((var physicsVelocityRef, var inputRef, var localTransformRef) in SystemAPI
                .Query<RefRW<PhysicsVelocity>, RefRO<PlayerInputComponent>, RefRW<LocalTransform>>())
            {
                physicsVelocityRef.ValueRW.Linear = math.mul(
                    quaternion.Euler(0, math.radians(inputRef.ValueRO.CurrentCameraAngle + 180), 0),
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