using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.data;
using HeroesGuild.Entities.Player;
using HeroesGuild.Utility;

namespace HeroesGuild.Combat
{
    public class PlayerCombat : CombatChar
    {
        [Signal] public delegate void ActionChosen(CombatUtil.CombatAction action);

        private new Player CharacterInstance => (Player) base.characterInstance;
        private ItemRecord PlayerWeaponRecord => Autoload.Get<Data>()
            .itemData[CharacterInstance.EquippedWeapon];

        public override int GetBaseDamage(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            var damage = action switch
            {
                CombatUtil.CombatAction.Quick => weapon.quickDamage,
                CombatUtil.CombatAction.Counter => weapon.counterDamage,
                CombatUtil.CombatAction.Heavy => weapon.heavyDamage,
                _ => throw new ArgumentOutOfRangeException()
            };
            damage *= 1 + CombatUtil.MULTIPLIER_PER_COMBO * hitCombo;
            return damage / 2;
        }

        protected override float GetEffectChance(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            return action switch
            {
                CombatUtil.CombatAction.Quick => weapon.quickEffectChance,
                CombatUtil.CombatAction.Counter => weapon.counterEffectChance,
                CombatUtil.CombatAction.Heavy => weapon.heavyEffectChance,
                _ => 0f
            };
        }

        protected override string GetStatusEffect(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            return action switch
            {
                CombatUtil.CombatAction.Quick => weapon.quickStatusEffect,
                CombatUtil.CombatAction.Counter => weapon.counterStatusEffect,
                CombatUtil.CombatAction.Heavy => weapon.heavyStatusEffect,
                _ => "None"
            };
        }

        public override string GetDamageType(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            return action switch
            {
                CombatUtil.CombatAction.Quick => weapon.quickDamageType,
                CombatUtil.CombatAction.Counter => weapon.counterDamageType,
                CombatUtil.CombatAction.Heavy => weapon.heavyDamageType,
                _ => "None"
            };
        }

        public override async Task<CombatUtil.CombatAction> GetAction()
        {
            if (new[] {"Confused", "Asleep", "Frozen"}
                .Any(key => CharacterInstance.statusEffects.ContainsKey(key)))
            {
                return CombatUtil.CombatAction.None;
            }

            var action = await ToSignal(this, nameof(ActionChosen));
            return (CombatUtil.CombatAction) action.Single();
        }

        private void OnCombatMenu_ActionSelected(CombatUtil.CombatAction action)
        {
            EmitSignal(nameof(ActionChosen), action);
        }
    }
}