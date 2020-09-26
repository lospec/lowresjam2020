using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.Combat.Effects.Animations;
using HeroesGuild.Entities.BaseEntity;
using HeroesGuild.Entities.Enemies.BaseEnemy;

namespace HeroesGuild.Combat
{
    public class CombatMenu : ShakableControl
    {
        [Signal] public delegate void ActionSelected(CombatUtil.CombatAction action);

        [Signal] public delegate void BagOpened();

        private static readonly PackedScene DamageLabel =
            ResourceLoader.Load<PackedScene>("res://combat/effects/damage_label.tscn");

        private const string ParticlePath = "res://particle_systems/particles/{0}.tscn";

        public enum Menu
        {
            Main,
            Attack
        }

        public Menu currentMenu = Menu.Main;

        private MarginContainer _buttons;
        private MarginContainer _mainButtonsMenu;
        private MarginContainer _attackButtonsMenu;
        public Label combatLabel;
        private CombatTurnResultUI _combatTurnResult;
        private Label _playerHealthLabel;
        private HealthIcon _playerHealthIcon;
        public TextureRect playerWeapon;
        private TextureProgress _enemyHealthBar;
        private Tween _enemyHealthBarTween;
        public CombatEnemyTexture enemyImage;
        private CombatAttackAnim _attackEffect;
        private Control _damageSpawnArea;
        private AnimationList _effectAnimations;
        private Control _particlePos;


        public override void _Ready()
        {
            _buttons =
                GetNode<MarginContainer>("VBoxContainer/PlayerHUD/ChoiceHUD/Buttons");
            _mainButtonsMenu =
                GetNode<MarginContainer>(
                    "VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/MainButtonsMenu");
            _attackButtonsMenu =
                GetNode<MarginContainer>(
                    "VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/AttackButtonsMenu");
            combatLabel =
                GetNode<Label>(
                    "VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel");
            _combatTurnResult =
                GetNode<CombatTurnResultUI>(
                    "VBoxContainer/PlayerHUD/ChoiceHUD/CombatTurnResult");
            _playerHealthLabel =
                GetNode<Label>(
                    "VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/Health");
            _playerHealthIcon =
                GetNode<HealthIcon>(
                    "VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/MarginContainer/HealthIcon");
            playerWeapon = GetNode<TextureRect>("WeaponContainer/PlayerWeapon");
            _enemyHealthBar =
                GetNode<TextureProgress>(
                    "VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/EnemyHealthBar");
            _enemyHealthBarTween =
                GetNode<Tween>(
                    "VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/Tween");
            enemyImage =
                GetNode<CombatEnemyTexture>(
                    "VBoxContainer/EnemyHUD/VBoxContainer/Enemy");
            _attackEffect = GetNode<CombatAttackAnim>("EffectsContainer/EffectTexture");
            _damageSpawnArea = GetNode<Control>("EffectsContainer/DamageSpawnArea");
            _effectAnimations = GetNode<AnimationList>("EffectAnimationList");
            _particlePos =
                GetNode<Control>(
                    "VBoxContainer/EnemyHUD/VBoxContainer/Enemy/ParticlePos");

            Visible = false;
            ResetUI();
        }

        public void ResetUI()
        {
            HideTurnResult();
            _buttons.Visible = true;
            switch (currentMenu)
            {
                case Menu.Main:
                    _mainButtonsMenu.Visible = true;
                    _attackButtonsMenu.Visible = false;
                    break;
                case Menu.Attack:
                    _mainButtonsMenu.Visible = false;
                    _attackButtonsMenu.Visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            combatLabel.Visible = false;
        }

        public void HideTurnResult()
        {
            _combatTurnResult.winContainer.Visible = false;
            _combatTurnResult.compareContainer.Visible = false;
        }

        public void SetPlayerHealthValue(int maxHealth, int currentHealth)
        {
            _playerHealthLabel.Text = $"{currentHealth}";
        }

        public void SetEnemyHealthValue(int maxHealth, int currentHealth)
        {
            _enemyHealthBar.MaxValue = maxHealth;
            _enemyHealthBar.Value = currentHealth;
        }

        public void UpdatePlayerHealthValue(int oldHealth, int newHealth)
        {
            _playerHealthLabel.Text = $"{newHealth}";
        }

        public void UpdateEnemyHealthValue(int oldHealth, int newHealth)
        {
            _enemyHealthBarTween.InterpolateProperty(_enemyHealthBar, "value",
                _enemyHealthBar.Value, newHealth,
                1f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            _enemyHealthBarTween.Start();
        }

        public void SetButtonsVisible(bool visible = true)
        {
            _buttons.Visible = visible;
        }

        public async Task ShowTurnResult(CombatUtil.CombatAction playerAction,
            CombatUtil.CombatAction enemyAction)
        {
            _combatTurnResult.Visible = true;
            await _combatTurnResult.ShowTurnCompare(playerAction, enemyAction);
            _combatTurnResult.ShowWinResult(playerAction, enemyAction);
        }

        public async Task ShowCombatLabel(string text, float duration = 0f)
        {
            combatLabel.Text = text;
            combatLabel.Visible = true;

            if (duration > 0)
            {
                await ToSignal(GetTree().CreateTimer(duration), "timeout");
                combatLabel.Visible = false;
            }
        }

        public async Task AnimatePlayerAttack(PlayerCombat playerCombat,
            CombatUtil.CombatAction action)
        {
            if (action == CombatUtil.CombatAction.Counter)
            {
                _attackEffect.Play(_effectAnimations.GetAnimation("counter"),
                    CombatUtil.GetActionColor(CombatUtil.CombatAction.Heavy));
                await ToSignal(_attackEffect, nameof(CombatAttackAnim.EffectDone));
            }

            var damageType = playerCombat.GetDamageType(action);
            var effectAnimation = _effectAnimations.GetAnimation(damageType);
            _attackEffect.Play(effectAnimation, CombatUtil.GetActionColor(action));
            await ToSignal(_attackEffect, nameof(CombatAttackAnim.EffectDone));
        }

        public async Task AnimatePlayerHurt(int damage, bool enemyCountered = false)
        {
            if (enemyCountered)
            {
                _attackEffect.Play(_effectAnimations.GetAnimation("counter"),
                    CombatUtil.GetActionColor(CombatUtil.CombatAction.Heavy));
                await ToSignal(_attackEffect, nameof(CombatAttackAnim.EffectDone));
            }

            Shake(1, 20, 1);
            _playerHealthIcon.Blink(1, 6);
            await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        }

        public async void AnimateEnemyHurt(BaseEnemy enemyInstance, int damage)
        {
            SpawnEnemyDamageLabel(damage);
            enemyImage.Shake(1, 15, 1);

            var region = ((AtlasTexture) enemyImage.Texture).Region;
            var regionPosition = region.Position;

            regionPosition.x = CombatAnimationUtil
                .AnimationStateRegionPositionX[CombatAnimationUtil.AnimationState.Hurt];

            if (enemyInstance.Health > 0)
            {
                await ToSignal(GetTree().CreateTimer(1), "timeout");
                regionPosition.x = CombatAnimationUtil
                    .AnimationStateRegionPositionX[
                        CombatAnimationUtil.AnimationState.Normal];
            }
            else
            {
                var modulate = enemyImage.Modulate;
                modulate.a = 0;
                enemyImage.Modulate = modulate;
            }

            region.Position = regionPosition;
            ((AtlasTexture) enemyImage.Texture).Region = region;
        }

        private void SpawnEnemyDamageLabel(int damage)
        {
            var label = (Label) DamageLabel.Instance();
            _damageSpawnArea.AddChild(label);
            label.RectPosition = new Vector2
            {
                x = (int) GD.RandRange(0f, _damageSpawnArea.RectSize.x) -
                    label.RectSize.x / 2,
                y = (int) GD.RandRange(0f, _damageSpawnArea.RectSize.y) -
                    label.RectSize.y / 2
            };
            label.Text = $"{damage}";
        }

        private bool HasParticle(string name) =>
            _particlePos.GetChildren().Cast<Node>()
                .Any(partice => partice.Name == name);

        private void SpawnParticle(string name)
        {
            if (HasParticle(name))
            {
                return;
            }

            var particle =
                GD.Load<PackedScene>(string.Format(ParticlePath, name.ToLower()))
                    .Instance();
            particle.Name = name;
            _particlePos.AddChild(particle);
        }

        private void RemoveParticle(string name)
        {
            foreach (Node particle in _particlePos.GetChildren())
            {
                if (particle.Name == name)
                {
                    _particlePos.RemoveChild(particle);
                    particle.QueueFree();
                }
            }
        }

        private void RemoveAllParticles()
        {
            foreach (Node particle in _particlePos.GetChildren())
            {
                _particlePos.RemoveChild(particle);
                particle.QueueFree();
            }
        }

        public void UpdateParticle(BaseEntity enemyInstance)
        {
            foreach (var particleName in enemyInstance.statusEffects.Keys
                .Select(statusEffectsKey => statusEffectsKey.ToLower())
                .Where(particleName => !HasParticle(particleName)))
            {
                SpawnParticle(particleName);
            }

            foreach (Node particle in _particlePos.GetChildren())
            {
                if (!enemyInstance.statusEffects.ContainsKey(particle.Name))
                {
                    RemoveParticle(particle.Name);
                }
            }
        }

        private void OpenMainMenu()
        {
            currentMenu = Menu.Main;
            ResetUI();
        }

        private void OpenAttackMenu()
        {
            currentMenu = Menu.Attack;
            ResetUI();
        }

        private void OnAttack_Pressed()
        {
            OpenAttackMenu();
        }

        private void OnCombatMenu_GUIInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel") && currentMenu == Menu.Attack)
            {
                OpenMainMenu();
            }
        }

        private void OnBack_Pressed()
        {
            OpenMainMenu();
        }

        private void OnBag_Pressed()
        {
            EmitSignal(nameof(BagOpened));
        }

        private void OnCounter_Pressed()
        {
            EmitSignal(nameof(ActionSelected), CombatUtil.CombatAction.Counter);
        }

        private void OnQuick_Pressed()
        {
            EmitSignal(nameof(ActionSelected), CombatUtil.CombatAction.Quick);
        }

        private void OnHeavy_Pressed()
        {
            EmitSignal(nameof(ActionSelected), CombatUtil.CombatAction.Heavy);
        }

        private void OnFlee_Pressed()
        {
            EmitSignal(nameof(ActionSelected), CombatUtil.CombatAction.Flee);
        }
    }
}