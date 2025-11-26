using Unity.Entities;
using UnityEngine;

namespace Camera
{
    [DisallowMultipleComponent]
    public class CameraEntityAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CameraEntityAuthoring>
        {
            public override void Bake(CameraEntityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ActiveCameraTargetComponent>(entity);
            }
        }
    }
}