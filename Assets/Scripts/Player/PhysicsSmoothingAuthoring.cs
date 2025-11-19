using Unity.Entities;
using Unity.Physics.GraphicsIntegration;
using UnityEngine;

namespace Player
{
    public class PhysicsSmoothingAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PhysicsSmoothingAuthoring>
        {
            public override void Bake(PhysicsSmoothingAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new PhysicsGraphicalSmoothing());
            }
        }
    }
}