namespace SubElementManager.Systems {
    using Colossal.Entities;
    using Game.Areas;
    using Game.Common;
    using Game.Objects;
    using Game.Tools;
    using LucaModsCommon.Systems;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    ///     Deletes sub-elements of target objects. Runs in Modification4 and defers all
    ///     structural changes through ModificationBarrier4B so the Mod5 search systems and the
    ///     ModificationEnd ValidationSystem see consistent post-delete state.
    /// </summary>
    public partial class SEM_SubElementDeleteSystem : CommonGameSystemBase {
        private ComponentTypeSet      m_AppliedTypes;
        private ModificationBarrier4B m_Barrier4B;
        private EntityQuery           m_Query;
        private SEM_ToolbarUISystem   m_ToolbarUISystem;

        /// <inheritdoc />
        protected override void OnCreate() {
            base.OnCreate();

            m_Query = SystemAPI.QueryBuilder()
                               .WithAll<Created>()
                               .WithAny<SubArea, SubObject>()
                               .WithNone<Deleted>()
                               .AddAdditionalQuery()
                               .WithAll<Temp, Updated>()
                               .WithAny<SubArea, SubObject>()
                               .WithNone<Deleted>()
                               .Build();

            m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(),
                                                  ComponentType.ReadWrite<Created>(),
                                                  ComponentType.ReadWrite<Updated>());

            m_ToolbarUISystem = World.GetOrCreateSystemManaged<SEM_ToolbarUISystem>();
            m_Barrier4B       = World.GetExistingSystemManaged<ModificationBarrier4B>();

            RequireAnyForUpdate(m_Query);
        }

        /// <inheritdoc />
        protected override void OnUpdate() {
            var ecb = m_Barrier4B.CreateCommandBuffer();

            // As a test, delete all subareas and subobjects that are created
            foreach (var entity in m_Query.ToEntityArray(Allocator.Temp)) {
                var anyRemoved = false;


                if (EntityManager.TryGetBuffer<SubArea>(entity, true, out var subAreaBuffer)) {
                    foreach (var subArea in subAreaBuffer) {
                        if (EntityManager.HasComponent<Deleted>(subArea.m_Area)) {
                            continue;
                        }

                        ecb.RemoveComponent(subArea.m_Area, m_AppliedTypes);
                        ecb.AddComponent(subArea.m_Area, typeof(Deleted));

                        anyRemoved = true;
                    }
                }


                if (EntityManager.TryGetBuffer<SubObject>(entity, true, out var subObjectBuffer)) {
                    foreach (var subObject in subObjectBuffer) {
                        if (EntityManager.HasComponent<Deleted>(subObject.m_SubObject)) {
                            continue;
                        }

                        ecb.RemoveComponent(subObject.m_SubObject, m_AppliedTypes);
                        ecb.AddComponent(subObject.m_SubObject, typeof(Deleted));

                        anyRemoved = true;
                    }
                }

                if (anyRemoved) {
                    ecb.AddComponent(entity, typeof(Updated));
                }
            }
        }
    }
}