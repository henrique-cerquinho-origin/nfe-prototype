using Unity.Entities;
using Unity.Transforms;

namespace Camera
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraPresentationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (MainCameraGameObject.Instance == null || !SystemAPI.HasSingleton<MainCameraEntityComponent>())
            {
                return;
            }
            
            Entity mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraEntityComponent>();
            var targetLocalToWorld = SystemAPI.GetComponent<LocalToWorld>(mainCameraEntity);
            MainCameraGameObject.Instance.transform.SetPositionAndRotation(
                targetLocalToWorld.Position,
                targetLocalToWorld.Rotation
            );
        }
    }
}