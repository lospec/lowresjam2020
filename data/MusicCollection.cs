using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct MusicCollection : IAudioCollection<MusicRecord>
    {
        private Dictionary<string, Func<MusicRecord>> _keyValue;
        [JsonProperty("BattleBeast")]
        public MusicRecord BattleBeast { get; set; }
        [JsonProperty("BattleDemon")]
        public MusicRecord BattleDemon { get; set; }
        [JsonProperty("BattleFlora")]
        public MusicRecord BattleFlora { get; set; }
        [JsonProperty("BattleGameOver")]
        public MusicRecord BattleGameOver { get; set; }
        [JsonProperty("BattleGnome")]
        public MusicRecord BattleGnome { get; set; }
        [JsonProperty("BattleHuman")]
        public MusicRecord BattleHuman { get; set; }
        [JsonProperty("BattleIntro")]
        public MusicRecord BattleIntro { get; set; }
        [JsonProperty("BattleRobot")]
        public MusicRecord BattleRobot { get; set; }
        [JsonProperty("BattleSlime")]
        public MusicRecord BattleSlime { get; set; }
        [JsonProperty("Overworld")]
        public MusicRecord Overworld { get; set; }
        [JsonProperty("TitleScreen")]
        public MusicRecord TitleScreen { get; set; }
        [JsonProperty("BattleVictoryJingle")]
        public MusicRecord BattleVictoryJingle { get; set; }
        public Dictionary<string, Func<MusicRecord>> KeyValue
        {
            set => _keyValue = value;
        }

        public bool TryGetValue(string key, out MusicRecord record)
        {
            record = default;
            if (_keyValue.TryGetValue(key, out var getter))
            {
                record = getter.Invoke();
                return true;
            }

            return false;
        }
    }
}