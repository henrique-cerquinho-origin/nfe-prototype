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
            RequireForUpdate<CameraRigRuntimeComponent>();
        }

        protected override void OnUpdate()
        {
            foreach (var cameraLookAtBridge in SystemAPI.Query<CameraRigRuntimeComponent>())
            {
                var activeCameraTarget = SystemAPI.GetSingletonEntity<ActiveCameraTargetComponent>();
                var targetLTW = SystemAPI.GetComponent<LocalToWorld>(activeCameraTarget);
                cameraLookAtBridge.LookAt.position = targetLTW.Position;
                cameraLookAtBridge.LookAt.rotation = targetLTW.Rotation;
                break;
            }
        }
    }
}