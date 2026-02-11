using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
public struct PlayerCameraTarget : IComponentData
{
    public Entity Target;
}

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class PlayerCameraSystem : SystemBase
{
    private Camera mainCamera;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerMoveData>();
    }

    protected override void OnUpdate()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) return;

        foreach (var (input, camTarget) in
                 SystemAPI.Query<RefRO<PlayerInput>, RefRO<PlayerCameraTarget>>()
                 .WithAll<PlayerTag>())
        {
            mainCamera.transform.rotation = math.mul(quaternion.RotateY(input.ValueRO.yaw), quaternion.RotateX(input.ValueRO.pitch));
            mainCamera.transform.position = SystemAPI.GetComponent<LocalToWorld>(camTarget.ValueRO.Target).Position;
            break;
        }
    }
}