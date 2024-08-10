using UnityEngine;
using Cinemachine;
using Unity.Entities;
using Unity.Transforms;

public class SetCamera : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    private EntityManager entityManager;
    private Entity playerEntity;
    private Transform playerTransform;
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery playerQuery = entityManager.CreateEntityQuery(typeof(PlayerTag));

        if (playerQuery.CalculateEntityCount() > 0)
        {
            playerEntity = playerQuery.GetSingletonEntity();
        }
    }
    void Update()
    {
        if (playerEntity != Entity.Null)
        {
            LocalToWorld playerLocalToWorld = entityManager.GetComponentData<LocalToWorld>(playerEntity);
            Vector3 playerPosition = playerLocalToWorld.Position;
            if (playerTransform == null)
            {
                GameObject playerGO = new GameObject("Player Follow Target");
                playerTransform = playerGO.transform;
            }
            playerTransform.position = playerPosition;
            VirtualCamera.Follow = playerTransform;
        }
    }
}
