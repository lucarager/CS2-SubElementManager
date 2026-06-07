namespace SubElementManager {
    using System.Collections.Generic;

    using Colossal;
    using Colossal.IO.AssetDatabase;

    using Game.Input;
    using Game.Modding;
    using Game.Settings;
    using Game.UI;
    using Game.UI.Widgets;

    [FileLocation(nameof(SubElementManager))]
    public class SEM_Setting : ModSetting {
        public SEM_Setting(IMod mod) : base(mod) {

        }

        public override void SetDefaults() {
        }

    }
}
