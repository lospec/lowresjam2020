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
    public abstract class CombatController : Node
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
                    AudioSystem.PlaySFX(AudioSystem.SFXCollection.BattleHurtPlayer);
                    break;
                case BaseEnemy enemy:
                {
                    var enemyRecord =
                        Autoload.Get<Data>().enemyData[enemy.enemyName];


                    if (AudioSystem.SFXCollection.TryGetValue(
                        $"BattleHurt{enemyRecord.Race}", out var sfx))
                    {
                        AudioSystem.PlaySFX(sfx);
                    }
                    else
                    {
                        GD.PrintErr($"No SFX found for {enemyRecord.Race} race");
                    }

                    break;
                }
            }
        }

        public void Attack(CombatController target, CombatAction action,
            int damage)
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

                if (AudioSystem.SFXCollection.TryGetValue($"BattleStatus{statusEffect}",
                    out var sfx))
                {
                    AudioSystem.PlaySFX(sfx);
                }
                else
                {
                    GD.PrintErr($"No SFX found for {statusEffect} status");
                }


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