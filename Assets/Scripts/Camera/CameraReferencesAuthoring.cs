using Camera;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

public class CameraReferencesAuthoring : MonoBehaviour
{
    [SerializeField] public CinemachineCamera PlayerCamera;
    [SerializeField] public CinemachineCamera CubeCamera;
    
    public class Baker : Baker<CameraReferencesAuthoring>
    {
        public override void Bake(CameraReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponentObject(
                entity,
                new CameraReferencesComponent
                {
                    PlayerCamera = authoring.PlayerCamera,
                    CubeCamera = authoring.CubeCamera
                }
            );
        }
    }
}