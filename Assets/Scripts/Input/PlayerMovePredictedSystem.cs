using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    // `Simulate` is enough; keep `PredictedGhost` if you want to exclude non-predicted ghosts.
                    .WithAll<Simulate, PhysicsMass, PhysicsVelocity, PlayerInputComponent>()
                    .Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Safe to read on both; on server IsFinalPredictionTick is typically true.
            // var netTime = SystemAPI.GetSingleton<NetworkTime>();
            // bool isFinal = netTime.IsFinalPredictionTick;

            var job = new Job();
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            [BurstCompile]
            public void Execute(
                ref PhysicsMass mass,
                ref PhysicsVelocity velocity,
                in PlayerInputComponent input,
                EnabledRefRO<Simulate> simulate
            )
            {
                if (!simulate.ValueRO) return;
                
                float yaw = math.radians(input.CurrentCameraAngle + 180f);
                var desiredVelocity = new float3(
                    input.MoveDelta.x * 10f,
                    velocity.Linear.y,
                    input.MoveDelta.y * 10f
                );

                velocity.Linear  = math.mul(quaternion.Euler(0, yaw, 0), desiredVelocity);
                velocity.Angular = float3.zero;
                mass.InverseInertia.xz = 0;

                // if (isFinal)
                // {
                //     // One-shot side effects (VFX/sounds). Avoid anything that would be replayed on resim.
                // }
            }
        }
    }
}