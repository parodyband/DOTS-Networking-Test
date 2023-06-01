using NetCode.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace NetCode.Systems
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var left = Input.GetKey(KeyCode.A);
            var right = Input.GetKey(KeyCode.D);
            var up = Input.GetKey(KeyCode.W);
            var down = Input.GetKey(KeyCode.S);

            foreach (var input in SystemAPI.Query<RefRW<InputComponent>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                input.ValueRW = default;
                if (left)
                    input.ValueRW.Horizontal -= 1;
                if (right)
                    input.ValueRW.Horizontal += 1;
                if (up)
                    input.ValueRW.Vertical += 1;
                if (down)
                    input.ValueRW.Vertical -= 1;
            }

        }
    }
}