using Unity.Entities;
using UnityEngine;

public class ReferencesAuthoring : MonoBehaviour
{
    [SerializeField] public GameObject PlayerPrefab;
    [SerializeField] public GameObject CameraPrefab;
    
    public class Baker : Baker<ReferencesAuthoring>
    {
        public override void Bake(ReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(
                entity,
                new ReferencesComponent
                {
                    PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic),
                    CameraPrefab = GetEntity(authoring.CameraPrefab, TransformUsageFlags.Dynamic)
                }
            );
        }
    }
}