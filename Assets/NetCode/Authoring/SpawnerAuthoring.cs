using NetCode.Components;
using Unity.Entities;
using UnityEngine;

namespace NetCode.Authoring
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject playerToSpawn;

        public class SpawnerBaker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity,
                    new Spawner { PlayerEntity = GetEntity(authoring.playerToSpawn, TransformUsageFlags.Dynamic) });
            }
        }                                                                                                                                                         
    }
}