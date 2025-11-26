using Input;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostSimulationSystemGroup))]
    [UpdateBefore(typeof(GhostInputSystemGroup))]
    public partial struct CameraLookAtPositioningSystem : ISystem
    {
        private ComponentLookup<CameraTargetComponent> _cameraLookup;
        private ComponentLookup<LocalToWorld> _localToWorldLookup;
        private ComponentLookup<LocalTransform> _localTransformLookup;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCameraTargetComponent>();
            _cameraLookup = state.GetComponentLookup<CameraTargetComponent>(true);
            _localTransformLookup = state.GetComponentLookup<LocalTransform>();
            _localToWorldLookup = state.GetComponentLookup<LocalToWorld>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _cameraLookup.Update(ref state);
            _localToWorldLookup.Update(ref state);
            _localTransformLookup.Update(ref state);
            var job = new Job
            {
                CameraTargetLookup = _cameraLookup,
                LocalTransformLookup = _localTransformLookup,
                LocalToWorldLookup = _localToWorldLookup,
                ActiveCameraTargetEntity = SystemAPI.GetSingleton<ActiveCameraTargetComponent>().Target,
                CameraLookAtBridgeEntity = SystemAPI.GetSingletonEntity<ActiveCameraTargetComponent>()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        private partial struct Job : IJobEntity
        {
            public Entity ActiveCameraTargetEntity;
            public Entity CameraLookAtBridgeEntity;
            [ReadOnly] public ComponentLookup<CameraTargetComponent> CameraTargetLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> LocalTransformLookup;

            public void Execute()
            {
                CameraTargetComponent activeCameraTarget = CameraTargetLookup[ActiveCameraTargetEntity];
                LocalTransform lookAtBridgeTransform = LocalTransformLookup[CameraLookAtBridgeEntity];
                lookAtBridgeTransform.Position = LocalToWorldLookup[activeCameraTarget.LookAtEntity].Position;
                LocalTransformLookup[CameraLookAtBridgeEntity] = lookAtBridgeTransform;
            }
        }
    }
}