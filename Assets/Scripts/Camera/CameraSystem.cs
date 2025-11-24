using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateAfter(typeof(CameraControlSystem))]
    public partial struct CameraSystem : ISystem
    {
        private ComponentLookup<LocalToWorld> _localToWorldLookup;
        private ComponentLookup<LocalTransform> _localTransformLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainCameraEntityTag>();
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<CameraLookAtComponent>()
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
                MainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraEntityTag>(),
                DeltaTime = SystemAPI.Time.DeltaTime,
                LocalToWorldLookup = _localToWorldLookup,
                LocalTransformLookup = _localTransformLookup
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct Job : IJobEntity
        {
            public Entity MainCameraEntity;
            public float DeltaTime;

            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            // If you schedule parallel and you’re sure only one writer touches a given entity,
            // add: [NativeDisableParallelForRestriction] to avoid safety warnings.
            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> LocalTransformLookup;

            public void Execute(ref ThirdPersonCameraComponent camera, in CameraLookAtComponent lookAt)
            {
                if (!LocalToWorldLookup.HasComponent(lookAt.LookAt)) return;
                camera.CurrentLookAt = math.lerp(
                    camera.CurrentLookAt,
                    LocalToWorldLookup[lookAt.LookAt].Position,
                    DeltaTime * 10
                );

                quaternion sphereRot = quaternion.Euler(
                    math.radians(camera.CurrentPhi),
                    math.radians(camera.CurrentTheta),
                    0f
                );

                float3 camVec = math.mul(sphereRot, math.forward()) * camera.CurrentDistance;

                LocalTransform camLT = LocalTransformLookup[MainCameraEntity];
                camLT.Position = camera.CurrentLookAt + camVec;
                camLT.Rotation = quaternion.LookRotationSafe(-camVec, math.up());
                LocalTransformLookup[MainCameraEntity] = camLT;
            }
        }
    }
}