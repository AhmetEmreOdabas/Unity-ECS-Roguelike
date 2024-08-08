using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawnerMono : MonoBehaviour
{
    public float2 MapBorder;
    public int NumberEnemyToSpawn;
    public GameObject EnemyPrefab;
    public float SpawnRate;
    public float SpeedIncreaseRate;
    public float EnemyIncreaseRate;
    public float MaxSpeed;
    public int MaxEnemies;
    public int IncreaseThreshold;
}

public class SpawnerBaker : Baker<EnemySpawnerMono>
{
    public override void Bake(EnemySpawnerMono authoring)
    {
        var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        AddComponent(entity, new SpawnerProperties()
        {
            MapBorder = authoring.MapBorder,
            NumberEnemyToSpawn = authoring.NumberEnemyToSpawn,
            SpawnRate = authoring.SpawnRate,
            SpawnCounter = authoring.SpawnRate,
            SpeedIncreaseRate = authoring.SpeedIncreaseRate,
            EnemyIncreaseRate = authoring.EnemyIncreaseRate,
            MaxSpeed = authoring.MaxSpeed,
            MaxEnemies = authoring.MaxEnemies,
            IncreaseThreshold = authoring.IncreaseThreshold,
            CurrentSpawnCount = 0,
            EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic)
        });
    }
}