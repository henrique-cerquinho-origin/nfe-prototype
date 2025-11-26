using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    //TODO should this be here?
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CameraBootstrapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate(
                SystemAPI.QueryBuilder().WithAll<CameraReferencesComponent>().WithNone<CamerasCreatedTag>().Build()
            );
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (references, entity) in SystemAPI.Query<CameraReferencesComponent>()
                .WithNone<CamerasCreatedTag>().WithEntityAccess())
            {
                var lookAt = new GameObject { name = "CameraLookAt" };
                CinemachineCamera playerCamera = Object.Instantiate(references.PlayerCamera);
                CinemachineCamera cubeCamera = Object.Instantiate(references.CubeCamera);
                playerCamera.Target.TrackingTarget = lookAt.transform;
                cubeCamera.Target.TrackingTarget = lookAt.transform;
                playerCamera.gameObject.SetActive(true);
                cubeCamera.gameObject.SetActive(false);
                
                Entity bridgeEntity = ecb.CreateEntity();
                ecb.SetName(bridgeEntity, "CameraBridge");
                ecb.AddComponent(bridgeEntity, new CinemachineBridgeComponent { LookAtTransform = lookAt.transform });
                
                ecb.AddComponent(entity, new CamerasCreatedTag());
            }
            
            ecb.Playback(EntityManager);
        }
    }
}