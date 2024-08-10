using Unity.Entities;

public struct BulletComponent : IComponentData
{
    public float Speed;
    public float Size;
    public float BullletRadius;
    public int Damage;
}
