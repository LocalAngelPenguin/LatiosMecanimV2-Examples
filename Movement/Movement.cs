using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Latios.Mecanim;

public struct PlayerTag : IComponentData { }
public struct PlayerMoveData : IComponentData
{
    public float MoveSpeed;
    public float JumpImpulse;
    public float MouseSensitivity;
    public bool Moving;

}
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, velocity, moveData, mass, input, ent) in
                 SystemAPI.Query<RefRW<LocalTransform>,
                                 RefRW<PhysicsVelocity>,
                                 RefRW<PlayerMoveData>,
                                 RefRW<PhysicsMass>,
                                 RefRW<PlayerInput>>()
                 .WithAll<PlayerTag>().WithEntityAccess())
        {

            mass.ValueRW.InverseInertia = float3.zero;

            transform.ValueRW.Rotation = quaternion.RotateY(input.ValueRO.yaw);

            float3 forward = math.forward(transform.ValueRO.Rotation);
            float3 right = math.mul(transform.ValueRO.Rotation, new float3(1, 0, 0));

            float3 moveDir = forward * input.ValueRO.Move.y + right * input.ValueRO.Move.x;
            moveDir = math.normalizesafe(moveDir);

            velocity.ValueRW.Linear.x = moveDir.x * moveData.ValueRO.MoveSpeed;
            velocity.ValueRW.Linear.z = moveDir.z * moveData.ValueRO.MoveSpeed;

            if (input.ValueRO.Jump && math.abs(velocity.ValueRO.Linear.y) < 0.01f)
            {
                velocity.ValueRW.Linear.y += moveData.ValueRO.JumpImpulse;
            }

            float3 horizontal = velocity.ValueRO.Linear;
            horizontal.y = 0;
            float speed = math.length(horizontal);

            if (!SystemAPI.HasComponent<MecanimController>(ent)) continue;

            var anim = SystemAPI.GetAspect<MecanimAspect>(ent);

            anim.SetFloatParameter("speed", speed);
            if (speed > 0.1f)
            {
                if(!moveData.ValueRO.Moving)
                {
                    moveData.ValueRW.Moving = true;
                    anim.StartInertialBlend(0.4f);
                }
            }
            else
            {
                if (moveData.ValueRO.Moving)
                {
                    moveData.ValueRW.Moving = false;
                    anim.StartInertialBlend(0.4f);
                }
            }

        }
    }
}
