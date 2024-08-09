using Unity.Entities;
using UnityEngine;

public partial class PlayerInputSystemListener : SystemBase
{
    private PlayerInputs _playerInputs;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<PlayerMoveInput>();
        _playerInputs = new PlayerInputs();
    }

    protected override void OnStartRunning()
    {
        _playerInputs.Enable();
    }

    protected override void OnUpdate()
    {
        var moveInput = _playerInputs.PlayerActionMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput { Value = moveInput });
        bool shootButton = _playerInputs.PlayerActionMap.PlayerShoot.IsPressed();
        SystemAPI.SetSingleton(new PlayerShooting {Shoot = shootButton });
    }

    protected override void OnStopRunning()
    {
        _playerInputs.Disable();
    }
}
