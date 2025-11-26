using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Camera
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
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

            foreach (var (references, managerEntity) in SystemAPI.Query<CameraReferencesComponent>()
                .WithNone<CamerasCreatedTag>()
                .WithEntityAccess())
            {
                var root = new GameObject("CameraRig");

                Transform lookAt = new GameObject("CameraLookAt").transform;
                lookAt.SetParent(root.transform, false);

                var cameraInstances = new Dictionary<CameraType, CinemachineCamera>();
                foreach ((CameraType type, CinemachineCamera camera) in references.Cameras)
                {
                    CinemachineCamera instance = Object.Instantiate(camera, root.transform);
                    instance.Target.TrackingTarget = lookAt;
                    cameraInstances.Add(type, instance);
                }

                cameraInstances[CameraType.Adventure].Priority = 1;

                ecb.AddComponent(managerEntity, new CamerasCreatedTag());
                ecb.AddComponent(
                    managerEntity,
                    new CameraStateComponent { Current = CameraType.Adventure, Desired = CameraType.Adventure }
                );
                ecb.AddComponent(
                    managerEntity,
                    new CameraRigRuntimeComponent { LookAt = lookAt, Cameras = cameraInstances }
                );
                
                Entity targetEntity = ecb.CreateEntity();
                ecb.SetName(targetEntity, "ActiveCameraTargetSingleton");
                ecb.AddComponent(targetEntity, new ActiveCameraTargetComponent { Target = Entity.Null });
                ecb.AddComponent(targetEntity, LocalTransform.FromPosition(0, 0, 0));
                ecb.AddComponent(targetEntity, new LocalToWorld());
            }

            ecb.Playback(EntityManager);
        }
    }
}