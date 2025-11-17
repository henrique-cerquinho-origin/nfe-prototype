using Camera;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Input
{
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
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerInputComponent>().Build());
        }

        protected override void OnUpdate()
        {
            foreach (RefRW<PlayerInputComponent> playerInputsRef in SystemAPI.Query<RefRW<PlayerInputComponent>>())
            {
                if (!_defaultActionsMap.Look.IsPressed())
                {
                    playerInputsRef.ValueRW.LookDelta = float2.zero;
                }
                else
                {
                    playerInputsRef.ValueRW.LookDelta = _defaultActionsMap.LookDelta.ReadValue<Vector2>();
                }

                if (SystemAPI.HasComponent<CameraControlComponent>(playerInputsRef.ValueRO.ControllingCamera))
                {
                    SystemAPI.GetComponentRW<CameraControlComponent>(playerInputsRef.ValueRO.ControllingCamera)
                        .ValueRW.LookDelta = playerInputsRef.ValueRW.LookDelta;
                }
            }
        }
    }
}