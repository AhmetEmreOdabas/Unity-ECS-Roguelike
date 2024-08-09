using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct BulletSystem : ISystem
{
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

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
            }
        }
    }
}
