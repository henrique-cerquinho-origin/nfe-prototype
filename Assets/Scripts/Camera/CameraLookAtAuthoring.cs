using Unity.Entities;
using UnityEngine;

namespace Camera
{
    [DisallowMultipleComponent]
    public class CameraLookAtAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CameraLookAtAuthoring>
        {
            public override void Bake(CameraLookAtAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ActiveCameraTargetComponent>(entity);
            }
        }
    }
}