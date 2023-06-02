using NetCode.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace NetCode.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<InputComponent>()
                .WithAll<Simulate>();
            state.RequireForUpdate(state.GetEntityQuery(queryBuilder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveJob = new MoveJob()
            {
                Speed = 4 * SystemAPI.Time.DeltaTime
            };

            state.Dependency = moveJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }

    public partial struct MoveJob : IJobEntity
    {
        public float Speed;

        private void Execute(InputComponent input, RefRW<LocalTransform> transform)
        {
            var move = new float2(input.Horizontal, input.Vertical);
            move = math.normalizesafe(move) * Speed;
            transform.ValueRW.Position += new float3(move.x, 0, move.y);
        }
    }
}