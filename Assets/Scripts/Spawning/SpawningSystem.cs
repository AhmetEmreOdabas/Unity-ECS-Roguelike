using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawningSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Random((uint)UnityEngine.Time.frameCount);
        EntityManager entityManager = state.EntityManager;
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        LocalTransform playerPosition = entityManager.GetComponentData<LocalTransform>(playerEntity);
        var spawnJob = new SpawnJob
        {
            DeltaTime = deltaTime,
            Ecb = ecb.AsParallelWriter(),
            Random = random,
            PlayerPos = playerPosition.Position
        };

        state.Dependency = spawnJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    private partial struct SpawnJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public Random Random;
        public float3 PlayerPos;
        public void Execute(ref SpawnerProperties spawner, [EntityIndexInQuery] int entityIndex)
        {
            spawner.SpawnCounter -= DeltaTime;
            if (spawner.SpawnCounter <= 0f)
            {
                for (int i = 0; i < spawner.NumberEnemyToSpawn; i++)
                {
                    float3 randomPosition = new float3(
                        Random.NextFloat(-spawner.MapBorder.x, spawner.MapBorder.x),
                        0f,
                        Random.NextFloat(-spawner.MapBorder.y, spawner.MapBorder.y)
                    );
                    randomPosition += new float3(PlayerPos.x, 0f, PlayerPos.z);
                    float3 direction = math.normalize(PlayerPos - randomPosition);
                    quaternion enemyRotation = quaternion.LookRotationSafe(direction, math.up());
                    var enemy = Ecb.Instantiate(entityIndex, spawner.EnemyPrefab);
                    Ecb.SetComponent(entityIndex, enemy, new LocalTransform
                    {
                        Position = randomPosition,
                        Rotation = enemyRotation,
                        Scale = 1f
                    });
                    Ecb.AddComponent(entityIndex, enemy, new EnemyComponent 
                    {
                        CurrentHealth = 50,
                        MoveSpeed = 5f
                    });
                    Ecb.AddComponent(entityIndex, enemy, new EnemyTag { });
                }
                spawner.SpawnCounter = spawner.SpawnRate;
                spawner.CurrentSpawnCount++;
                if (spawner.CurrentSpawnCount >= spawner.IncreaseThreshold)
                {
                    spawner.SpawnRate = math.max(spawner.MaxSpeed, spawner.SpawnRate - spawner.SpeedIncreaseRate);
                    spawner.NumberEnemyToSpawn = math.min(spawner.MaxEnemies, spawner.NumberEnemyToSpawn + (int)spawner.EnemyIncreaseRate);
                    spawner.CurrentSpawnCount = 0;
                }
            }
        }
    }
}
