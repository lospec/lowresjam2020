using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.Combat.Effects.animations;
using HeroesGuild.Entities.BaseEntity;
using HeroesGuild.Entities.enemies.base_enemy;

namespace HeroesGuild.Combat
{
    public class CombatMenu : ShakableControl
    {
        [Signal] public delegate void ActionSelected(CombatUtil.CombatAction action);

        [Signal] public delegate void BagOpened();

        public enum Menu
        {
            Main,
            Attack
        }

        public Menu CurrentMenu = Menu.Main;
        private PackedScene damageLabel = ResourceLoader.Load<PackedScene>("res://Combat/Effects/DamageLabel.tscn");
        private MarginContainer buttons;
        private MarginContainer mainButtonsMenu;
        private MarginContainer attackButtonsMenu;
        public Label combatLabel;
        private CombatTurnResultUI combatTurnResult;
        private Label playerHealthLabel;
        private HealthIcon playerHealthIcon;
        public TextureRect playerWeapon;
        private TextureProgress enemyHealthBar;
        private Tween enemyHealthBarTween;
        public CombatEnemyTexture enemyImage;
        private CombatAttackAnim attackEffect;
        private Control damageSpawnArea;
        private AnimationList effectAnimations;
        private Control particlePos;


        public override void _Ready()
        {
            buttons = GetNode<MarginContainer>("VBoxContainer/PlayerHUD/ChoiceHUD/Buttons");
            mainButtonsMenu = GetNode<MarginContainer>("VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/MainButtonsMenu");
            attackButtonsMenu =
                GetNode<MarginContainer>("VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/AttackButtonsMenu");
            combatLabel = GetNode<Label>("VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel");
            combatTurnResult = GetNode<CombatTurnResultUI>("VBoxContainer/PlayerHUD/ChoiceHUD/CombatTurnResult");
            playerHealthLabel =
                GetNode<Label>("VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/Health");
            playerHealthIcon =
                GetNode<HealthIcon>(
                    "VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/MarginContainer/HealthIcon");
            playerWeapon = GetNode<TextureRect>("WeaponContainer/PlayerWeapon");
            enemyHealthBar =
                GetNode<TextureProgress>("VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer");
            enemyHealthBarTween =
                GetNode<Tween>("VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/Tween");
            enemyImage = GetNode<CombatEnemyTexture>("VBoxContainer/EnemyHUD/VBoxContainer/Enemy");
            attackEffect = GetNode<CombatAttackAnim>("EffectsContainer/EffectTexture");
            damageSpawnArea = GetNode<Control>("EffectsContainer/DamageSpawnArea");
            effectAnimations = GetNode<AnimationList>("EffectAnimationList");
            particlePos = GetNode<Control>("VBoxContainer/EnemyHUD/VBoxContainer/Enemy/ParticlePos");

            Visible = false;
            ResetUI();
        }

        public void ResetUI()
        {
            HideTurnResult();
            buttons.Visible = true;
            switch (CurrentMenu)
            {
                case Menu.Main:
                    mainButtonsMenu.Visible = true;
                    attackButtonsMenu.Visible = false;
                    break;
                case Menu.Attack:
                    mainButtonsMenu.Visible = false;
                    attackButtonsMenu.Visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            combatLabel.Visible = false;
        }

        public void HideTurnResult()
        {
            combatTurnResult.winContainer.Visible = false;
            combatTurnResult.compareContainer.Visible = false;
        }

        public void SetPlayerHealthValue(int maxHealth, int currentHealth)
        {
            playerHealthLabel.Text = $"{currentHealth}";
        }

        public void SetEnemyHealthValue(int maxHealth, int currentHealth)
        {
            enemyHealthBar.MaxValue = maxHealth;
            enemyHealthBar.Value = currentHealth;
        }

        public void UpdatePlayerHealthValue(int oldHealth, int newHealth)
        {
            playerHealthLabel.Text = $"{newHealth}";
        }

        public void UpdateEnemyHealthValue(int oldHealth, int newHealth)
        {
            enemyHealthBarTween.InterpolateProperty(enemyHealthBarTween, "value", enemyHealthBar.Value, newHealth,
                1f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            enemyHealthBarTween.Start();
        }

        public void SetButtonsVisible(bool visible = true)
        {
            buttons.Visible = visible;
        }

        public async Task ShowTurnResult(CombatUtil.CombatAction playerAction, CombatUtil.CombatAction enemyAction)
        {
            combatTurnResult.Visible = true;
            await combatTurnResult.ShowTurnCompare(playerAction, enemyAction, 1.5f);
            combatTurnResult.ShowWinResult(playerAction, enemyAction);
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

        public async Task AnimatePlayerAttack(PlayerCombat playerCombat, CombatUtil.CombatAction action)
        {
            if (action == CombatUtil.CombatAction.Counter)
            {
                attackEffect.Play(effectAnimations.GetAnimation("counter"),
                    CombatUtil.GetActionColor(CombatUtil.CombatAction.Heavy));
                await ToSignal(attackEffect, nameof(CombatAttackAnim.EffectDone));
            }

            var damageType = playerCombat.GetDamageType(action);
            var effectAnimation = effectAnimations.GetAnimation(damageType);
            attackEffect.Play(effectAnimation, CombatUtil.GetActionColor(action));
            await ToSignal(attackEffect, nameof(CombatAttackAnim.EffectDone));
        }

        public async Task AnimatePlayerHurt(int damage, bool enemyCountered = false)
        {
            if (enemyCountered)
            {
                attackEffect.Play(effectAnimations.GetAnimation("counter"),
                    CombatUtil.GetActionColor(CombatUtil.CombatAction.Heavy));
                await ToSignal(attackEffect, nameof(CombatAttackAnim.EffectDone));
            }

            Shake(1, 20, 1);
            playerHealthIcon.Blink(1, 6);
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
                    .AnimationStateRegionPositionX[CombatAnimationUtil.AnimationState.Normal];
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
            var label = (Label) damageLabel.Instance();
            damageSpawnArea.AddChild(label);
            label.RectPosition = new Vector2
            {
                x = (int) GD.RandRange(0f, damageSpawnArea.RectSize.x) - label.RectSize.x / 2,
                y = (int) GD.RandRange(0f, damageSpawnArea.RectSize.y) - label.RectSize.y / 2
            };
            label.Text = $"{damage}";
        }

        private bool HasParticle(string name) =>
            particlePos.GetChildren().Cast<Node>().Any(partice => partice.Name == name);

        private void SpawnParticle(string name)
        {
            if (HasParticle(name))
            {
                return;
            }

            var path = $"res://Particle Systems/Particles/{name.ToLower()}.tscn";
            var particle = GD.Load<PackedScene>(path).Instance();
            particle.Name = name;
            particlePos.AddChild(particle);
        }

        private void RemoveParticle(string name)
        {
            foreach (Node particle in particlePos.GetChildren())
            {
                if (particle.Name == name)
                {
                    particlePos.RemoveChild(particle);
                    particle.QueueFree();
                }
            }
        }

        private void RemoveAllParticles()
        {
            foreach (Node particle in particlePos.GetChildren())
            {
                particlePos.RemoveChild(particle);
                particle.QueueFree();
            }
        }

        public void UpdateParticle(BaseEntity enemyInstance)
        {
            foreach (var particleName in enemyInstance.StatusEffects.Keys
                .Select(statusEffectsKey => statusEffectsKey.ToLower())
                .Where(particleName => !HasParticle(particleName)))
            {
                SpawnParticle(particleName);
            }

            foreach (Node particle in particlePos.GetChildren())
            {
                if (!enemyInstance.StatusEffects.ContainsKey(particle.Name))
                {
                    RemoveParticle(particle.Name);
                }
            }
        }

        private void OpenMainMenu()
        {
            CurrentMenu = Menu.Main;
            ResetUI();
        }

        private void OpenAttackMenu()
        {
            CurrentMenu = Menu.Attack;
            ResetUI();
        }

        private void OnAttack_Pressed()
        {
            OpenAttackMenu();
        }

        private void OnCombatMenu_GUIInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel") && CurrentMenu == Menu.Attack)
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