using Unity.Entities;
using Unity.Transforms;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CinemachineBridgeSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ActiveCameraTargetComponent>();
            RequireForUpdate<CinemachineBridgeComponent>();
        }

        protected override void OnUpdate()
        {
            var activeCameraTarget = SystemAPI.GetSingletonEntity<ActiveCameraTargetComponent>();
            var targetLTW = SystemAPI.GetComponent<LocalToWorld>(activeCameraTarget);
            foreach (var cameraLookAtBridge in SystemAPI.Query<CinemachineBridgeComponent>())
            {
                cameraLookAtBridge.LookAtTransform.position = targetLTW.Position;
                cameraLookAtBridge.LookAtTransform.rotation = targetLTW.Rotation;
            }
        }
    }
}