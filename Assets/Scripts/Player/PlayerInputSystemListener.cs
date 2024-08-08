using Unity.Entities;
using UnityEngine;

public partial class PlayerInputSystemListener : SystemBase
{
    private PlayerInputs _playerInputs;
    private Entity _playerEntity;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<PlayerMoveInput>();
        _playerInputs = new PlayerInputs();
    }

    protected override void OnStartRunning()
    {
        _playerInputs.Enable();
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
    }

    protected override void OnUpdate()
    {
        var moveInput = _playerInputs.PlayerActionMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput { Value = moveInput });
    }

    protected override void OnStopRunning()
    {
        _playerInputs.Disable();
        _playerEntity = Entity.Null;
    }
}
