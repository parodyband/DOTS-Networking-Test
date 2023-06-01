using NetCode.Components;
using Unity.Entities;
using UnityEngine;

namespace NetCode.Authoring
{
    public class InputAuthoring : MonoBehaviour
    {
        public class InputComponentBaker : Baker<InputAuthoring>
        {
            public override void Bake(InputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new InputComponent());
            }
        }
    }
}