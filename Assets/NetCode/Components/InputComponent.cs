using Unity.NetCode;

namespace NetCode.Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct InputComponent : IInputComponentData
    {
        public int Horizontal;
        public int Vertical;
    }
}