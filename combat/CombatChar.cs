using System;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.combat.combat_actions;
using HeroesGuild.data;
using HeroesGuild.entities.base_entity;
using HeroesGuild.entities.enemies.base_enemy;
using HeroesGuild.entities.player;
using HeroesGuild.utility;

namespace HeroesGuild.combat
{
    public abstract class CombatChar : Node
    {
        protected const int MULTIPLIER_PER_COMBO = 1;
        [Signal] public delegate void DamageTaken(int damage, string damageType);

        public int hitCombo = 0;
        public BaseEntity CharacterInstance { get; set; }

        public void TakeDamage(int damage, DamageType damageType)
        {
            GD.Print($"TAKE DAMAGE: {damage} type {damageType}");
            EmitSignal(nameof(DamageTaken), damage, damageType);

            if (CharacterInstance.statusEffects.ContainsKey("Frozen") &&
                damageType == DamageType.Fire)
            {
                CharacterInstance.statusEffects.Remove("Frozen");
            }

            CharacterInstance.Health -= damage;

            if (CharacterInstance.Health <= 0)
            {
                CharacterInstance.Health = 0;
            }

            hitCombo = 0;

            PlayHurtSFX();
        }

        private void PlayHurtSFX()
        {
            switch (CharacterInstance)
            {
                case Player _:
                    AudioSystem.PlaySFX(AudioSystem.SFX.PlayerHurt, -30);
                    break;
                case BaseEnemy enemy:
                {
                    var enemyRecord =
                        Autoload.Get<Data>().enemyData[enemy.enemyName];

                    AudioSystem.SFX sfx;
                    switch (enemyRecord.Race)
                    {
                        case "Beast":
                            sfx = AudioSystem.SFX.BeastHurt;
                            break;
                        case "Demon":
                            sfx = AudioSystem.SFX.DemonHurt;
                            break;
                        case "Flora":
                            sfx = AudioSystem.SFX.FloraHurt;
                            break;
                        case "Human":
                            sfx = AudioSystem.SFX.HumanHurt;
                            break;
                        case "Robot":
                            sfx = AudioSystem.SFX.RobotHurt;
                            break;
                        case "Slime":
                            sfx = AudioSystem.SFX.SlimeHurt;
                            break;
                        default:
                            GD.PrintErr($"No SFX found for {enemyRecord.Race} race");
                            return;
                    }

                    AudioSystem.PlaySFX(sfx, -30f);
                    break;
                }
            }
        }

        public void Attack(CombatChar target, CombatAction action,
            int damage, BaseEntity instance, BaseEntity targetInstance)
        {
            if (CharacterInstance.statusEffects.ContainsKey("Weak"))
            {
                damage /= 2;
            }

            damage = Mathf.Max(damage, 1);
            var damageType = action.DamageType;
            target.TakeDamage(damage, damageType);

            var statusEffect = action.StatusEffect;
            var statusEffectChance = action.EffectChance;

            var enemy = target.CharacterInstance as BaseEnemy;
            if (enemy?.Stat.Resistance == "Fire" && statusEffect == "OnFire")
            {
                return;
            }

            if (GD.Randf() < statusEffectChance)
            {
                target.CharacterInstance.AddStatusEffect(statusEffect);
                var sfx = statusEffect switch
                {
                    "Charged" => AudioSystem.SFX.ChargedStatus,
                    "Confusion" => AudioSystem.SFX.ConfusionStatus,
                    "Frozen" => AudioSystem.SFX.FrozenStatus,
                    "Poisoned" => AudioSystem.SFX.PoisonedStatus,
                    "OnFire" => AudioSystem.SFX.OnfireStatus,
                    "Weak" => AudioSystem.SFX.WeakStatus,
                    _ => throw new ArgumentOutOfRangeException()
                };

                AudioSystem.PlaySFX(sfx, -30);
                if (enemy != null)
                {
                    GD.Print($"Applied {statusEffect} to {enemy.enemyName}");
                }
            }
        }

        public abstract Task<BaseCombatAction> GetAction();
        public abstract int GetBaseDamage(CombatAction action);
    }
}