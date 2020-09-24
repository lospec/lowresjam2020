using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.Entities.BaseEntity;
using HeroesGuild.Entities.enemies.base_enemy;
using HeroesGuild.Entities.Player;
using HeroesGuild.Utility;

namespace HeroesGuild.Combat
{
    public class Combat : CanvasLayer
    {
        private const string WeaponTexturePath = "res://combat/weapon_sprites/{0}.png";

        [Signal] private delegate void CombatDone(CombatUtil.CombatOutcome outcome,
            BaseEnemy enemyInstance);

        [Signal] private delegate void BagOpened(Player playerInstance);

        private static readonly Texture[] Backgrounds =
        {
            ResourceLoader.Load<Texture>("res://combat/scenic_backgrounds/rocks.png"),
            ResourceLoader.Load<Texture>("res://combat/scenic_backgrounds/beach.png"),
            ResourceLoader.Load<Texture>("res://combat/scenic_backgrounds/forest.png"),
            ResourceLoader.Load<Texture>("res://combat/scenic_backgrounds/lakes.png"),
            ResourceLoader.Load<Texture>(
                "res://combat/scenic_backgrounds/mountain_little.png"),
            ResourceLoader.Load<Texture>("res://combat/scenic_backgrounds/path.png"),
            ResourceLoader.Load<Texture>("res://combat/scenic_backgrounds/plain.png"),
            ResourceLoader.Load<Texture>(
                "res://combat/scenic_backgrounds/plateau_small.png"),
        };


        private Player _playerInstance;
        private BaseEnemy _enemyInstance;

        private CombatMenu _combatMenu;
        private PlayerCombat _playerCombat;
        private EnemyCombat _enemyCombat;

        public override void _Ready()
        {
            _combatMenu = GetNode<CombatMenu>("CombatMenu");
            _playerCombat = GetNode<PlayerCombat>("PlayerCombat");
            _enemyCombat = GetNode<EnemyCombat>("EnemyCombat");
        }

        private void SetupCombat(Player player, BaseEnemy enemy)
        {
            GetNode<TextureRect>("Background").Texture = Backgrounds.RandomElement();
            AudioSystem.StopMusic();
            _playerCombat.CharacterInstance = player;
            _enemyCombat.CharacterInstance = enemy;

            _combatMenu.currentMenu = CombatMenu.Menu.Main;
            _combatMenu.ResetUI();

            _playerInstance = player;
            _enemyInstance = enemy;

            if (!_playerInstance.IsConnected(nameof(BaseEntity.HealthChanged),
                _combatMenu,
                nameof(_combatMenu.UpdatePlayerHealthValue)))
            {
                _playerInstance.Connect(nameof(BaseEntity.HealthChanged), _combatMenu,
                    nameof(_combatMenu.UpdatePlayerHealthValue));
            }

            if (!_enemyInstance.IsConnected(nameof(BaseEntity.HealthChanged),
                _combatMenu,
                nameof(_combatMenu.UpdateEnemyHealthValue)))
            {
                _enemyInstance.Connect(nameof(BaseEntity.HealthChanged), _combatMenu,
                    nameof(_combatMenu.UpdateEnemyHealthValue));
            }

            if (!_enemyCombat.IsConnected(nameof(CombatChar.DamageTaken), this,
                nameof(OnEnemy_TakeDamage)))
            {
                _enemyCombat.Connect(nameof(CombatChar.DamageTaken), this,
                    nameof(OnEnemy_TakeDamage));
            }

            _combatMenu.SetPlayerHealthValue(_playerInstance.maxHealth,
                _playerInstance.Health);
            _combatMenu.SetEnemyHealthValue(_enemyInstance.maxHealth,
                _enemyInstance.Health);

            var weaponName = _playerInstance.EquippedWeapon;
            var weaponTexture =
                GD.Load<Texture>(string.Format(WeaponTexturePath,
                    weaponName.ToLower()));
            if (weaponTexture == null)
            {
                GD.PushWarning($"Weapon Battle sprite for {weaponName} not found");
            }

            _combatMenu.playerWeapon.Texture = weaponTexture;
            ((AtlasTexture) _combatMenu.enemyImage.Texture).Atlas =
                _enemyInstance.battleTexture;
            ((AtlasTexture) _combatMenu.enemyImage.Texture).Region = new Rect2(
                CombatAnimationUtil.AnimationStateRegionPositionX[
                    CombatAnimationUtil.AnimationState.Normal],
                CombatAnimationUtil.BattleTexturePosY,
                CombatAnimationUtil.BattleTextureWidth,
                CombatAnimationUtil.BattleTextureHeight);
            StartCombat();
        }

        private async void StartCombat()
        {
            var sfxPlayer = AudioSystem.PlaySFX(AudioSystem.SFX.BattleIntro, null, -20);
            sfxPlayer.Connect("finished", this, nameof(PlayBattleMusic));
            var combat = true;
            while (combat)
            {
                _combatMenu.UpdateParticle(_enemyInstance);
                combat = await TakeTurn();
                if (combat)
                {
                    foreach (var statusEffectsKey in _playerInstance.statusEffects.Keys)
                    {
                        var statusEffect =
                            _playerInstance.statusEffects[statusEffectsKey];
                        statusEffect.OnTurnEnd(_playerCombat);
                        if (statusEffect.expired)
                        {
                            _playerInstance.statusEffects.Remove(statusEffectsKey);
                        }
                    }

                    foreach (var statusEffectsKey in _enemyInstance.statusEffects.Keys)
                    {
                        var statusEffect =
                            _enemyInstance.statusEffects[statusEffectsKey];
                        statusEffect.OnTurnEnd(_enemyCombat);
                        if (statusEffect.expired)
                        {
                            _enemyInstance.statusEffects.Remove(statusEffectsKey);
                        }
                    }

                    if (CheckCombatEnd())
                    {
                        if (_playerInstance.Health <= 0)
                        {
                            await _combatMenu.ShowCombatLabel("YOU DIED", 2);
                            await _combatMenu.ShowCombatLabel("GAME OVER", 2);
                            _combatMenu.combatLabel.Visible = true;
                            EndCombat(CombatUtil.CombatOutcome.CombatLose);
                        }
                        else if (_enemyInstance.Health <= 0)
                        {
                            await _combatMenu.ShowCombatLabel("YOU WON", 2);
                            await _combatMenu.ShowCombatLabel("CONGRATULATION", 2);
                            _combatMenu.combatLabel.Visible = true;
                            EndCombat(CombatUtil.CombatOutcome.CombatWin);
                        }

                        combat = false;
                    }
                }

                if (combat)
                {
                    _combatMenu.ResetUI();
                }
            }
        }

        private void EndCombat(CombatUtil.CombatOutcome outcome)
        {
            _playerInstance.Disconnect(nameof(BaseEntity.HealthChanged), _combatMenu,
                nameof(_combatMenu.UpdatePlayerHealthValue));
            _enemyInstance.Disconnect(nameof(BaseEntity.HealthChanged), _combatMenu,
                nameof(_combatMenu.UpdateEnemyHealthValue));
            EmitSignal(nameof(CombatDone), outcome, _enemyInstance);
        }


        private bool CheckCombatEnd()
        {
            return _playerInstance.Health <= 0 || _enemyInstance.Health <= 0;
        }

        private async Task<bool> TakeTurn()
        {
            var playerAction = await _playerCombat.GetAction();
            var enemyAction = await _enemyCombat.GetAction();
            _combatMenu.SetButtonsVisible(false);
            await _combatMenu.ShowTurnResult(playerAction, enemyAction);
            var timer = GetTree().CreateTimer(1.5f);
            if (playerAction == CombatUtil.CombatAction.Flee)
            {
                var flee = await PlayerFlee(enemyAction);
                if (flee)
                {
                    _combatMenu.combatLabel.Visible = true;
                    EndCombat(CombatUtil.CombatOutcome.CombatFlee);
                    return false;
                }
            }
            else
            {
                var win = CombatUtil.ActionCompare(playerAction, enemyAction);
                switch (win)
                {
                    case CombatUtil.TurnOutcome.Tie:
                        await Tie(playerAction);
                        break;
                    case CombatUtil.TurnOutcome.PlayerWin:
                        await PlayerWin(playerAction);
                        break;
                    case CombatUtil.TurnOutcome.EnemyWin:
                        await EnemyWin(enemyAction);
                        break;
                    default:
                        await _combatMenu.ShowCombatLabel("ERROR: Invalid win check",
                            2);
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (timer.TimeLeft > 0)
            {
                await ToSignal(timer, "timeout");
            }

            _combatMenu.HideTurnResult();
            if (CheckCombatEnd())
            {
                if (_playerInstance.Health <= 0)
                {
                    await _combatMenu.ShowCombatLabel("YOU DIED", 2);
                    await _combatMenu.ShowCombatLabel("GAME OVER", 2);
                    _combatMenu.combatLabel.Visible = true;
                    EndCombat(CombatUtil.CombatOutcome.CombatLose);
                }
                else if (_enemyInstance.Health <= 0)
                {
                    await _combatMenu.ShowCombatLabel("YOU WON", 2);
                    await _combatMenu.ShowCombatLabel("CONGRATULATION", 2);
                    _combatMenu.Visible = true;
                    EndCombat(CombatUtil.CombatOutcome.CombatWin);
                }

                return false;
            }

            return true;
        }

        private async Task EnemyWin(CombatUtil.CombatAction enemyAction)
        {
            var enemyDamage = _enemyCombat.GetBaseDamage(enemyAction);
            switch (enemyAction)
            {
                case CombatUtil.CombatAction.Quick:
                    _enemyCombat.Attack(_playerCombat, enemyAction, enemyDamage,
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage);
                    break;
                case CombatUtil.CombatAction.Counter:
                    enemyDamage /= 2;
                    _enemyCombat.Attack(_playerCombat, enemyAction, enemyDamage,
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage, true);
                    break;
                case CombatUtil.CombatAction.Heavy:
                    _enemyCombat.Attack(_playerCombat, enemyAction, enemyDamage,
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage);
                    break;
                default:
                    _combatMenu.HideTurnResult();
                    await _combatMenu.ShowCombatLabel(
                        "ERROR. Unknown Action on EnemyWin()", 2);
                    throw new ArgumentOutOfRangeException(nameof(enemyAction),
                        enemyAction, null);
            }
        }

        private async Task PlayerWin(CombatUtil.CombatAction playerAction)
        {
            var playerDamage = _playerCombat.GetBaseDamage(playerAction);
            _playerCombat.hitCombo += 1;
            switch (playerAction)
            {
                case CombatUtil.CombatAction.Quick:
                    await _combatMenu.AnimatePlayerAttack(_playerCombat, playerAction);
                    _playerCombat.Attack(_enemyCombat, playerAction, playerDamage,
                        _playerInstance, _enemyInstance);
                    break;
                case CombatUtil.CombatAction.Counter:
                    await _combatMenu.AnimatePlayerAttack(_playerCombat, playerAction);
                    _playerCombat.Attack(_enemyCombat, playerAction, playerDamage,
                        _playerInstance, _enemyInstance);
                    break;
                case CombatUtil.CombatAction.Heavy:
                    await _combatMenu.AnimatePlayerAttack(_playerCombat, playerAction);
                    _playerCombat.Attack(_enemyCombat, playerAction, playerDamage,
                        _playerInstance, _enemyInstance);
                    break;
                default:
                    _combatMenu.HideTurnResult();
                    await _combatMenu.ShowCombatLabel(
                        "ERROR. Unknown Action on PlayerWin()", 2);
                    throw new ArgumentOutOfRangeException(nameof(playerAction),
                        playerAction, null);
            }
        }

        private async Task Tie(CombatUtil.CombatAction action)
        {
            var enemyDamage = _enemyCombat.GetBaseDamage(action);
            var playerDamage = _playerCombat.GetBaseDamage(action);
            switch (action)
            {
                case CombatUtil.CombatAction.Quick:
                    await _combatMenu.AnimatePlayerAttack(_playerCombat, action);
                    _playerCombat.Attack(_enemyCombat, action, playerDamage,
                        _playerInstance, _enemyInstance);
                    _enemyCombat.Attack(_playerCombat, action, enemyDamage,
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage);
                    break;
                case CombatUtil.CombatAction.Counter:
                    await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
                    break;
                case CombatUtil.CombatAction.Heavy:
                    await _combatMenu.AnimatePlayerAttack(_playerCombat, action);
                    playerDamage /= 2;
                    _playerCombat.Attack(_enemyCombat, action, playerDamage,
                        _playerInstance, _enemyInstance);
                    enemyDamage /= 2;
                    _enemyCombat.Attack(_playerCombat, action, enemyDamage,
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage);
                    break;
                default:
                    _combatMenu.HideTurnResult();
                    await _combatMenu.ShowCombatLabel("ERROR. Unknown Action on Tie()",
                        2);
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private async Task<bool> PlayerFlee(CombatUtil.CombatAction enemyAction)
        {
            var rule = new CombatUtil.FleeRule(enemyAction);
            var enemyDamage = _enemyCombat.GetBaseDamage(enemyAction);
            var outcome = rule.Roll();
            switch (outcome)
            {
                case CombatUtil.FleeRule.FleeOutcome.Success:
                    await _combatMenu.ShowCombatLabel("Got away safely", 2);
                    return true;
                case CombatUtil.FleeRule.FleeOutcome.SuccessDmg:
                    _enemyCombat.Attack(_playerCombat, enemyAction,
                        (int) (enemyDamage * rule.damageModifier),
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage);
                    await _combatMenu.ShowCombatLabel("Got away not so safely", 2);
                    return true;
                case CombatUtil.FleeRule.FleeOutcome.Fail:
                    await _combatMenu.ShowCombatLabel("Failed to flee", 2);
                    _enemyCombat.Attack(_playerCombat, enemyAction,
                        (int) (enemyDamage * rule.damageModifier),
                        _enemyInstance, _playerInstance);
                    await _combatMenu.AnimatePlayerHurt(enemyDamage);
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PlayBattleMusic()
        {
            var enemyRace = _enemyInstance.Stat.Race;
            var enemyRaceMusic = new Dictionary<string, AudioSystem.Music>
            {
                {"Robot", AudioSystem.Music.BattleRobot},
                {"Beast", AudioSystem.Music.BattleBeast},
                {"Demon", AudioSystem.Music.BattleDemon},
                {"Human", AudioSystem.Music.BattleHuman},
                {"Gnome", AudioSystem.Music.BattleGnome},
                {"Flora", AudioSystem.Music.BattleFlora},
                {"Slime", AudioSystem.Music.BattleSlime}
            };
            AudioSystem.PlayMusic(enemyRaceMusic[enemyRace], -25);
        }

        private void OnEnemy_TakeDamage(int damage, string damageType)
        {
            _combatMenu.UpdateParticle(_enemyInstance);
            _combatMenu.AnimateEnemyHurt(_enemyInstance, damage);
        }

        private void OnPlayer_EnemyDetected(Player player, BaseEnemy enemy)
        {
            GetTree().Paused = true;
            player.hudMargin.Visible = false;
            SetupCombat(player, enemy);
            _combatMenu.Visible = true;
        }

        private void OnCounter_Pressed()
        {
        }

        private void OnQuick_Pressed()
        {
        }

        private void OnHeavy_Pressed()
        {
        }

        private void OnFlee_Pressed()
        {
        }

        private void OnCombatMenu_BagOpened()
        {
            EmitSignal(nameof(BagOpened), _playerInstance);
        }

        public override void _Process(float delta)
        {
            GetNode<CanvasItem>("Background").Visible = _combatMenu.Visible;
        }
    }
}