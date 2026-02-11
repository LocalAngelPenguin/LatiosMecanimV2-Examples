using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpImpulse = 5f;
    public float mouseSensitivity = 2f;
     public GameObject cameraTarget;

    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(entity);
             AddComponent<PlayerInput>(entity);

            AddComponent(entity, new PlayerMoveData
            {
                MoveSpeed = authoring.moveSpeed,
                JumpImpulse = authoring.jumpImpulse,
                MouseSensitivity = authoring.mouseSensitivity,
            });
        
            AddComponent(entity, new PlayerCameraTarget
            {
                Target = GetEntity(authoring.cameraTarget, TransformUsageFlags.Dynamic)
            });
        
        }
    }
}
