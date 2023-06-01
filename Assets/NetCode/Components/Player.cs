using Unity.Entities;
using Unity.NetCode;

namespace NetCode.Components
{
    public struct Player : IComponentData
    {
        [GhostField]
        public int SpookyLevel;
    }
}
