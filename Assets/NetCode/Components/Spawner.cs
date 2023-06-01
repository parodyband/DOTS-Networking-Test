using Unity.Entities;

namespace NetCode.Components
{
    public struct Spawner : IComponentData
    {
        public Entity PlayerEntity;
    }
}