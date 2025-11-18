using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct CameraSystem : ISystem
    {
        private ComponentLookup<LocalToWorld> _localToWorldLookup;
        private ComponentLookup<LocalTransform> _localTransformLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainCameraEntityComponent>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithPresent<ThirdPersonCameraComponent>()
                    .Build()
            );
            _localToWorldLookup = state.GetComponentLookup<LocalToWorld>(true);
            _localTransformLookup = state.GetComponentLookup<LocalTransform>(false);
        }

        public void OnUpdate(ref SystemState state)
        {
            _localToWorldLookup.Update(ref state);
            _localTransformLookup.Update(ref state);
            var job = new Job
            {
                MainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraEntityComponent>(),
                LocalToWorldLookup = _localToWorldLookup,
                LocalTransformLookup = _localTransformLookup
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct Job : IJobEntity
        {
            public Entity MainCameraEntity;

            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            // If you schedule parallel and you’re sure only one writer touches a given entity,
            // add: [NativeDisableParallelForRestriction] to avoid safety warnings.
            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            public void Execute(in ThirdPersonCameraComponent camera)
            {
                if (!LocalToWorldLookup.HasComponent(camera.LookAt)) return;
                LocalToWorld targetLTW = LocalToWorldLookup[camera.LookAt];

                quaternion sphereRot = quaternion.Euler(
                    math.radians(camera.CurrentPhi),
                    math.radians(camera.CurrentTheta),
                    0f
                );

                float3 camVec = math.mul(sphereRot, math.forward()) * camera.CurrentDistance;

                LocalTransform camLT = LocalTransformLookup[MainCameraEntity];
                camLT.Position = targetLTW.Position + camVec;
                camLT.Rotation = quaternion.LookRotationSafe(-camVec, math.up());
                LocalTransformLookup[MainCameraEntity] = camLT;
            }
        }
    }
}