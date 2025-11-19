using Camera;
using Player;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [AlwaysSynchronizeSystem]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputActions.PlayerActions _defaultActionsMap;

        protected override void OnCreate()
        {
            var inputActions = new PlayerInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            _defaultActionsMap = inputActions.Player;

            RequireForUpdate<CommandTarget>();
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerCameraRefComponent>().Build());
        }

        protected override void OnUpdate()
        {
            Entity cmdTargetEnt = SystemAPI.GetSingletonEntity<CommandTarget>();
            Entity targetPlayer = SystemAPI.GetComponentRO<CommandTarget>(cmdTargetEnt).ValueRO.targetEntity;
            Entity cameraEntity = SystemAPI.GetComponentRO<PlayerCameraRefComponent>(targetPlayer).ValueRO.Camera;
            var thirdPersonCamera = SystemAPI.GetComponent<ThirdPersonCameraComponent>(cameraEntity);
            if (targetPlayer == Entity.Null) return;

            Vector2 look = _defaultActionsMap.Look.IsPressed()
                ? _defaultActionsMap.LookDelta.ReadValue<Vector2>()
                : Vector2.zero;
            var move = _defaultActionsMap.Move.ReadValue<Vector2>();

            var inputRef = SystemAPI.GetComponentRW<PlayerInputComponent>(targetPlayer);
            inputRef.ValueRW.LookDelta = new float2(look.x, look.y);
            inputRef.ValueRW.MoveDelta = new float2(move.x, move.y);
            inputRef.ValueRW.CurrentCameraAngle = thirdPersonCamera.CurrentTheta;
        }
    }
}