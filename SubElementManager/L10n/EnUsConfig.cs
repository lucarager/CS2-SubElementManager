namespace SubElementManager.L10n {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Colossal;
    using Colossal.IO.AssetDatabase.Internal;

    public class SEM_LocaleEn : IDictionarySource {
        private readonly SEM_Setting m_Setting;

        public SEM_LocaleEn(SEM_Setting setting) {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "SEM" },
                { "SubElementManager.UI.ToolOptions.Title", "SEM" },
                { "SubElementManager.UI.ToolOptions.FixedRandomSeed", "Disable Asset Randomness" },
                { "SubElementManager.UI.ToolOptions.Decorations", "Remove Decorations" },
                { "SubElementManager.UI.ToolOptions.Markings", "Remove Markings" },
                { "SubElementManager.UI.ToolOptions.Surface", "Remove Surfaces" },
                { "SubElementManager.UI.ToolOptions.Vegetation", "Remove Vegetation" },
                { "SubElementManager.UI.ToolOptions.Elements", "Remove Elements" },
                { "SubElementManager.UI.ToolOptions.NoBoundaryFence", "No Fence" },
                { "SubElementManager.UI.ToolOptions.NoBoundaryHedge", "No Hedge" },
                { "SubElementManager.UI.ToolOptions.NoMarkingLane", "No Lane Markings" },
                { "SubElementManager.UI.ToolOptions.NoSurfaceGrass", "No Grass" },
                { "SubElementManager.UI.ToolOptions.NoSurfacePavement", "No Pavement" },
                { "SubElementManager.UI.ToolOptions.NoVegetationTree", "No Trees" },
                { "SubElementManager.UI.ToolOptions.NoVegetationShrub", "No Shrubs" },
                { "SubElementManager.UI.ToolOptions.NoElementParking", "No Parking" },
                { "SubElementManager.UI.ToolOptions.NoElementLights", "No Lights" },
                { "SubElementManager.UI.ToolOptions.ToggleAll", "Toggle All" }
            };
        }

        public void Unload() {

        }
    }
}
