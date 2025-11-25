using Unity.Entities;
using UnityEngine;

namespace Camera
{
    [DisallowMultipleComponent]
    public class CameraTargetAuthoring : MonoBehaviour
    {
        public GameObject LookAt;
        
        public class Baker : Baker<CameraTargetAuthoring>
        {
            public override void Bake(CameraTargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(
                    entity,
                    new CameraTargetComponent { LookAtEntity = GetEntity(authoring.LookAt, TransformUsageFlags.Dynamic) }
                );
            }
        }
    }
}