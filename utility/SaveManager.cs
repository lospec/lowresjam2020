using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.utility
{
    public static class SaveManager
    {
        private const string SaveDataPath = "user://heroes_guild.save";
        public static SaveData SaveData { get; private set; } = new SaveData();

        static SaveManager()
        {
            LoadGame();
        }

        public static void LoadGame()
        {
            var file = new File();
            if (!file.FileExists(SaveDataPath))
            {
                GD.PrintErr("Attempted to load game data but no save data file found");
                return;
            }

            file.Open(SaveDataPath, File.ModeFlags.Read);
            SaveData = JsonConvert.DeserializeObject<SaveData>(file.GetAsText());
            file.Close();
        }

        public static void SaveGame()
        {
            var file = new File();
            file.Open(SaveDataPath, File.ModeFlags.Write);
            GD.Print($"Save to {file.GetPathAbsolute()}");
            file.StoreString(JsonConvert.SerializeObject(SaveData, Formatting.Indented));
            file.Close();
        }

        public static void ResetSave()
        {
            SaveData = new SaveData();
            SaveGame();
        }
    }
}