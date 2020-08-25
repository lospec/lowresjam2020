using System;
using Godot;
using HeroesGuild.Combat;
using HeroesGuild.data;
using HeroesGuild.Entities.enemies.base_enemy;
using HeroesGuild.Entities.Player;
using HeroesGuild.UI.dropped_items;
using HeroesGuild.UI.pause_menu;
using HeroesGuild.Utility;
using Array = Godot.Collections.Array;

namespace HeroesGuild.World
{
    // TODO Scene signals
    public class World : Node
    {
        private readonly PackedScene _baseEnemyScene =
            ResourceLoader.Load<PackedScene>("res://Entities/enemies/base_enemy/base_enemy.tscn");

        private YSort map;
        private Node2D enemySpawns;
        private Combat.Combat combat;
        private CombatMenu combatMenu;
        private PauseMenu pauseMenu;
        private Player player;
        private DroppedItems droppedItemsGUI;

        public override void _Ready()
        {
            map = GetNode<YSort>("Map");
            enemySpawns = GetNode<Node2D>("EnemySpawns");
            combat = GetNode<Combat.Combat>("Combat");
            combatMenu = combat.GetNode<CombatMenu>("CombatMenu");
            pauseMenu = GetNode<PauseMenu>("PauseMenu");
            player = map.GetNode<Player>("Player");
            droppedItemsGUI = GetNode<DroppedItems>("DroppedItems");

            player.birdsSystem.Visible = true;
            player.cloudsSystem.Visible = true;

            player.Position = Autoload.Get<SaveData>().WorldPosition;

            if (AudioSystem.CurrentPlayingMusic != AudioSystem.Music.Overworld)
            {
                AudioSystem.PlayMusic(AudioSystem.Music.Overworld, -30);
            }

            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            foreach (EnemySpawn.EnemySpawn enemySpawn in enemySpawns.GetChildren())
            {
                SpawnEnemy(enemySpawn);
            }
        }

        private void SpawnEnemy(EnemySpawn.EnemySpawn enemySpawn)
        {
            if (enemySpawn.enemies.Length == 0)
            {
                GD.PushWarning($"{enemySpawn} enemy spawn has no enemy names attached.");
                return;
            }

            if (enemySpawn.enemyNumber <= 0)
            {
                GD.PushWarning($"{enemySpawn} enemy spawn's enemyNumber not greater than 0");
                return;
            }

            for (var i = 0; i < enemySpawn.enemyNumber; i++)
            {
                var enemy = (BaseEnemy) _baseEnemyScene.Instance();
                var safe = false;
                var enemyName = enemySpawn.enemies.RandomElement();
                if (string.IsNullOrWhiteSpace(enemyName))
                {
                    GD.PushError($"{enemySpawn.Name} enemy spawn has a null enemy attached");
                    return;
                }

                var data = Autoload.Get<Data>();
                if (!data.EnemyData.ContainsKey(enemyName))
                {
                    GD.PushError($"{enemySpawn} enemy spawn has a {enemyName} enemy with no data attached.");
                    return;
                }

                enemy.LoadEnemy(enemyName);
                while (!safe)
                {
                    enemy.Position = enemySpawn.GetRandomGlobalPosition();
                    map.CallDeferred(nameof(map.AddChild), enemy);
                    safe = enemy.IsInAllowedTile();

                    if (!safe)
                    {
                        enemy.QueueFree();
                    }
                }

                enemy.Connect(nameof(BaseEnemy.Died), this, nameof(OnEnemy_Death), new Array {enemySpawn});
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (combatMenu.Visible && pauseMenu.pauseMenuControl.Visible)
                {
                    pauseMenu.TogglePauseCombat(player);
                }
                else
                {
                    if (droppedItemsGUI.margin.Visible)
                    {
                        droppedItemsGUI.Close();
                    }
                    else
                    {
                        pauseMenu.TogglePause(player);
                    }
                }
            }
        }

        private async void OnDoorDetection_BodyEntered(KinematicBody2D body)
        {
            if (!body.IsInGroup("enemies"))
            {
                var offset = new Vector2(0, 5);
                Autoload.Get<SaveData>().WorldPosition = player.Position + offset;

                AudioSystem.PlaySFX(AudioSystem.SFX.DoorOpen, null, -30);
                var transitionParams =
                    new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.3f);
                await Autoload.Get<Transitions>()
                    .ChangeSceneDoubleTransition("res://guild_hall/guild_hall.tscn", transitionParams);
            }
        }

        private void OnCombat_CombatDone(CombatUtil.CombatOutcome outcome, BaseEnemy enemyInstance)
        {
            switch (outcome)
            {
                case CombatUtil.CombatOutcome.CombatWin:
                    enemyInstance.Die();
                    droppedItemsGUI.DropItems(enemyInstance.EnemyName, player);
                    combatMenu.Visible = false;
                    GetTree().Paused = false;
                    player.hudMargin.Visible = true;
                    break;
                case CombatUtil.CombatOutcome.CombatLose:
                    enemyInstance.Die();
                    GetTree().Paused = false;
                    GameOver();
                    break;
                case CombatUtil.CombatOutcome.CombatFlee:
                    enemyInstance.Die();
                    GetTree().Paused = false;
                    if (player.Health <= 0)
                    {
                        GameOver();
                    }
                    else
                    {
                        combatMenu.Visible = false;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null);
            }
        }

        private async void GameOver()
        {
            var saveData = Autoload.Get<SaveData>();
            saveData.WorldPosition = SaveData.DefaultWorldPosition;
            saveData.Coins = SaveData.DEFAULT_COINS;
            saveData.Inventory = SaveData.DefaultInventory;
            saveData.EquippedWeapon = SaveData.DEFAULT_WEAPON;
            saveData.EquippedArmor = SaveData.DEFAULT_ARMOR;
            saveData.MaxHealth = SaveData.DEFAULT_HEALTH;
            saveData.Health = SaveData.DEFAULT_HEALTH;
            player.Health = SaveData.DEFAULT_HEALTH;

            AudioSystem.StopMusic();
            await Autoload.Get<Transitions>().ChangeSceneDoubleTransition("res://UI/Main Menu/Main Menu.tscn",
                new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.2f));
        }

        private async void OnEnemy_Death(EnemySpawn.EnemySpawn enemySpawnInstance)
        {
            var timer = new Timer();
            CallDeferred(nameof(AddChild), timer);
            timer.Connect("timeout", this, nameof(SpawnEnemy), new Array {enemySpawnInstance});
            timer.Connect("timeout", timer, nameof(QueueFree));
            await ToSignal(timer, "ready");
            timer.Start((float) GD.RandRange(5, 10));
        }
    }
}