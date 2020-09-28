using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.data;
using HeroesGuild.entities.base_entity;
using HeroesGuild.entities.enemies.base_enemy;
using HeroesGuild.entities.player;
using HeroesGuild.utility;

namespace HeroesGuild.combat
{
	public abstract class CombatChar : Node
	{
		[Signal] public delegate void DamageTaken(int damage, string damageType);

		public int hitCombo = 0;
		public BaseEntity CharacterInstance { get; set; }

		public void TakeDamage(int damage, string damageType)
		{
			GD.Print($"TAKE DAMAGE: {damage} type {damageType}");
			EmitSignal(nameof(DamageTaken), damage, damageType);

			if (CharacterInstance.statusEffects.ContainsKey("Frozen") &&
				damageType == "Fire")
			{
				CharacterInstance.statusEffects.Remove("Frozen");
			}

			CharacterInstance.Health -= damage;

			if (CharacterInstance.Health <= 0)
			{
				CharacterInstance.Health = 0;
			}

			hitCombo = 0;

			switch (CharacterInstance)
			{
				case Player _:
					AudioSystem.PlaySFX("BattleHurtPlayer");
					break;
				case BaseEnemy enemy:
					{
						var enemyRecord =
							Autoload.Get<Data>().enemyData[enemy.enemyName];

						string sfx = $"BattleHurt{enemyRecord.Race}";

						try
						{
							AudioSystem.PlaySFX(sfx);
						}
						catch (KeyNotFoundException)
						{
							GD.PrintErr($"No SFX found for {enemyRecord.Race} race");
						}

						break;
					}
			}
		}

		public void Attack(CombatChar target, CombatUtil.CombatAction action,
			int damage, BaseEntity instance,
			BaseEntity targetInstance)
		{
			if (CharacterInstance.statusEffects.ContainsKey("Weak"))
			{
				damage /= 2;
			}

			damage = Mathf.Max(damage, 1);
			var damageType = GetDamageType(action);
			target.TakeDamage(damage, damageType);

			var statusEffect = GetStatusEffect(action);
			var statusEffectChance = GetEffectChance(action);

			var enemy = target.CharacterInstance as BaseEnemy;
			if (enemy?.Stat.Resistance == "Fire" && statusEffect == "OnFire")
			{
				return;
			}

			if (GD.Randf() < statusEffectChance)
			{
				target.CharacterInstance.AddStatusEffect(statusEffect);

				var sfx = $"BattleStatus{statusEffect}";

				try
				{
					AudioSystem.PlaySFX(sfx);
				}
				catch (KeyNotFoundException)
				{
					GD.PrintErr($"No SFX found for {statusEffect} status");
				}

				if (enemy != null)
				{
					GD.Print($"Applied {statusEffect} to {enemy.enemyName}");
				}
			}
		}

		protected abstract float GetEffectChance(CombatUtil.CombatAction action);

		protected abstract string GetStatusEffect(CombatUtil.CombatAction action);

		public abstract string GetDamageType(CombatUtil.CombatAction action);

		public abstract Task<CombatUtil.CombatAction> GetAction();
		public abstract int GetBaseDamage(CombatUtil.CombatAction action);
	}
}