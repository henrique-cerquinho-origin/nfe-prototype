using Input;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [UpdateAfter(typeof(CameraLookAtPositioningSystem))]
    public partial struct CameraRotationControlSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerInputComponent>().Build());
            state.RequireForUpdate<ActiveCameraTargetComponent>();
            _localTransformLookup = state.GetComponentLookup<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _localTransformLookup.Update(ref state);
            var job = new Job
            {
                LocalTransformLookup = _localTransformLookup,
                CameraLookAtBridgeEntity = SystemAPI.GetSingletonEntity<ActiveCameraTargetComponent>()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        private partial struct Job : IJobEntity
        {
            public Entity CameraLookAtBridgeEntity;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> LocalTransformLookup;

            public void Execute(ref PlayerInputComponent input, in PlayerLocalInputComponent localInput)
            {
                LocalTransform lookAtBridgeTransform = LocalTransformLookup[CameraLookAtBridgeEntity];

                float3 forward = math.mul(lookAtBridgeTransform.Rotation, math.forward());

                float theta = math.atan2(forward.x, forward.z);
                float phi = math.asin(math.clamp(forward.y, -1f, 1f));

                phi += math.radians(localInput.LookDelta.y);
                theta += math.radians(localInput.LookDelta.x);

                float phiLimit = math.radians(89.999f);
                phi = math.clamp(phi, -phiLimit, phiLimit);

                float cp = math.cos(phi);
                var dir = new float3(math.sin(theta) * cp, math.sin(phi), math.cos(theta) * cp);

                lookAtBridgeTransform.Rotation = quaternion.LookRotationSafe(dir, math.up());

                LocalTransformLookup[CameraLookAtBridgeEntity] = lookAtBridgeTransform;
                input.CurrentCameraTheta = theta;
            }
        }
    }
}