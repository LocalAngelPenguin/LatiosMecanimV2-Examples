using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct PlayerInput : IComponentData
{
    public Vector2 Move;
    public Vector2 Look;
    public bool Jump;
    public float yaw;
    public float pitch;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    const float maxPitchRad = 89f * (math.PI / 180f);
    private PlayerInputActions actions;

    protected override void OnCreate()
    {
        actions = new PlayerInputActions();
        actions.Enable();
    }

    protected override void OnUpdate()
    {
        Vector2 move = actions.Player.Move.ReadValue<Vector2>();
        Vector2 look = actions.Player.Look.ReadValue<Vector2>();
        bool jump = actions.Player.Jump.WasPressedThisFrame();

        foreach (var (input, moveData) in SystemAPI.Query<RefRW<PlayerInput>, RefRW<PlayerMoveData>>())
        {
            float yaw = input.ValueRO.yaw + look.x * moveData.ValueRO.MouseSensitivity;
            float pitch = input.ValueRO.pitch - look.y * moveData.ValueRO.MouseSensitivity;
            pitch = math.clamp(pitch, -maxPitchRad, maxPitchRad);

            input.ValueRW.yaw = yaw;
            input.ValueRW.pitch = pitch;

            input.ValueRW.Move = move;
            input.ValueRW.Jump = jump;
        }
    }

    protected override void OnDestroy()
    {
        actions.Disable();
    }
}
