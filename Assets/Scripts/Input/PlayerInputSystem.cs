using Camera;
using Player;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class PlayerInputSystem : SystemBase
    {
        private PlayerInputActions.PlayerActions _defaultActionsMap;
        
        protected override void OnCreate()
        {
            var inputActions = new PlayerInputActions();
            inputActions.Enable();
            inputActions.Player.Enable();
            _defaultActionsMap = inputActions.Player;
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerInputComponent>().Build());
        }

        protected override void OnUpdate()
        {
            foreach (var playerInputsRef in SystemAPI.Query<RefRW<PlayerInputComponent>>())
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
            }
        }
    }
}