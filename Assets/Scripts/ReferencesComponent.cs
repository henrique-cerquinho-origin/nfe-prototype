using Unity.Entities;

public struct ReferencesComponent : IComponentData
{
    public Entity PlayerPrefab;
    public Entity CameraPrefab;
}