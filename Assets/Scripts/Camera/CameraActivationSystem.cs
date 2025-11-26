using Unity.Entities;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.LocalSimulation)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(CinemachineBridgeSystem))]
    public partial class CameraActivationSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<CameraRigRuntimeComponent>();
            RequireForUpdate<CameraStateComponent>();
        }

        protected override void OnUpdate()
        {
            var cameraStateRef = SystemAPI.GetSingletonRW<CameraStateComponent>();
            foreach (var cameraRig in SystemAPI.Query<CameraRigRuntimeComponent>())
            {
                if (cameraStateRef.ValueRO.Current == cameraStateRef.ValueRO.Desired)
                    return;

                cameraRig.Cameras[cameraStateRef.ValueRO.Current].Priority = 0;
                cameraRig.Cameras[cameraStateRef.ValueRO.Desired].Priority = 1;
                cameraStateRef.ValueRW.Current = cameraStateRef.ValueRO.Desired;
                return;
            }
        }
    }
}