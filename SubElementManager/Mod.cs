namespace SubElementManager {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Colossal;          

    using Game;
    using Game.Modding;

    using LucaModsCommon.Mod;

    using Newtonsoft.Json;
    using SubElementManager.L10n;

    public sealed class SEM_Mod : LucaModBase<SEM_Mod>, IMod {
        public override   string ModName       => "SubElementManager";
        public override   string Id            => "SubElementManager"; 
        protected override string UiHostPrefix => "sem"; 

        protected override ModSetting CreateSettings(IMod mod) => new SEM_Setting(mod);

        protected override IDictionarySource CreateEnUsLocalization(ModSetting settings) =>
            new SEM_LocaleEn((SEM_Setting)settings);

        protected override void RegisterSystems(UpdateSystem updateSystem) {
            updateSystem.UpdateAfter<Systems.SEM_RandomSeedFixerSystem>(SystemUpdatePhase.PostTool);
            updateSystem.UpdateAt<Systems.SEM_SubElementDeleteSystem>(SystemUpdatePhase.Modification4B);
            updateSystem.UpdateAt<Systems.SEM_ToolbarUISystem>(SystemUpdatePhase.UIUpdate);
        }

        /// <summary>
        /// Exports the en-US dictionary to L10n/lang/en-US.json. Runs only in debug builds with the
        /// EXPORT_EN_US directive (the shared I18N configuration) so translators have an up-to-date file.
        /// </summary>
        protected override void GenerateLanguageFile() {
            var entries = new SEM_LocaleEn((SEM_Setting)Settings)
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
