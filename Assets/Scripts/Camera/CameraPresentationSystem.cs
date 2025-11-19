using Unity.Entities;
using Unity.Transforms;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraPresentationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (MainCameraGameObject.Instance == null || !SystemAPI.HasSingleton<MainCameraEntityTag>()) return;
            
            Entity mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraEntityTag>();
            var targetLocalToWorld = SystemAPI.GetComponent<LocalToWorld>(mainCameraEntity);
            MainCameraGameObject.Instance.transform.SetPositionAndRotation(
                targetLocalToWorld.Position,
                targetLocalToWorld.Rotation
            );
        }
    }
}