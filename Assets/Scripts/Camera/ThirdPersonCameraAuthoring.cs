using Unity.Entities;
using UnityEngine;

namespace Camera
{
    [DisallowMultipleComponent]
    public class ThirdPersonCameraAuthoring : MonoBehaviour
    {
        [Header("Rotation")]
        public float InitialPhi = 45f;
        public float InitialTheta = 45f;

        [Header("Distance")]
        public float StartDistance = 5f;

        public class Baker : Baker<ThirdPersonCameraAuthoring>
        {
            public override void Bake(ThirdPersonCameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

                AddComponent(
                    entity,
                    new ThirdPersonCameraComponent
                    {
                        CurrentPhi = authoring.InitialPhi,
                        CurrentTheta = authoring.InitialTheta,
                        CurrentDistance = authoring.StartDistance
                    }
                );
                // SetComponentEnabled<ThirdPersonCameraComponent>(entity, false);
                AddComponent(entity, new CameraControlComponent());
            }
        }
    }
}