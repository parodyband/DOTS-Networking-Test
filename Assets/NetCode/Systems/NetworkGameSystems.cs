using NetCode.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace NetCode.Systems
{
    // The client-side system for handling the transition to the in-game state
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInGameClientSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            // Define a query that only targets entities with a NetworkId component and no NetworkStreamInGame component
            var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NetworkId>()
                .WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(queryBuilder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Create a command buffer for modifying entities
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            // Iterate over entities matching the previous query
            foreach (var (networkId, entity) in SystemAPI.Query<RefRO<NetworkId>>()
                .WithEntityAccess().WithNone<NetworkStreamInGame>())
            {
                // Add the NetworkStreamInGame component to the entity
                commandBuffer.AddComponent<NetworkStreamInGame>(entity);

                // Create a new entity for the GoInGameRPC request
                var rpcRequestEntity = commandBuffer.CreateEntity();

                // Add the GoInGameRPC and SendRpcCommandRequest components to the new entity
                commandBuffer.AddComponent<GoInGameRPC>(rpcRequestEntity);
                commandBuffer.AddComponent(rpcRequestEntity, new SendRpcCommandRequest { TargetConnection = entity });
            }
            commandBuffer.Playback(state.EntityManager);
        }
    }

    // The server-side system for handling the transition to the in-game state
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInGameServerSystem : ISystem
    {
        private ComponentLookup<NetworkId> m_NetworkIdFromEntity;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            // Define a query that targets entities with both GoInGameRPC and ReceiveRpcCommandRequest components
            var queryBuilder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GoInGameRPC>()
                .WithAll<ReceiveRpcCommandRequest>();

            state.RequireForUpdate(state.GetEntityQuery(queryBuilder));

            // Initialize the NetworkIdLookup for accessing NetworkId components
            m_NetworkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefab = SystemAPI.GetSingleton<Spawner>().PlayerEntity;
            state.EntityManager.GetName(prefab,out var prefabName);
            // Get the name of the current world
            var worldName = new FixedString32Bytes(state.WorldUnmanaged.Name);

            // Create a command buffer for modifying entities
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            // Update the NetworkIdLookup with the current state
            m_NetworkIdFromEntity.Update(ref state);

            // Iterate over entities matching the previous query
            foreach (var (rpcRequest, requestEntity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
                         .WithAll<GoInGameRPC>()
                         .WithEntityAccess())
            {
                // Add the NetworkStreamInGame component to the source connection entity
                commandBuffer.AddComponent<NetworkStreamInGame>(rpcRequest.ValueRO.SourceConnection);

                // Get the NetworkId for the source connection entity
                var networkId = m_NetworkIdFromEntity[rpcRequest.ValueRO.SourceConnection];
                
                var player = commandBuffer.Instantiate(prefab);
                
                commandBuffer.SetComponent(player, new GhostOwner{ NetworkId = networkId.Value });
                
                commandBuffer.AppendToBuffer(rpcRequest.ValueRO.SourceConnection, new LinkedEntityGroup{Value = player});

                // Log connection details
                Debug.Log($"{worldName} connecting {networkId.Value} to game, spawning a player {prefabName}");

                // Destroy the current GoInGameRPC request entity
                commandBuffer.DestroyEntity(requestEntity);
            }
            
            // Apply the command buffer modifications
            commandBuffer.Playback(state.EntityManager);
        }
    }
}
