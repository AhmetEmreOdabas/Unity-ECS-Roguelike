using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        foreach (Entity entity in allEntities)
        {
            if(entityManager.HasComponent<BulletComponent>(entity) && entityManager.HasComponent<BulletLifeTimeComponent>(entity))
            {
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
                localTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * localTransform.Forward();
                entityManager.SetComponentData(entity, localTransform);
                BulletLifeTimeComponent bulletLifeTimeComponent = entityManager.GetComponentData<BulletLifeTimeComponent>(entity);
                bulletLifeTimeComponent.LifeTime -= SystemAPI.Time.DeltaTime;
                if(bulletLifeTimeComponent.LifeTime <= 0)
                {
                    entityManager.DestroyEntity(entity);
                    continue;
                }
                entityManager.SetComponentData(entity, bulletLifeTimeComponent);
                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                physicsWorldSingleton.SphereCastAll(localTransform.Position, bulletComponent.Size, float3.zero, bulletComponent.BullletRadius, ref hits, new CollisionFilter 
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = (uint)CollisionLayer.Enemy,
                });
                if(hits.Length > 0) 
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        Entity hitEntity = hits[i].Entity;
                        if(entityManager.HasComponent<EnemyComponent>(hitEntity)) 
                        {
                            EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(hitEntity);
                            enemyComponent.CurrentHealth -= bulletComponent.Damage;
                            entityManager.SetComponentData(hitEntity, enemyComponent);
                            if(enemyComponent.CurrentHealth <= 0) 
                            {
                                entityManager.DestroyEntity(hitEntity);
                            }
                        }
                    }
                    entityManager.DestroyEntity(entity);
                }
                hits.Dispose();
            }
        }
    }
}
