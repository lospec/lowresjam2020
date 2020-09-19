using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using File = Godot.File;


namespace HeroesGuild.data
{
    public class Data : Node
    {
        private const string ITEM_DATA_PATH = "res://data/item_data.json";
        private const string ENEMY_DATA_PATH = "res://data/enemy_data.json";
        private const string CHARACTER_DATA_PATH = "res://data/character_data.json";

        public Dictionary<string, ItemRecord> itemData = new Dictionary<string, ItemRecord>();
        public Dictionary<string, EnemyRecord> enemyData = new Dictionary<string, EnemyRecord>();
        public Dictionary<string, CharacterRecord> characterData = new Dictionary<string, CharacterRecord>();

        private int _minSpeedStat = int.MaxValue;
        private int _maxSpeedStat = int.MinValue;
        private bool _speedStatRead = false;

        public override void _Ready()
        {
            itemData = ParseData<ItemRecord>(ITEM_DATA_PATH);
            enemyData = ParseData<EnemyRecord>(ENEMY_DATA_PATH);
            characterData = ParseData<CharacterRecord>(CHARACTER_DATA_PATH);
            SetMinMaxSpeedStat();
        }

        public float GetLerpedSpeedStat(int speedStat, float minSpeed, float maxSpeed)
        {
            if (!_speedStatRead)
            {
                SetMinMaxSpeedStat();
            }

            return Mathf.Lerp(minSpeed, maxSpeed,
                (float) (speedStat - _minSpeedStat) / (_maxSpeedStat - _minSpeedStat));
        }

        private void SetMinMaxSpeedStat()
        {
            foreach (var speed in enemyData.Values.Select(enemyRecord => enemyRecord.MoveSpeed))
            {
                if (speed < _minSpeedStat)
                {
                    _minSpeedStat = speed;
                }

                if (speed > _maxSpeedStat)
                {
                    _maxSpeedStat = speed;
                }

                _speedStatRead = true;
            }
        }

        private Dictionary<string, T> ParseData<T>(string path) where T : IDataRecord
        {
            var file = new File();
            if (!file.FileExists(path))
            {
                GD.PushError($"{path} file not found");
                return null;
            }

            file.Open(path, File.ModeFlags.Read);
            var json = file.GetAsText();
            var data = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            return data;
        }
    }
}