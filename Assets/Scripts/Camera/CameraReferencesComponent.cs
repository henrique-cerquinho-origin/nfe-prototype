using Unity.Cinemachine;
using Unity.Entities;

namespace Camera
{
    public class CameraReferencesComponent : IComponentData
    {
        public CinemachineCamera PlayerCamera;
        public CinemachineCamera CubeCamera;
    }
}