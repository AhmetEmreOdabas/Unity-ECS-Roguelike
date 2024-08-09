using Unity.Entities;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{
    public float MoveSpeed;
    public float RotationSpeed;
    public GameObject BulletPrefab;
    public float BulletSpeed;
    public float BulletLifeTime;

    public class PlayerBaker : Baker<PlayerMono>
    {
        public override void Bake(PlayerMono authoring)
        {
            var playerEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(playerEntity);
            AddComponent<PlayerMoveInput>(playerEntity);
            AddComponent(playerEntity, new PlayerProperties
            {
                MoveSpeed = authoring.MoveSpeed,
                RotationSpeed = authoring.RotationSpeed,
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None),
                BulletCount = 3,
                BulletSpeed = authoring.BulletSpeed,
                BulletLifeTime = authoring.BulletLifeTime,
            });
            AddComponent(playerEntity, new PlayerShooting{});
        }
    }
}  
