using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{
    public float MoveSpeed;
    public float RotationSpeed;

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
                RotationSpeed = authoring.RotationSpeed
            });
        }
    }
}  
