using Unity.Entities;
using Unity.Mathematics;

public struct PlayerMoveInput : IComponentData
{
    public float2 Value;
    public float2 MousePos;
}
