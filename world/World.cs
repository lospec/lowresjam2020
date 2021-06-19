using System;
using System.Collections.Generic;
using Godot;
using HeroesGuild.combat;
using HeroesGuild.data;
using HeroesGuild.entities.enemies.base_enemy;
using HeroesGuild.entities.player;
using HeroesGuild.ui.dropped_items;
using HeroesGuild.ui.pause_menu;
using HeroesGuild.utility;
using HeroesGuild.world.enemy_spawn;
using HeroesGuild.world.static_objects;
using Array = Godot.Collections.Array;

namespace HeroesGuild.world
{
    public class World : Node
    {
        private const string GameOverScenePath = "res://ui/game_over/game_over.tscn";
        private const string GuildHallScenePath = "res://guild_hall/guild_hall.tscn";
        private readonly PackedScene _baseEnemyScene =
            ResourceLoader.Load<PackedScene>(
                "res://entities/enemies/base_enemy/base_enemy.tscn");
        private Combat _combat;
        private CombatMenu _combatMenu;
        private DroppedItems _droppedItemsGUI;
        private Node2D _enemySpawns;

        private YSort _map;
        private PauseMenu _pauseMenu;
        private Player _player;
        private GuildHall _guildHall;

        private Vector2 _returnToGuildPosition;

        public override void _Ready()
        {
            _map = GetNode<YSort>("Map");
            _enemySpawns = GetNode<Node2D>("EnemySpawns");
            _combat = GetNode<Combat>("Combat");
            _combatMenu = _combat.GetNode<CombatMenu>("CombatMenu");
            _pauseMenu = GetNode<PauseMenu>("PauseMenu");
            _player = _map.GetNode<Player>("Player");
            _guildHall = _map.GetNode<GuildHall>("GuildHall");
            _droppedItemsGUI = GetNode<DroppedItems>("DroppedItems");

            _player.birdsSystem.Visible = true;
            _player.cloudsSystem.Visible = true;

            _player.Position = SaveManager.SaveData.WorldPosition;

            _returnToGuildPosition = _guildHall.getPlayerExitPosition();
            _pauseMenu.returnToGuildPosition = _returnToGuildPosition;

            if (!AudioSystem.IsMusicPlaying)
            {
                AudioSystem.PlayMusic(AudioSystem.MusicCollection.Overworld);
            }

            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            foreach (EnemySpawn enemySpawn in _enemySpawns.GetChildren())
                SpawnEnemy(enemySpawn);
        }

        private void SpawnEnemy(EnemySpawn enemySpawn)
        {
            if (enemySpawn.enemies.Length == 0)
            {
                GD.PushWarning(
                    $"{enemySpawn} enemy spawn has no enemy names attached.");
                return;
            }

            if (enemySpawn.enemyNumber <= 0)
            {
                GD.PushWarning(
                    $"{enemySpawn} enemy spawn's enemyNumber not greater than 0");
                return;
            }

            for (var i = 0; i < enemySpawn.enemyNumber; i++)
            {
                var enemy = (BaseEnemy) _baseEnemyScene.Instance();
                var safe = false;
                var enemyName = enemySpawn.enemies.RandomElement();
                if (string.IsNullOrWhiteSpace(enemyName))
                {
                    GD.PushError(
                        $"{enemySpawn.Name} enemy spawn has a null enemy attached");
                    return;
                }

                var data = Autoload.Get<Data>();
                if (!data.enemyData.ContainsKey(enemyName))
                {
                    GD.PushError(
                        $"{enemySpawn} enemy spawn has a {enemyName} enemy with no data attached.");
                    return;
                }

                enemy.OnDependency = (ref BaseEnemy.EnemyDependencies dependency) =>
                {
                    dependency.EnemyName = enemyName;
                    dependency.PlayerInstance = _player;
                };

                while (!safe)
                {
                    enemy.Position = enemySpawn.GetRandomGlobalPosition();
                    _map.CallDeferred("add_child", enemy);
                    safe = enemy.IsInAllowedTile();

                    if (!safe) enemy.QueueFree();
                }

                enemy.Connect(nameof(BaseEnemy.Died), this, nameof(OnEnemy_Death),
                    new Array {enemySpawn});
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (_combatMenu.Visible)
                {
                    if (_pauseMenu.pauseMenuControl.Visible)
                        _pauseMenu.TogglePauseCombat(_player);
                }
                else
                {
                    if (_droppedItemsGUI.margin.Visible)
                        _droppedItemsGUI.Close();
                    else
                        _pauseMenu.TogglePause(_player);
                }
            }
        }

        private async void OnDoorDetection_BodyEntered(KinematicBody2D body)
        {
            if (body.IsInGroup("player"))
            {
                var offset = new Vector2(0, 5);
                SaveManager.SaveData.WorldPosition = _player.Position + offset;

                AudioSystem.PlaySFX(AudioSystem.SFXCollection.GuildHallEnter);
                var transitionParams =
                    new Transitions.TransitionParams(
                        Transitions.TransitionType.ShrinkingCircle, 0.3f);

                await Autoload.Get<Transitions>()
                    .ChangeSceneDoubleTransition(GuildHallScenePath, transitionParams);
            }
        }

        private void OnCombat_CombatDone(CombatUtil.CombatOutcome outcome,
            BaseEnemy enemyInstance)
        {
            switch (outcome)
            {
                case CombatUtil.CombatOutcome.CombatWin:
                    AudioSystem.StopAllMusic();
                    AudioSystem.PlayMusic(AudioSystem.MusicCollection.BattleVictoryJingle);

                    enemyInstance.Die();
                    _combatMenu.Visible = false;
                    _droppedItemsGUI.DropItems(enemyInstance.enemyName, _player);
                    break;
                case CombatUtil.CombatOutcome.CombatLose:
                    enemyInstance.Die();
                    GetTree().Paused = false;
                    GameOver();
                    break;
                case CombatUtil.CombatOutcome.CombatFlee:
                    AudioSystem.StopAllMusic();
                    AudioSystem.PlayMusic(AudioSystem.MusicCollection.Overworld);
                    enemyInstance.Die();
                    GetTree().Paused = false;
                    if (_player.Health <= 0)
                        GameOver();
                    else
                        _combatMenu.Visible = false;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outcome), outcome,
                        null);
            }
        }

        private void _on_DroppedItems_Closed(bool wasAutomatic)
        {
            AudioSystem.StopAllAudio();
            AudioSystem.PlayMusic(AudioSystem.MusicCollection.Overworld);

            _combatMenu.Visible = false;
            _player.hudMargin.Visible = true;
            GetTree().Paused = false;
        }

        private async void GameOver()
        {
            _player.isDead = true;

            var saveData = SaveManager.SaveData;
            saveData.WorldPosition = SaveData.DefaultWorldPosition;
            saveData.Coins = SaveData.DefaultCoins;
            saveData.Inventory = new List<string>(SaveData.DefaultInventory);
            saveData.EquippedWeapon = SaveData.DefaultWeapon;
            saveData.EquippedArmor = SaveData.DefaultArmor;
            saveData.MaxHealth = SaveData.DefaultHealth;
            saveData.Health = SaveData.DefaultHealth;
            _player.Health = SaveData.DefaultHealth;

            AudioSystem.StopAllMusic();
            await Autoload.Get<Transitions>().ChangeSceneDoubleTransition(
                GameOverScenePath,
                new Transitions.TransitionParams(
                    Transitions.TransitionType.ShrinkingCircle, 0.2f));
        }

        private async void OnEnemy_Death(BaseEnemy enemy, EnemySpawn enemySpawnInstance)
        {
            var timer = new Timer();
            CallDeferred("add_child", timer);
            timer.Connect("timeout", this, nameof(SpawnEnemy),
                new Array {enemySpawnInstance});
            timer.Connect("timeout", timer, "queue_free");
            await ToSignal(timer, "ready");
            timer.Start((float) GD.RandRange(5, 10));
        }
    }
}