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
        /// Options for the toolbar UI, used to determine which sub elements to remove.
        /// </summary>
        [Flags]
        public enum SEM_ToolOptions {
            None = 0,
            NoFence = 1 >> 0,
            NoSurfaceGrass = 1 << 1,
            NoSurfacePavement = 1 << 2,
            NoSurface = 1 << 3,
            NoVegetation = 1 << 4,
            FixedRandomSeed = 1 << 5,
            All = NoFence | NoSurfaceGrass | NoSurface | NoSurfacePavement | NoVegetation,
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
