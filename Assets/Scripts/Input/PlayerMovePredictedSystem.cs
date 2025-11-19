using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Input
{
    [WorldSystemFilter(
        WorldSystemFilterFlags.ClientSimulation |
        WorldSystemFilterFlags.ThinClientSimulation |
        WorldSystemFilterFlags.ServerSimulation
    )]
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial struct PlayerMovePredictedSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    // `Simulate` is enough; keep `PredictedGhost` if you want to exclude non-predicted ghosts.
                    .WithAll<Simulate, PhysicsVelocity, PlayerInputComponent, LocalTransform>()
                    .Build());
        }

        public void OnUpdate(ref SystemState state)
        {
            // Safe to read on both; on server IsFinalPredictionTick is typically true.
            var netTime = SystemAPI.GetSingleton<NetworkTime>();
            bool isFinal = netTime.IsFinalPredictionTick;

            foreach (var (velocityRef, inputRef, ltRef) in SystemAPI
                .Query<RefRW<PhysicsVelocity>, RefRO<PlayerInputComponent>, RefRW<LocalTransform>>()
                .WithAll<Simulate>())
            {
                float yaw = math.radians(inputRef.ValueRO.CurrentCameraAngle + 180f);
                var desiredVelocity = new float3(
                    inputRef.ValueRO.MoveDelta.x * 10f,
                    velocityRef.ValueRW.Linear.y,
                    inputRef.ValueRO.MoveDelta.y * 10f
                );

                velocityRef.ValueRW.Linear  = math.mul(quaternion.Euler(0, yaw, 0), desiredVelocity);
                velocityRef.ValueRW.Angular = float3.zero;
                ltRef.ValueRW.Rotation = quaternion.identity;

                if (isFinal)
                {
                    // One-shot side effects (VFX/sounds). Avoid anything that would be replayed on resim.
                }
            }
        }
    }
}