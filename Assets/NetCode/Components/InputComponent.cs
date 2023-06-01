using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace NetCode.Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct InputComponent : IInputComponentData
    {
        public int Horizontal;
        public int Vertical;
    }
}