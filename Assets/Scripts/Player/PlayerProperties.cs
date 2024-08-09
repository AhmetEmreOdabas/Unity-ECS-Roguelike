using Unity.Entities;

public struct PlayerProperties : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;
    public Entity BulletPrefab;
    public int BulletCount;
    public float BulletAngle;
    public float BulletSpeed;
    public float BulletLifeTime;
}
