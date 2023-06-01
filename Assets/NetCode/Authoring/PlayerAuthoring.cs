using NetCode.Components;
using Unity.Entities;
using UnityEngine;

namespace NetCode.Authoring
{
    [DisallowMultipleComponent]
    public class PlayerAuthoring : MonoBehaviour
    {
        public int spookyLevel;

        public class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Player { SpookyLevel = authoring.spookyLevel });
            }
        }
    }
}