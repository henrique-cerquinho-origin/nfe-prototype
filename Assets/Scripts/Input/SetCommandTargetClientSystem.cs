using Unity.Entities;
using Unity.NetCode;

namespace Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct SetCommandTargetClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate(
                SystemAPI.QueryBuilder()
                    .WithAll<GhostOwnerIsLocal, PlayerInputComponent>()
                    .Build());
            state.RequireForUpdate<CommandTarget>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Entity cmdTargetEnt = SystemAPI.GetSingletonEntity<CommandTarget>();
            RefRW<CommandTarget> cmdTargetRef = SystemAPI.GetComponentRW<CommandTarget>(cmdTargetEnt);
            
            if (cmdTargetRef.ValueRO.targetEntity != Entity.Null)
                return;
            
            foreach (var (_, entity) in SystemAPI.Query<RefRO<GhostOwnerIsLocal>>()
                .WithAll<PlayerInputComponent>()
                .WithEntityAccess())
            {
                cmdTargetRef.ValueRW.targetEntity = entity;
                break;
            }
        }
    }
}