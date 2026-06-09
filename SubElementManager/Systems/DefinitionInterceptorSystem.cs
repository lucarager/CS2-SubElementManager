namespace SubElementManager.Systems {
    using System;

    using Colossal.Entities;
    using Game.Areas;
    using Game.Common;
    using Game.Prefabs;
    using Game.Tools;
    using LucaModsCommon.Systems;
    using Unity.Collections;
    using Unity.Entities;

    using static SubElementManager.Systems.SEM_ToolbarUISystem;

    /// <summary>
    /// Suppresses an object's definition-based sub-elements (Mechanism A: sub-areas and sub-nets)
    /// by destroying their <see cref="CreationDefinition"/> entities in PostTool, before the
    /// Generate* systems consume them in Modification1/2. Because nothing is ever instantiated,
    /// this avoids the created-then-deleted churn (and orphan/aggregate crashes) of post-creation
    /// deletion.
    ///
    /// Identification is by PREFAB (the only thing available at the definition stage):
    /// <list type="bullet">
    ///   <item>Sub-areas carry a <see cref="Game.Areas.Node"/> buffer; the structural
    ///   <see cref="AreaType.Lot"/> is always preserved. Grass/pavement are matched by prefab name.</item>
    ///   <item>Sub-nets carry a <see cref="NetCourse"/>; boundaries (fence/hedge) are matched by prefab name.</item>
    /// </list>
    ///
    /// NOTE: the runtime tags the spec referenced (Game.Net.LaneGeometry, Game.Objects.Plant) are
    /// archetype/instance components — they are added in NetLaneGeometryPrefab.GetArchetypeComponents,
    /// NOT on the prefab — so they do not exist yet at the definition stage and cannot be used here.
    /// The boundary branch logs each sub-net prefab name (debug) so hedge naming can be confirmed in-game.
    ///
    /// Sub-objects (vegetation/elements) have NO definition (Mechanism B, born in SubObjectSystem) and
    /// are handled by the separate deletion system, not here.
    /// </summary>
    public partial class SEM_DefinitionInterceptorSystem : CommonGameSystemBase {
        private const SEM_ToolOptions RelevantOptions = SEM_ToolOptions.NoSurfaceGrass
                                                      | SEM_ToolOptions.NoSurfacePavement
                                                      | SEM_ToolOptions.NoBoundaryFence
                                                      | SEM_ToolOptions.NoBoundaryHedge;

        private EntityQuery         m_Query;
        private PrefabSystem        m_PrefabSystem;
        private SEM_ToolbarUISystem m_ToolbarUISystem;

        /// <inheritdoc />
        protected override void OnCreate() {
            base.OnCreate();

            m_PrefabSystem    = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_ToolbarUISystem = World.GetOrCreateSystemManaged<SEM_ToolbarUISystem>();

            // Every tool-emitted sub-element definition shares these three; OwnerDefinition is what
            // separates sub-elements from the top-level object (and from player-drawn standalone areas).
            m_Query = SystemAPI.QueryBuilder()
                               .WithAll<CreationDefinition, OwnerDefinition, Updated>()
                               .Build();

            RequireForUpdate(m_Query);
        }

        /// <inheritdoc />
        protected override void OnUpdate() {
            var options = m_ToolbarUISystem.CurrentToolOptions;
            if ((options & RelevantOptions) == SEM_ToolOptions.None) {
                return;
            }

            using var entities = m_Query.ToEntityArray(Allocator.Temp);
            using var toRemove = new NativeList<Entity>(entities.Length, Allocator.Temp);

            foreach (var entity in entities) {
                var definition = EntityManager.GetComponentData<CreationDefinition>(entity);
                if (!TryGetPrefabName(definition.m_Prefab, out var name)) {
                    continue;
                }

                if (EntityManager.HasComponent<Node>(entity)) {
                    // Sub-area (surface). Never strip the structural lot.
                    if (EntityManager.TryGetComponent<AreaGeometryData>(definition.m_Prefab, out var geometry)
                        && geometry.m_Type == AreaType.Lot) {
                        continue;
                    }

                    if (options.HasFlag(SEM_ToolOptions.NoSurfaceGrass)
                        && NameContains(name, "grass") && NameContains(name, "surface")) {
                        toRemove.Add(entity);
                    } else if (options.HasFlag(SEM_ToolOptions.NoSurfacePavement)
                               && NameContains(name, "pavement") && NameContains(name, "surface")) {
                        toRemove.Add(entity);
                    }
                } else if (EntityManager.HasComponent<NetCourse>(entity)) {
                    // Sub-net (boundary). Log the prefab name so real fence/hedge naming can be confirmed.
                    m_Log.Debug($"[boundary] sub-net definition prefab='{name}'");

                    if (options.HasFlag(SEM_ToolOptions.NoBoundaryFence) && NameContains(name, "fence")) {
                        toRemove.Add(entity);
                    } else if (options.HasFlag(SEM_ToolOptions.NoBoundaryHedge) && NameContains(name, "hedge")) {
                        toRemove.Add(entity);
                    }
                }
                // Otherwise it's an ObjectDefinition (installed upgrade) — leave it alone.
            }

            if (toRemove.Length == 0) {
                return;
            }

            // Immediate structural change (main thread) so the Generate* systems never see these.
            EntityManager.DestroyEntity(toRemove.AsArray());
            m_Log.Debug($"Suppressed {toRemove.Length} sub-element definition(s).");
        }

        private bool TryGetPrefabName(Entity prefab, out string name) {
            if (prefab != Entity.Null && m_PrefabSystem.TryGetPrefab<PrefabBase>(prefab, out var prefabBase)) {
                name = prefabBase.name;
                return true;
            }

            name = null;
            return false;
        }

        private static bool NameContains(string name, string token) =>
            name != null && name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
