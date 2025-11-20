using Input;
using Unity.Entities;
using Unity.Physics.GraphicsIntegration;
using UnityEngine;

namespace Player
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [SerializeField] public GameObject LookAt;
        
        public class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                // need to add this here because of ghosting!! it needs to be added at bake time
                AddComponent(entity, new PlayerNetworkId());
                AddComponent(entity, new PlayerInputComponent());
                AddComponent(
                    entity,
                    new PlayerLookAtRefComponent { LookAt = GetEntity(authoring.LookAt, TransformUsageFlags.Dynamic) }
                );
            }
        }
    }
}