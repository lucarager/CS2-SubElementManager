namespace SubElementManager.Systems {
    using System.Collections.Generic;
    using System.Linq;

    using Game.Common;
    using Game.Objects;
    using Game.Tools;
    using Game.Vehicles;

    using LucaModsCommon.Systems;

    using Unity.Entities;

    /// <summary>
    /// System responsible for modifying the sub elements of target objects, either via query or via request
    /// </summary>
    public partial class SEM_SubElementModificationSystem : CommonGameSystemBase {
       /// <inheritdoc/>
        protected override void OnCreate() {
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate() {
        }
    }
}
