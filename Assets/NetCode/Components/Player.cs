using Unity.Entities;
using Unity.NetCode;
using Unity.Rendering;

namespace NetCode.Components
{
    [MaterialProperty("_Spooky_Level")]
    public struct Player : IInputComponentData
    {
        [GhostField]
        public float Value;
    }
}