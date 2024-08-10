using Unity.Entities;

public struct EnemyComponent : IComponentData
{
    public int CurrentHealth;
    public float MoveSpeed;
}
