using Unity.Cinemachine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Camera
{
    public class CinemachineBridge : MonoBehaviour
    {
        public Transform LookAtTransform;
        public CinemachineCamera CinemachineCamera; // this should be a reference on a camera manager, which has all virtual cams

        private EntityManager _em;
        private EntityQuery _activeCamQuery;

        private void Awake()
        {
            _em = ClientServerBootstrap.ClientWorld.EntityManager;
            if (CinemachineCamera != null && LookAtTransform != null)
            {
                CinemachineCamera.Follow = LookAtTransform;
            }
        }

        private void Update()
        {
            if (_em == default) return;

            Entity targetEntity = _em.CreateEntityQuery(ComponentType.ReadOnly<ActiveCameraTargetComponent>())
                .GetSingletonEntity();
            if (targetEntity == Entity.Null) return;
            if (!_em.Exists(targetEntity)) return;
            if (!_em.HasComponent<LocalToWorld>(targetEntity)) return;

            var targetLTW = _em.GetComponentData<LocalToWorld>(targetEntity);
            LookAtTransform.position = targetLTW.Position;
            LookAtTransform.rotation = targetLTW.Rotation;
        }
    }
}