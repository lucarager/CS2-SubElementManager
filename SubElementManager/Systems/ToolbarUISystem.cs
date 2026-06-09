namespace SubElementManager.Systems {
    using System;
    using System.Runtime.Remoting.Metadata;

    using Game.Input;
    using Game.Tools;

    using LucaModsCommon.Extensions;
    using LucaModsCommon.Systems;

    using SubElementManager;

    public partial class SEM_ToolbarUISystem : CommonUISystemBase {
        protected override string ModId => SEM_Mod.Instance.Id;

        /// <summary>
        /// Options for the toolbar UI
        /// </summary>
        [Flags]
        public enum SEM_ToolOptions {
            // SubLanes - Boundaries
            NoBoundaryFence = 1 << 0,
            NoBoundaryHedge = 1 << 1,
            NoBoundaryAll = NoBoundaryFence | NoBoundaryHedge,
            // SubLanes - Markings
            NoMarkingLane = 1 << 2,
            NoMarkingAll = NoMarkingLane,
            // SubAreas - Surfaces
            NoSurfaceGrass = 1 << 3,
            NoSurfacePavement = 1 << 4,
            NoSurfaceAll = NoSurfaceGrass | NoSurfacePavement,
            // SubObjects - Vegetation
            NoVegetationTree = 1 << 5,
            NoVegetationShrub = 1 << 6,
            NoVegetationAll = NoVegetationTree | NoVegetationShrub,
            // SubObjects - Elements
            NoElementParking = 1 << 7,
            NoElementLights = 1 << 8,
            NoElementAll = NoElementParking | NoElementLights,
            // Random seed
            FixedRandomSeed = 1 << 9,
            // Meta
            None = 0,
            All = NoBoundaryAll | NoMarkingAll | NoSurfaceAll | NoVegetationAll | NoElementAll | FixedRandomSeed,
        }

        public SEM_ToolOptions CurrentToolOptions { get; set; } = SEM_ToolOptions.None;

        private ValueBindingHelper<int>  m_ToolOptionsBinding;
        private ValueBindingHelper<bool> m_EnableToolButtonsBinding;
        private bool CurrentlyUsingObjectTool => m_ToolSystem.activeTool is ObjectToolSystem;
        private ToolSystem m_ToolSystem;
        private ObjectToolSystem m_ObjectToolSystem;

        protected override void OnCreate() {
            base.OnCreate();

            m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            m_ObjectToolSystem = World.GetOrCreateSystemManaged<ObjectToolSystem>();

            m_ToolOptionsBinding = CreateBinding("TOOL_OPTIONS", (int)SEM_ToolOptions.None, HandleToolOptionsUpdate);
            m_EnableToolButtonsBinding = CreateBinding("ENABLE_TOOL_BUTTONS", false);
        }

        protected override void OnUpdate() {
            m_EnableToolButtonsBinding.Value = CurrentlyUsingObjectTool;
        }

        private void HandleToolOptionsUpdate(int value) {
            CurrentToolOptions = (SEM_ToolOptions)value;
        }

    }
}
