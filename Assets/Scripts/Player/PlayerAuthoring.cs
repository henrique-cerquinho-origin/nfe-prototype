using Unity.Entities;
using UnityEngine;

namespace Input
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField] public GameObject ControllingCamera;
        
        public class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerInputComponent());
                AddComponent(
                    entity,
                    new PlayerCameraRefComponent
                    {
                        Camera = GetEntity(authoring.ControllingCamera, TransformUsageFlags.Dynamic)
                    }
                );
            }
        }
    }
}