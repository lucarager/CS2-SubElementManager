namespace SubElementManager.Systems {
    using System.Collections.Generic;

    using Colossal.Entities;
    using Game.Common;
    using Game.Objects;
    using Game.Tools;
    using LucaModsCommon.Systems;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// Deletes an object's sub-OBJECTS (trees, props, decorations, …). These are created by
    /// <see cref="SubObjectSystem"/> directly from the prefab buffer — there is no
    /// definition to intercept — so deletion is the only per-instance lever.
    ///
    /// Timing matters: <see cref="SubObjectReferencesSystem"/> (Mod3) is the ONLY system that removes
    /// a deleted sub-object from its owner's <see cref="SubObject"/> buffer, and only while the entity
    /// is still alive (CleanUpSystem destroys the entity but never touches owner buffers). Because we
    /// run AFTER that reference pass (so the buffer is populated and current), refs won't get another
    /// chance — so we clean the owner buffer ourselves to avoid leaving a dangling reference to a
    /// soon-to-be-destroyed entity.
    ///
    /// We deliberately do NOT mark the owner <see cref="Updated"/>: that would make
    /// <see cref="SubObjectSystem"/> re-create the deleted sub-objects from the prefab next frame.
    ///
    /// </summary>
    public partial class SEM_SubElementDeleteSystem : CommonGameSystemBase {
        private ComponentTypeSet     m_AppliedTypes;
        private ModificationBarrier3 m_Barrier3;
        private EntityQuery          m_Query;
        private SEM_ToolbarUISystem  m_ToolbarUISystem;

        /// <inheritdoc />
        protected override void OnCreate() {
            base.OnCreate();

            // Owners being placed (apply: Created; preview: Temp + Updated) that carry a SubObject buffer.
            m_Query = SystemAPI.QueryBuilder()
                               .WithAll<Created>()
                               .WithAny<SubObject>()
                               .WithNone<Deleted>()
                               .AddAdditionalQuery()
                               .WithAll<Temp, Updated>()
                               .WithAny<SubObject>()
                               .WithNone<Deleted>()
                               .Build();

            m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(),
                                                  ComponentType.ReadWrite<Created>(),
                                                  ComponentType.ReadWrite<Updated>());

            m_ToolbarUISystem = World.GetOrCreateSystemManaged<SEM_ToolbarUISystem>();
            m_Barrier3        = World.GetExistingSystemManaged<ModificationBarrier3>();

            RequireAnyForUpdate(m_Query);
        }

        /// <inheritdoc />
        protected override void OnUpdate() {
            //var ecb = m_Barrier3.CreateCommandBuffer();

            //foreach (var owner in m_Query.ToEntityArray(Allocator.Temp)) {
            //    if (!EntityManager.TryGetBuffer<SubObject>(owner, true, out var subObjects)) {
            //        continue;
            //    }

            //    // Collect (read-only) which direct children we delete; ecb ops below are deferred,
            //    // so the read-only buffer handle stays valid through this loop.
            //    var deletedDirect = new HashSet<Entity>();
            //    foreach (var so in subObjects) {
            //        if (TryDelete(ecb, owner, so.m_SubObject)) {
            //            deletedDirect.Add(so.m_SubObject);
            //        }
            //    }

            //    if (deletedDirect.Count == 0) {
            //        continue;
            //    }

            //    // Remove the deleted entries from the owner's buffer right now. CleanUpSystem will
            //    // destroy the entities but does not touch owner buffers, and SubObjectReferencesSystem
            //    // already ran this frame — so without this the buffer would hold dangling references.
            //    // (Buffer-content edits are not a structural change, so this is safe on the main thread.)
            //    var buffer = EntityManager.GetBuffer<SubObject>(owner);
            //    for (var i = buffer.Length - 1; i >= 0; i--) {
            //        if (deletedDirect.Contains(buffer[i].m_SubObject)) {
            //            buffer.RemoveAt(i);
            //        }
            //    }
            //}
        }

        private bool TryDelete(EntityCommandBuffer ecb, Entity owner, Entity sub) {
            if (sub == Entity.Null || EntityManager.HasComponent<Deleted>(sub)) {
                return false;
            }

            // The game never strips these via the owner-survives path — they anchor
            // net/aggregate/secondary rendering and crash AggregateMeshSystem if orphaned.
            if (EntityManager.HasComponent<Secondary>(sub)
             || EntityManager.HasComponent<Game.Buildings.Building>(sub)
             || EntityManager.HasComponent<Game.Buildings.ServiceUpgrade>(sub)) {
                return false;
            }

            // Only delete things we actually own.
            if (EntityManager.TryGetComponent<Owner>(sub, out var o) && o.m_Owner != owner) {
                return false;
            }

            ecb.RemoveComponent(sub, m_AppliedTypes);
            ecb.AddComponent(sub, typeof(Deleted));

            // Cascade into nested sub-objects (their owner is `sub`, which is itself being destroyed,
            // so no buffer cleanup is needed for them).
            if (EntityManager.TryGetBuffer<SubObject>(sub, true, out var nested)) {
                foreach (var n in nested) {
                    TryDelete(ecb, sub, n.m_SubObject);
                }
            }

            return true;
        }
    }
}
