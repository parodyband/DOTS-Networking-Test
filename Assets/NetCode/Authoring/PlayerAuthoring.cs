using NetCode.Components;
using Unity.Entities;
using UnityEngine;

namespace NetCode.Authoring
{
    [DisallowMultipleComponent]
    public class PlayerAuthoring : MonoBehaviour
    {
        public float spookyLevel;

        public class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Player { Value = authoring.spookyLevel });
            }
        }
    }
}