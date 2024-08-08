using Unity.Entities;
using Unity.Mathematics;

public struct SpawnerProperties : IComponentData
{
    public float2 MapBorder;
    public int NumberEnemyToSpawn;
    public float SpawnRate;
    public float SpawnCounter;
    public float SpeedIncreaseRate;
    public float EnemyIncreaseRate;
    public float MaxSpeed;
    public int MaxEnemies;
    public int IncreaseThreshold;
    public int CurrentSpawnCount;
    public Entity EnemyPrefab;
}
