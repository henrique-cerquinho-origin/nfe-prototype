using Camera;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputActions.PlayerActions _defaultActionsMap;
        
        protected override void OnCreate()
        {
            var inputActions = new PlayerInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            _defaultActionsMap = inputActions.Player;
        
            // RequireForUpdate<FixedTickSystem.Singleton>();
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerInputComponent, PlayerCameraRefComponent>().Build());
        }

        protected override void OnUpdate()
        {
            foreach ((var playerInputsRef, var playerCameraRef) in SystemAPI
                .Query<RefRW<PlayerInputComponent>, RefRO<PlayerCameraRefComponent>>())
            {
                if (!_defaultActionsMap.Look.IsPressed())
                {
                    playerInputsRef.ValueRW.LookDelta = float2.zero;
                }
                else
                {
                    playerInputsRef.ValueRW.LookDelta = _defaultActionsMap.LookDelta.ReadValue<Vector2>();
                }
                
                playerInputsRef.ValueRW.MoveDelta = _defaultActionsMap.Move.ReadValue<Vector2>();
                SystemAPI.GetComponentRW<CameraControlComponent>(playerCameraRef.ValueRO.Camera)
                    .ValueRW.LookDelta = playerInputsRef.ValueRW.LookDelta;
            }
        }
    }
}