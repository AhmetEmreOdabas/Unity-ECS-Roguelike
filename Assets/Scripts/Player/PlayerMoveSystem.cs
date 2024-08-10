using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct PlayerMoveSystem : ISystem
{
    private EntityManager _entityManager;
    private Entity _playerShootEntity;
    private Entity _playerPropertiesEntity;
    private PlayerProperties _playerProperties;
    private PlayerShooting _playerShooting;
    public void OnCreate(ref SystemState state)
    {
        
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        _playerShootEntity = SystemAPI.GetSingletonEntity<PlayerShooting>();
        _playerShooting = _entityManager.GetComponentData<PlayerShooting>(_playerShootEntity);
        _playerPropertiesEntity = SystemAPI.GetSingletonEntity<PlayerProperties>();
        _playerProperties = _entityManager.GetComponentData<PlayerProperties>(_playerPropertiesEntity);
        LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerShootEntity);
        var deltaTime = SystemAPI.Time.DeltaTime;
        new PlayerMoveJob
        {
            DeltaTime = deltaTime,
        }.Schedule();
        Shoot(ref state);
    }
    [BurstCompile]
    private void Shoot(ref SystemState state)
    {
        if(_playerShooting.Shoot)
        {
            for (int i = 0; i < _playerProperties.BulletCount; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
                Entity bulletEntity = _entityManager.Instantiate(_playerProperties.BulletPrefab);
                ECB.AddComponent(bulletEntity, new BulletComponent 
                {
                    Speed = _playerProperties.BulletSpeed,
                    BullletRadius = 1f,
                    Size = 0.5f,
                    Damage = 5
                });
                ECB.AddComponent(bulletEntity, new BulletLifeTimeComponent
                {
                    LifeTime = _playerProperties.BulletLifeTime
                });
                float angleStep = 20f;
                float startAngle = -(angleStep * (_playerProperties.BulletCount - 1)) / 2;
                float angle = startAngle + i * angleStep;
                LocalTransform bulletTransform = _entityManager.GetComponentData<LocalTransform>(bulletEntity);
                LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerPropertiesEntity);
                bulletTransform.Position = playerTransform.Position + playerTransform.Up() + playerTransform.Forward() * 2f;
                quaternion rotationOffset = quaternion.RotateY(math.radians(angle));
                bulletTransform.Rotation = math.mul(playerTransform.Rotation, rotationOffset);
                ECB.SetComponent(bulletEntity, bulletTransform);
                ECB.Playback(_entityManager);
                ECB.Dispose();
            }
        }
    }
    [BurstCompile]
    public partial struct PlayerMoveJob : IJobEntity
    {
        public float DeltaTime;
        [BurstCompile]
        private void Execute(ref LocalTransform transform, in PlayerMoveInput moveInput, PlayerProperties playerProperties, PlayerShooting playerShooting)
        {
            transform.Position.xz += moveInput.Value * playerProperties.MoveSpeed * DeltaTime;
            if (math.lengthsq(moveInput.Value) > float.Epsilon)
            {
                var forward = new float3(moveInput.Value.x, 0f, moveInput.Value.y);
                transform.Rotation = math.slerp(transform.Rotation, quaternion.LookRotation(forward, math.up()), playerProperties.RotationSpeed * DeltaTime);
            }
        }
    }
}
