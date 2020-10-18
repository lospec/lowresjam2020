using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct SFXCollection : IAudioCollection<SFXRecord>
    {
        private Dictionary<string, Func<SFXRecord>> _keyValue;

        [JsonProperty("BattleAttackCounter")]
        public SFXRecord BattleAttackCounter { get; set; }
        [JsonProperty("BattleAttackHeavy")]
        public SFXRecord BattleAttackHeavy { get; set; }
        [JsonProperty("BattleAttackQuick")]
        public SFXRecord BattleAttackQuick { get; set; }
        [JsonProperty("BattleHurtBeast")]
        public SFXRecord BattleHurtBeast { get; set; }
        [JsonProperty("BattleHurtDemon")]
        public SFXRecord BattleHurtDemon { get; set; }
        [JsonProperty("BattleHurtFlora")]
        public SFXRecord BattleHurtFlora { get; set; }
        [JsonProperty("BattleHurtGnome")]
        public SFXRecord BattleHurtGnome { get; set; }
        [JsonProperty("BattleHurtHuman")]
        public SFXRecord BattleHurtHuman { get; set; }
        [JsonProperty("BattleHurtPlayer")]
        public SFXRecord BattleHurtPlayer { get; set; }
        [JsonProperty("BattleHurtRobot")]
        public SFXRecord BattleHurtRobot { get; set; }
        [JsonProperty("BattleHurtSlime")]
        public SFXRecord BattleHurtSlime { get; set; }
        [JsonProperty("BattleStatusCharged")]
        public SFXRecord BattleStatusCharged { get; set; }
        [JsonProperty("BattleStatusConfusion")]
        public SFXRecord BattleStatusConfusion { get; set; }
        [JsonProperty("BattleStatusFrozen")]
        public SFXRecord BattleStatusFrozen { get; set; }
        [JsonProperty("BattleStatusOnFire")]
        public SFXRecord BattleStatusOnFire { get; set; }
        [JsonProperty("BattleStatusPoison")]
        public SFXRecord BattleStatusPoison { get; set; }
        [JsonProperty("BattleStatusWeak")]
        public SFXRecord BattleStatusWeak { get; set; }
        [JsonProperty("CharacterSelectorButtonHover")]
        public SFXRecord CharacterSelectorButtonHover { get; set; }
        [JsonProperty("CharacterSelectorButtonPressed")]
        public SFXRecord CharacterSelectorButtonPressed { get; set; }
        [JsonProperty("CharacterSelectorCharacterPressed")]
        public SFXRecord CharacterSelectorCharacterPressed { get; set; }
        [JsonProperty("GuildHallChestOpen")]
        public SFXRecord GuildHallChestOpen { get; set; }
        [JsonProperty("GuildHallEnter")]
        public SFXRecord GuildHallEnter { get; set; }
        [JsonProperty("GuildHallExit")]
        public SFXRecord GuildHallExit { get; set; }
        [JsonProperty("GuildInterfaceTabChanged")]
        public SFXRecord GuildInterfaceTabChanged { get; set; }
        [JsonProperty("InventoryItemEquipped")]
        public SFXRecord InventoryItemEquipped { get; set; }
        [JsonProperty("InventoryItemUsed")]
        public SFXRecord InventoryItemUsed { get; set; }
        [JsonProperty("InventoryItemUsedConfirmNo")]
        public SFXRecord InventoryItemUsedConfirmNo { get; set; }
        [JsonProperty("InventoryItemUsedConfirmYes")]
        public SFXRecord InventoryItemUsedConfirmYes { get; set; }
        [JsonProperty("PauseMenuInventoryExpandedItemInfoBackPressed")]
        public SFXRecord PauseMenuInventoryExpandedItemInfoBackPressed { get; set; }
        [JsonProperty("PauseMenuItemPressed")]
        public SFXRecord PauseMenuItemPressed { get; set; }
        [JsonProperty("PauseMenuTabChanged")]
        public SFXRecord PauseMenuTabChanged { get; set; }
        [JsonProperty("PlayerFootstep")]
        public SFXRecord PlayerFootstep { get; set; }
        [JsonProperty("PlayerFootstepAlt")]
        public SFXRecord PlayerFootstepAlt { get; set; }
        [JsonProperty("TitleScreenIntroNox1")]
        public SFXRecord TitleScreenIntroNox1 { get; set; }
        [JsonProperty("TitleScreenIntroNox2")]
        public SFXRecord TitleScreenIntroNox2 { get; set; }
        [JsonProperty("TitleScreenIntroNox3")]
        public SFXRecord TitleScreenIntroNox3 { get; set; }
        [JsonProperty("TitleScreenIntroPureAsbestos")]
        public SFXRecord TitleScreenIntroPureAsbestos { get; set; }
        [JsonProperty("TitleScreenIntroUnsettled")]
        public SFXRecord TitleScreenIntroUnsettled { get; set; }
        [JsonProperty("TitleScreenIntroWildleoknight")]
        public SFXRecord TitleScreenIntroWildleoknight { get; set; }
        [JsonProperty("TitleScreenKeyPressed")]
        public SFXRecord TitleScreenKeyPressed { get; set; }
        public Dictionary<string, Func<SFXRecord>> KeyValue
        {
            set => _keyValue = value;
        }

        public bool TryGetValue(string key, out SFXRecord record)
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