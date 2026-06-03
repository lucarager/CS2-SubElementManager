namespace SubElementManager {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Colossal;          // IDictionarySource, IDictionaryEntryError
    using Colossal.Logging;  // ILog

    using Game;
    using Game.Modding;

    using LucaModsCommon.Mod;

    using Newtonsoft.Json;

    public sealed class Mod : LucaModBase<Mod>, IMod {
        public override   string ModName       => "SubElementManager";
        public override   string Id            => "SubElementManager"; // binding group; must match UI/mod.json "id"
        protected override string UiHostPrefix => "sem"; // AddHostLocation prefix for coui:// asset URLs

        protected override ModSetting CreateSettings(IMod mod) => new Setting(mod);

        protected override IDictionarySource CreateEnUsLocalization(ModSetting settings) =>
            new LocaleEN((Setting)settings);

        protected override void RegisterSystems(UpdateSystem updateSystem) {
            //updateSystem.UpdateAt<Systems.SubElementManagerSystem>(SystemUpdatePhase.UIUpdate);
            //updateSystem.UpdateAt<Systems.SubElementManagerUISystem>(SystemUpdatePhase.UIUpdate);
        }

        /// <summary>
        /// Exports the en-US dictionary to L10n/lang/en-US.json. Runs only in debug builds with the
        /// EXPORT_EN_US directive (the shared I18N configuration) so translators have an up-to-date file.
        /// </summary>
        protected override void GenerateLanguageFile() {
            var entries = new LocaleEN((Setting)Settings)
                          .ReadEntries(new List<IDictionaryEntryError>(), new Dictionary<string, int>())
                          .ToDictionary(pair => pair.Key, pair => pair.Value);
            var json = JsonConvert.SerializeObject(entries, Formatting.Indented);
            try {
                var dir = Path.GetDirectoryName(GetThisFilePath());
                File.WriteAllText($@"{dir}/L10n/lang/en-US.json", json);
            } catch (Exception ex) {
                Log.Error(ex.ToString());
            }
        }

        private static string GetThisFilePath([CallerFilePath] string path = null) => path;
    }
}
