using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemySystem : ISystem
{
    private EntityManager _entityManager;
    private Entity _playerEntity;
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerProperties>();
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);

        NativeArray<Entity> allEntities = _entityManager.GetAllEntities();

        foreach (Entity entity in allEntities)
        {
            if(_entityManager.HasComponent<EnemyComponent>(entity))
            {
                LocalTransform enemyTransform = _entityManager.GetComponentData<LocalTransform>(entity);
                EnemyComponent enemyComponent = _entityManager.GetComponentData<EnemyComponent>(entity);
                float3 moveDir = math.normalize(playerTransform.Position - enemyTransform.Position);
                enemyTransform.Position += enemyComponent.MoveSpeed * SystemAPI.Time.DeltaTime * moveDir;
                quaternion enemyRotation = quaternion.LookRotationSafe(moveDir, math.up());
                enemyTransform.Rotation = math.slerp(enemyTransform.Rotation, quaternion.LookRotation(moveDir, math.up()), 10f * SystemAPI.Time.DeltaTime);
                _entityManager.SetComponentData(entity, enemyTransform);
            }
        }
    }
}
