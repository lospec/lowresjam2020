using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.utility
{
    public static class SaveManager
    {
        private const string SaveDataPath = "user://heroes_guild.save";
        public static SaveData SaveData { get; private set; } = new SaveData();

        private static readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                MissingMemberHandling = MissingMemberHandling.Error
            };

        static SaveManager()
        {
            LoadGame();
        }

        private static string ReadSaveFile(out bool result)
        {
            result = false;
            var file = new File();
            if (!file.FileExists(SaveDataPath))
            {
                GD.PrintErr("Attempted to load game data but no save data file found");
                return string.Empty;
            }

            file.Open(SaveDataPath, File.ModeFlags.Read);
            var saveDataText = file.GetAsText();
            file.Close();
            return saveDataText;
        }

        public static void LoadGame()
        {
            var saveDataText = ReadSaveFile(out var result);
            if (!result)
            {
                return;
            }

            LoadGame(saveDataText);
        }

        private static void LoadGame(string saveDataText)
        {
            SaveData data;
            try
            {
                data = JsonConvert.DeserializeObject<SaveData>(saveDataText,
                    JsonSerializerSettings);
            }
            catch (JsonReaderException ex)
            {
                GD.PrintErr(
                    "Failed to read Save File - Save file is most likely not in the correct JSON format");
                GD.PrintErr($"Caught the following exception:\n{ex}");
                GD.Print("Will now load default save data");
                SaveData = new SaveData();
                return;
            }
            catch (JsonSerializationException ex)
            {
                GD.PrintErr(
                    "Failed to Deserialize Save File - Save file may be for an unsupported version of the game");
                GD.PrintErr($"Caught the following exception:\n{ex}");
                GD.Print("Will now load default save data");
                SaveData = new SaveData();
                return;
            }

            if (data == null)
            {
                GD.Print("Save file is corrupted");
                GD.PrintErr("Failed to Deserialize Save File");
                return;
            }

            SaveData = data;
        }


        public static void SaveGame()
        {
            var file = new File();
            file.Open(SaveDataPath, File.ModeFlags.Write);
            GD.Print($"Save to {file.GetPathAbsolute()}");
            file.StoreString(JsonConvert.SerializeObject(SaveData, Formatting.Indented,
                JsonSerializerSettings));
            file.Close();
        }

        public static void ResetSave()
        {
            SaveData = new SaveData();
            SaveGame();
        }
    }
}