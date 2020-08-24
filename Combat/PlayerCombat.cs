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

        private new Player CharacterInstance => (Player) base.CharacterInstance;
        private ItemRecord PlayerWeaponRecord => Singleton.Get<Data>(this)
            .ItemData[CharacterInstance.EquippedWeapon];

        public override int GetBaseDamage(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            var damage = action switch
            {
                CombatUtil.CombatAction.Quick => weapon.QuickDamage,
                CombatUtil.CombatAction.Counter => weapon.CounterDamage,
                CombatUtil.CombatAction.Heavy => weapon.HeavyDamage,
                _ => throw new ArgumentOutOfRangeException()
            };
            damage *= 1 + CombatUtil.MULTIPLIER_PER_COMBO * HitCombo;
            return damage / 2;
        }

        protected override float GetEffectChance(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            return action switch
            {
                CombatUtil.CombatAction.Quick => weapon.QuickEffectChance,
                CombatUtil.CombatAction.Counter => weapon.CounterEffectChance,
                CombatUtil.CombatAction.Heavy => weapon.HeavyEffectChance,
                _ => 0f
            };
        }

        protected override string GetStatusEffect(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            return action switch
            {
                CombatUtil.CombatAction.Quick => weapon.QuickStatusEffect,
                CombatUtil.CombatAction.Counter => weapon.CounterStatusEffect,
                CombatUtil.CombatAction.Heavy => weapon.HeavyStatusEffect,
                _ => "None"
            };
        }

        public override string GetDamageType(CombatUtil.CombatAction action)
        {
            var weapon = PlayerWeaponRecord;
            return action switch
            {
                CombatUtil.CombatAction.Quick => weapon.QuickDamageType,
                CombatUtil.CombatAction.Counter => weapon.CounterDamageType,
                CombatUtil.CombatAction.Heavy => weapon.HeavyDamageType,
                _ => "None"
            };
        }

        protected override async Task<CombatUtil.CombatAction> GetAction()
        {
            if (new[] {"Confused", "Asleep", "Frozen"}
                .Any(key => CharacterInstance.StatusEffects.ContainsKey(key)))
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