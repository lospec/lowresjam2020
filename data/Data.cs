using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public class Data : Node
    {
        private const string ITEM_DATA_PATH = "res://data/item_data.json";
        private const string ENEMY_DATA_PATH = "res://data/enemy_data.json";
        private const string CHARACTER_DATA_PATH = "res://data/character_data.json";
        private const string MUSIC_DATA_PATH = "res://data/music_data.json";
        private const string SFX_DATA_PATH = "res://data/sfx_data.json";
        private int _maxSpeedStat = int.MinValue;

        private int _minSpeedStat = int.MaxValue;
        private bool _speedStatRead = false;
        public Dictionary<string, CharacterRecord> characterData =
            new Dictionary<string, CharacterRecord>();
        public Dictionary<string, EnemyRecord> enemyData =
            new Dictionary<string, EnemyRecord>();

        public Dictionary<string, ItemRecord> itemData =
            new Dictionary<string, ItemRecord>();

        public Dictionary<string, MusicRecord> musicData =
            new Dictionary<string, MusicRecord>();

        public SFXCollection sfxCollection;
        public MusicCollection musicCollection;

        public override void _Ready()
        {
            itemData = ParseData<ItemRecord>(ITEM_DATA_PATH);
            enemyData = ParseData<EnemyRecord>(ENEMY_DATA_PATH);
            characterData = ParseData<CharacterRecord>(CHARACTER_DATA_PATH);
            musicData = ParseData<MusicRecord>(MUSIC_DATA_PATH);
            sfxCollection = ParseCollection<SFXCollection, SFXRecord>(SFX_DATA_PATH);
            musicCollection =
                ParseCollection<MusicCollection, MusicRecord>(MUSIC_DATA_PATH);

            SetMinMaxSpeedStat();
        }

        public float GetLerpedSpeedStat(int speedStat, float minSpeed, float maxSpeed)
        {
            if (!_speedStatRead) SetMinMaxSpeedStat();

            return Mathf.Lerp(minSpeed, maxSpeed,
                (float) (speedStat - _minSpeedStat) / (_maxSpeedStat - _minSpeedStat));
        }

        private void SetMinMaxSpeedStat()
        {
            foreach (var speed in enemyData.Values.Select(enemyRecord =>
                enemyRecord.MoveSpeed))
            {
                if (speed < _minSpeedStat) _minSpeedStat = speed;

                if (speed > _maxSpeedStat) _maxSpeedStat = speed;

                _speedStatRead = true;
            }
        }

        private static T ParseCollection<T, TRecord>(string path)
            where T : IAudioCollection<TRecord> where TRecord : IDataRecord
        {
            var file = new File();
            if (!file.FileExists(path))
            {
                GD.PushError($"{path} file not found");
            }

            file.Open(path, File.ModeFlags.Read);
            var json = file.GetAsText();
            var data = JsonConvert.DeserializeObject<T>(json);

            var dict = new Dictionary<string, Func<TRecord>>();

            foreach (var propertyInfo in data.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public |
                               BindingFlags.DeclaredOnly))
            {
                var attrs = propertyInfo.GetCustomAttributes(true);
                string name = null;
                foreach (var attr in attrs)
                {
                    if (attr is JsonPropertyAttribute jsonAttr)
                    {
                        name = jsonAttr.PropertyName;
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                try
                {
                    TRecord Value() => (TRecord) propertyInfo.GetValue(data);
                    dict.Add(name, Value);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            data.KeyValue = dict;
            return data;
        }

        private static Dictionary<string, T> ParseData<T>(string path)
            where T : IDataRecord
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