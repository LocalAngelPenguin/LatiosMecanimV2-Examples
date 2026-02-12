using Latios.Kinemation;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class SkeletonAspectBakingSystem : SystemBase
{
    private EntityQuery query;

    protected override void OnCreate()
    {
        query = GetEntityQuery(new EntityQueryDesc
        {
            Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities,
            All = new[]
            {
            ComponentType.ReadWrite<OptimizedSkeletonState>(),
        }
        });
    }

    protected override void OnUpdate()
    {
        var entities = query.ToEntityArray(Allocator.Temp);

        var em = World.EntityManager;

        for (int i = 0; i < entities.Length; i++)
        {
            var skeleton = em.GetAspect<OptimizedSkeletonAspect>(entities[i]);
            skeleton.ForceInitialize();
        }

        entities.Dispose();
        Enabled = false;
    }
}
