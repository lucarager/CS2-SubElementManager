namespace SubElementManager.Systems {
    using Game.Common;
    using Game.Prefabs;
    using Game.Tools;
    using LucaModsCommon.Systems;
    using Unity.Burst.Intrinsics;
    using Unity.Entities;
    using static SubElementManager.Systems.SEM_ToolbarUISystem;

    /// <summary>
    ///     System responsible for modifying the sub elements of target objects, either via query or via request
    /// </summary>
    public partial class SEM_RandomSeedFixerSystem : CommonGameSystemBase {
        private EntityQuery         m_Query;
        private SEM_ToolbarUISystem m_ToolbarUISystem;
        private Entity              m_CachedPrefabEntity = Entity.Null;
        private int                 m_CachedRandomSeed   = 0;

        /// <inheritdoc />
        protected override void OnCreate() {
            base.OnCreate();

            m_ToolbarUISystem = World.GetOrCreateSystemManaged<SEM_ToolbarUISystem>();
            m_Query           = SystemAPI.QueryBuilder().WithAll<CreationDefinition, Updated>().Build();

            RequireForUpdate(m_Query);
        }

        /// <inheritdoc />
        protected override void OnUpdate() {
            if (!m_ToolbarUISystem.CurrentToolOptions.HasFlag(SEM_ToolOptions.FixedRandomSeed)) {
                // Ensure our cache is clear
                m_CachedPrefabEntity = Entity.Null;
                return;
            }

            var job = new SEM_RandomSeedFixerJob {
                CreationDefinitionTypeHandle = SystemAPI.GetComponentTypeHandle<CreationDefinition>(),
                PrefabRefTypeHandle = SystemAPI.GetComponentTypeHandle<PrefabRef>(),
                CachedPrefabEntity = m_CachedPrefabEntity,
                CachedRandomSeed = m_CachedRandomSeed,
            };

            Dependency = job.Schedule(m_Query, Dependency);
            Dependency.Complete();

            m_CachedPrefabEntity = job.CachedPrefabEntity;
            m_CachedRandomSeed = job.CachedRandomSeed;
        }

#if USE_BURST
        [BurstCompile]
#endif
        private struct SEM_RandomSeedFixerJob : IJobChunk {
            public required ComponentTypeHandle<PrefabRef>          PrefabRefTypeHandle;
            public required            ComponentTypeHandle<CreationDefinition> CreationDefinitionTypeHandle;
            
            public required            Entity                                  CachedPrefabEntity;
            public                     int                                     CachedRandomSeed;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                                in v128           chunkEnabledMask) {
                var creationDefinitions = chunk.GetNativeArray(ref CreationDefinitionTypeHandle);
                var prefabRefs = chunk.GetNativeArray(ref PrefabRefTypeHandle);

                for (var i = 0; i < creationDefinitions.Length; i++) {
                    var creationDefinition = creationDefinitions[i];
                    var prefabRef = prefabRefs[i];

                    // Whenever we switch prefab, cache the first random seed.
                    if (CachedPrefabEntity != prefabRef.m_Prefab) {
                        CachedPrefabEntity = prefabRef.m_Prefab;
                        CachedRandomSeed   = creationDefinition.m_RandomSeed;
                    }

                    creationDefinition.m_RandomSeed = CachedRandomSeed;
                    creationDefinitions[i]          = creationDefinition;
                }
            }
        }
    }
}