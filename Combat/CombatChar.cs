using System;
using Godot;
using HeroesGuild.data;
using HeroesGuild.Entities.BaseEntity;
using HeroesGuild.Entities.enemies.base_enemy;
using HeroesGuild.Entities.Player;
using HeroesGuild.Utility;

namespace HeroesGuild.Combat
{
    public abstract class CombatChar : Node
    {
        [Signal] public delegate void DamageTaken(int damage, string damageType);

        public int HitCombo = 0;
        public BaseEntity characterInstance;

        public void TakeDamage(int damage, string damageType)
        {
            GD.Print($"TAKE DAMAGE: {damage} type {damageType}");
            EmitSignal(nameof(DamageTaken), damage, damageType);

            if (characterInstance.StatusEffects.ContainsKey("Frozen") && damageType == "Fire")
            {
                characterInstance.StatusEffects.Remove("Frozen");
            }

            characterInstance.Health -= damage;

            if (characterInstance.Health <= 0)
            {
                characterInstance.Health = 0;
            }

            HitCombo = 0;

            switch (characterInstance)
            {
                case Player _:
                    AudioSystem.PlaySFX(AudioSystem.SFX.PlayerHurt, null, -30);
                    break;
                case BaseEnemy enemy:
                {
                    var enemyRecord = Singleton.Get<Data>(this).EnemyData[enemy.EnemyName];
                    var sfx = enemyRecord.Race switch
                    {
                        "Beast" => AudioSystem.SFX.BeastHurt,
                        "Demon" => AudioSystem.SFX.DemonHurt,
                        "Flora" => AudioSystem.SFX.FloraHurt,
                        "Human" => AudioSystem.SFX.HumanHurt,
                        "Robot" => AudioSystem.SFX.RobotHurt,
                        "Slime" => AudioSystem.SFX.SlimeHurt,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    AudioSystem.PlaySFX(sfx, null, -30f);
                    break;
                }
            }
        }

        public void Attack(CombatChar target, int action, int damage, BaseEntity instance, BaseEntity targetInstance)
        {
            if (characterInstance.StatusEffects.ContainsKey("Weak"))
            {
                damage /= 2;
            }

            damage = Mathf.Max(damage, 1);
            var damageType = GetDamageType(action);
            target.TakeDamage(damage, damageType);

            var statusEffect = GetStatusEffect(action);
            var statusEffectChance = GetEffectChance(action);

            var enemy = characterInstance as BaseEnemy;
            if (enemy?.Stat.Resistance == "Fire" && statusEffect == "OnFire")
            {
                return;
            }

            if (GD.Randf() < statusEffectChance)
            {
                target.characterInstance.AddStatusEffect(statusEffect);
                AudioSystem.SFX sfx = statusEffect switch
                {
                    "Charged" => AudioSystem.SFX.ChargedStatus,
                    "Confusion" => AudioSystem.SFX.ConfusionStatus,
                    "Frozen" => AudioSystem.SFX.FrozenStatus,
                    "Poisoned" => AudioSystem.SFX.PoisonedStatus,
                    "OnFire" => AudioSystem.SFX.OnfireStatus,
                    "Weak" => AudioSystem.SFX.WeakStatus,
                    _ => throw new ArgumentOutOfRangeException()
                };

                AudioSystem.PlaySFX(sfx, null, -30);
                if (enemy != null)
                {
                    GD.Print($"Applied {statusEffect} to {enemy.EnemyName}");
                }
            }
        }

        protected abstract float GetEffectChance(int action);

        protected abstract string GetStatusEffect(int action);

        protected abstract string GetDamageType(int action);

        protected abstract int GetAction();
    }
}