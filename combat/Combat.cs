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
    public class Combat : CanvasLayer
    {
        private const string WeaponTexturePath = "res://combat/weapon_sprites/{0}.png";

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
                "res://combat/scenic_backgrounds/plateau_small.png")
        };

        private CombatMenu _combatMenu;

        private BaseEnemy _enemyInstance;
        private Player _playerInstance;

        private EnemyCombat _enemyCombat;
        private PlayerCombat _playerCombat;


        public override void _Ready()
        {
            base._Ready();
            _combatMenu = GetNode<CombatMenu>("CombatMenu");
            _playerCombat = GetNode<PlayerCombat>("PlayerCombat");
            _enemyCombat = GetNode<EnemyCombat>("EnemyCombat");
            _playerCombat.OnDependency(_combatMenu);
        }

        private void SetupCombat(Player player, BaseEnemy enemy)
        {
            GetNode<TextureRect>("Background").Texture = Backgrounds.RandomElement();
            AudioSystem.StopAllMusic();
            _playerCombat.CharacterInstance = player;
            _enemyCombat.CharacterInstance = enemy;

            _combatMenu.currentMenu = CombatMenu.Menu.Main;
            _combatMenu.ResetUI();

            _playerInstance = player;
            _enemyInstance = enemy;

            ConnectCombatSignals();

            _combatMenu.SetPlayerHealthValue(_playerInstance.maxHealth,
                _playerInstance.Health);
            _combatMenu.SetEnemyHealthValue(_enemyInstance.maxHealth,
                _enemyInstance.Health);

            LoadTextureResources();
            StartCombatLoop();
        }

        private void LoadTextureResources()
        {
            var weaponName = _playerInstance.EquippedWeapon;
            var weaponTexture =
                GD.Load<Texture>(string.Format(WeaponTexturePath,
                    weaponName.ToLower()));
            if (weaponTexture == null)
                GD.PushWarning($"Weapon Battle sprite for {weaponName} not found");

            _combatMenu.playerWeapon.Texture = weaponTexture;
            ((AtlasTexture) _combatMenu.enemyImage.Texture).Atlas =
                _enemyInstance.battleTexture;
            ((AtlasTexture) _combatMenu.enemyImage.Texture).Region = new Rect2(
                CombatAnimationUtil.AnimationStateRegionPositionX[
                    CombatAnimationUtil.AnimationState.Normal],
                CombatAnimationUtil.BattleTexturePosY,
                CombatAnimationUtil.BattleTextureWidth,
                CombatAnimationUtil.BattleTextureHeight);
        }

        private void ConnectCombatSignals()
        {
            if (!_playerInstance.IsConnected(nameof(BaseEntity.HealthChanged),
                _combatMenu,
                nameof(_combatMenu.UpdatePlayerHealthValue)))
                _playerInstance.Connect(nameof(BaseEntity.HealthChanged), _combatMenu,
                    nameof(_combatMenu.UpdatePlayerHealthValue));

            if (!_enemyInstance.IsConnected(nameof(BaseEntity.HealthChanged),
                _combatMenu,
                nameof(_combatMenu.UpdateEnemyHealthValue)))
                _enemyInstance.Connect(nameof(BaseEntity.HealthChanged), _combatMenu,
                    nameof(_combatMenu.UpdateEnemyHealthValue));

            if (!_enemyCombat.IsConnected(nameof(CombatController.DamageTaken), this,
                nameof(OnEnemy_TakeDamage)))
                _enemyCombat.Connect(nameof(CombatController.DamageTaken), this,
                    nameof(OnEnemy_TakeDamage));
        }

        private async void StartCombatLoop()
        {
            var introPlayer =
                AudioSystem.PlayMusic(AudioSystem.MusicCollection.BattleIntro);
            introPlayer.Connect("finished", this, nameof(PlayBattleMusic));
            var combat = true;
            while (combat)
            {
                _combatMenu.UpdateParticle(_enemyInstance);
                combat = await TakeTurn();
                if (combat)
                {
                    ApplyStatusEffects();

                    if (CheckCombatEnd())
                    {
                        await CheckCombatOutcome();
                        combat = false;
                    }
                }

                if (combat) _combatMenu.ResetUI();
            }
        }

        private async Task CheckCombatOutcome()
        {
            if (_playerInstance.Health <= 0)
            {
                await _combatMenu.ShowCombatLabel("YOU DIED", 2);
                await _combatMenu.ShowCombatLabel("GAME OVER", 2);
                _combatMenu.combatLabel.Visible = true;
                EndCombat(CombatOutcome.CombatLose);
            }
            else if (_enemyInstance.Health <= 0)
            {
                await _combatMenu.ShowCombatLabel("YOU WON", 2);
                await _combatMenu.ShowCombatLabel("CONGRATULATION", 2);
                _combatMenu.combatLabel.Visible = true;
                EndCombat(CombatOutcome.CombatWin);
            }
        }

        private void ApplyStatusEffects()
        {
            foreach (var statusEffectsKey in _playerInstance.statusEffects.Keys)
            {
                var statusEffect =
                    _playerInstance.statusEffects[statusEffectsKey];
                statusEffect.OnTurnEnd(_playerCombat);
                if (statusEffect.expired)
                    _playerInstance.statusEffects.Remove(statusEffectsKey);
            }

            foreach (var statusEffectsKey in _enemyInstance.statusEffects.Keys)
            {
                var statusEffect =
                    _enemyInstance.statusEffects[statusEffectsKey];
                statusEffect.OnTurnEnd(_enemyCombat);
                if (statusEffect.expired)
                    _enemyInstance.statusEffects.Remove(statusEffectsKey);
            }
        }

        private void EndCombat(CombatOutcome outcome)
        {
            AudioSystem.PlayMusic(AudioSystem.MusicCollection.Overworld);
            DisconnectCombatSignals();
            EmitSignal(nameof(CombatDone), outcome, _enemyInstance);
        }

        private void DisconnectCombatSignals()
        {
            _playerInstance.Disconnect(nameof(BaseEntity.HealthChanged), _combatMenu,
                nameof(_combatMenu.UpdatePlayerHealthValue));
            _enemyInstance.Disconnect(nameof(BaseEntity.HealthChanged), _combatMenu,
                nameof(_combatMenu.UpdateEnemyHealthValue));
        }


        private bool CheckCombatEnd()
        {
            return _playerInstance.Health <= 0 || _enemyInstance.Health <= 0;
        }

        private async Task<bool> TakeTurn()
        {
            var basePlayerAction
                = await _playerCombat.GetAction();
            var enemyAction = await _enemyCombat.GetAction() as CombatAction;
            _combatMenu.SetButtonsVisible(false);
            await _combatMenu.ShowTurnResult(basePlayerAction, enemyAction);
            var timer = GetTree().CreateTimer(1.5f);
            switch (basePlayerAction)
            {
                case FleeAction fleeAction:
                {
                    var flee = await EvaluateFlee(fleeAction, enemyAction);
                    if (flee)
                    {
                        _combatMenu.combatLabel.Visible = true;
                        EndCombat(CombatOutcome.CombatFlee);
                        return false;
                    }

                    break;
                }
                case CombatAction playerAction:
                    await EvaluateActions(playerAction, enemyAction);
                    break;
            }

            if (timer.TimeLeft > 0) await ToSignal(timer, "timeout");

            _combatMenu.HideTurnResult();
            if (CheckCombatEnd())
            {
                await CheckCombatOutcome();
                return false;
            }

            return true;
        }

        private async Task EvaluateActions(CombatAction playerAction,
            CombatAction enemyAction)
        {
            var turnOutcome =
                CombatAction.CompareActions(playerAction, enemyAction);
            switch (turnOutcome)
            {
                case TurnOutcome.Tie:
                    await EvaluateTie(playerAction, enemyAction);
                    break;
                case TurnOutcome.PlayerWin:
                    await PlayerAttack(playerAction);
                    _playerCombat.hitCombo += 1;
                    break;
                case TurnOutcome.EnemyWin:
                    await EnemyAttack(enemyAction);
                    break;
                default:
                    await _combatMenu.ShowCombatLabel("ERROR: Invalid win check",
                        2);
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task EnemyAttack(CombatAction enemyAction,
            float damageModifier = 1f)
        {
            var enemyDamage = _enemyCombat.GetBaseDamage(enemyAction);
            ApplyDamageModifier(damageModifier, ref enemyDamage);
            _enemyCombat.Attack(_playerCombat, enemyAction, enemyDamage);
            await _combatMenu.AnimatePlayerHurt(enemyAction);
        }


        private async Task PlayerAttack(CombatAction playerAction,
            float damageModifier = 1f)
        {
            var playerDamage = _playerCombat.GetBaseDamage(playerAction);
            ApplyDamageModifier(damageModifier, ref playerDamage);
            await _combatMenu.AnimatePlayerAttack(playerAction);
            _playerCombat.Attack(_enemyCombat, playerAction, playerDamage);
        }

        private async Task EvaluateTie(CombatAction playerAction,
            CombatAction enemyAction)
        {
            switch (playerAction)
            {
                case QuickAction _:
                    await PlayerAttack(playerAction);
                    await EnemyAttack(enemyAction);
                    break;
                case CounterAction _:
                    await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
                    break;
                case HeavyAction _:
                    await PlayerAttack(playerAction, 0.5f);
                    await EnemyAttack(enemyAction, 0.5f);
                    break;
                default:
                    _combatMenu.HideTurnResult();
                    await _combatMenu.ShowCombatLabel(
                        "ERROR. Unknown Action on EvaluateTie()",
                        2);
                    throw new ArgumentOutOfRangeException(
                        $"Unkown action {enemyAction}/{playerAction}");
            }
        }

        private static void ApplyDamageModifier(float damageModifier,
            ref int damage)
        {
            damage = Mathf.Max(1, (int) (damage * damageModifier));
        }

        private async Task<bool> EvaluateFlee(FleeAction fleeAction,
            CombatAction enemyAction)
        {
            async Task OnSuccess()
            {
                await _combatMenu.ShowCombatLabel("Got away safely", 2);
            }

            async Task OnSuccessWithDamage(float damageModifier)
            {
                await EnemyAttack(enemyAction, damageModifier);
                await _combatMenu.ShowCombatLabel("Got away not so safely", 2);
            }

            async Task OnFail(float damageModifier)
            {
                await _combatMenu.ShowCombatLabel("Failed to flee", 2);
                await EnemyAttack(enemyAction, damageModifier);
            }

            return await fleeAction.Evaluate(enemyAction, OnSuccess,
                OnSuccessWithDamage, OnFail);
        }

        private void PlayBattleMusic()
        {
            var enemyRace = _enemyInstance.Stat.Race;
            if (AudioSystem.MusicCollection.TryGetValue($"Battle{enemyRace}",
                out var music))
            {
                AudioSystem.PlayMusic(music);
            }
            else
            {
                GD.PrintErr($"Music not found for {enemyRace}");
            }
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
            base._Process(delta);
            GetNode<CanvasItem>("Background").Visible = _combatMenu.Visible;
        }

        [Signal] private delegate void CombatDone(CombatOutcome outcome,
            BaseEnemy enemyInstance);

        [Signal] private delegate void BagOpened(Player playerInstance);
    }
}