using System.Linq;
using Godot;
using HeroesGuild.entities.player;
using HeroesGuild.guild_hall.chest;
using HeroesGuild.guild_hall.guild_interface;
using HeroesGuild.ui.pause_menu;
using HeroesGuild.utility;

namespace HeroesGuild.guild_hall
{
    public class GuildHall : Node
    {
        public enum Tiles
        {
            Empty = -1,
            PlankOdd,
            PlankEven,
            Black,
            Door,
            Wall
        }

        private const string WorldScenePath = "res://world/world.tscn";

        private const int CHEST_GAP_X = 10;
        private const int CHEST_GAP_Y = 0;
        private const int TOP_POS_Y = -8;
        private const int BOTTOM_POS_Y = 6;
        private const int BLACK_TILES_POS_Y = 7;
        private const int PLANK_START_POS_Y = -2;
        private static readonly PackedScene ChestResource = ResourceLoader
            .Load<PackedScene>("res://guild_hall/chest/chest.tscn");

        private ChestGUI _chestGUI;
        private TileMap _extendedTileMap;
        private Vector2 _firstChestPosition;
        private GuildInterface _guildInterface;
        private YSort _objectsYSort;
        private PauseMenu _pauseMenu;
        private Player _player;
        private CollisionShape2D _rightBorder;

        public Chest currentOpenedChest = null;
        public bool guildInterfaceOpen = false;

        public override void _Ready()
        {
            _chestGUI = GetNode<ChestGUI>("ChestGUI");
            _objectsYSort = GetNode<YSort>("Objects");
            _player = _objectsYSort.GetNode<Player>("Player");
            _pauseMenu = GetNode<PauseMenu>("PauseMenu");
            _guildInterface = GetNode<GuildInterface>("GuildInterface");
            _extendedTileMap = GetNode<TileMap>("Extended");
            _firstChestPosition = GetNode<Position2D>("FirstChestPosition").Position;
            _rightBorder = GetNode<CollisionShape2D>("Borders/Right");
            _player.birdsSystem.Visible = false;
            _player.cloudsSystem.Visible = false;
            UpdateGuildFromLevel();
        }

        private async void OnDoorDetection_BodyEntered(KinematicBody2D body)
        {
            if (body.IsInGroup("player"))
            {
				AudioSystem.PlaySFX(AudioSystem.SFXCollection.GuildHallEnter);
                await Autoload.Get<Transitions>().ChangeSceneDoubleTransition
                (WorldScenePath,
                    new Transitions.TransitionParams(
                        Transitions.TransitionType.ShrinkingCircle, 0.3f));
            }
        }

        private async void OnPlayerOpenChest_InputReceived(Chest chest)
        {
            if (currentOpenedChest != null || chest.animatedSprite.Playing) return;

            AudioSystem.PlaySFX(AudioSystem.SFXCollection.GuildHallChestOpen);
            chest.animatedSprite.Play("open");
            await ToSignal(chest.animatedSprite, "animation_finished");
            chest.animatedSprite.Stop();
            currentOpenedChest = chest;
            _chestGUI.Open(_player, chest);
        }

        public override async void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                if (guildInterfaceOpen)
                {
                    guildInterfaceOpen = false;
                    _guildInterface.Toggle(_player);
                }
                else
                {
                    _pauseMenu.TogglePause(_player);
                }
            }

            if (Input.IsActionJustPressed("close_chest") && currentOpenedChest !=
                null && !currentOpenedChest.animatedSprite.Playing)
            {
                _chestGUI.Close();
                currentOpenedChest.animatedSprite.Play("close");
                await ToSignal(currentOpenedChest.animatedSprite, "animation_finished");
                currentOpenedChest.animatedSprite.Stop();
                currentOpenedChest = null;
            }
        }

        private void OnPlayerGuildHallDesk_InputReceived(Node desk)
        {
            guildInterfaceOpen = true;
            _guildInterface.Toggle(_player);
        }

        private void OnGuildInterface_GuildHallLevelUp()
        {
            UpdateGuildFromLevel();
        }

        private void UpdateGuildFromLevel()
        {
            foreach (Vector2 tile in _extendedTileMap.GetUsedCells())
                _extendedTileMap.SetCellv(tile, (int) Tiles.Empty);

            foreach (Node chest in GetTree().GetNodesInGroup("Chest"))
                chest.QueueFree();

            var saveData = Autoload.Get<SaveData>();
            for (var i = 0; i < saveData.GuildLevel; i++)
            {
                if (i >= saveData.ChestContent.Count)
                    saveData.ChestContent.Add(Enumerable.Range(0, 8)
                        .ToDictionary(key => key, _ => string.Empty));

                var position = _firstChestPosition +
                               new Vector2(CHEST_GAP_X * i, CHEST_GAP_Y * i);
                var chestInstance = (Chest) ChestResource.Instance();
                chestInstance.Position = position;
                chestInstance.chestID = i;
                _objectsYSort.AddChild(chestInstance);

                var x = i;
                _extendedTileMap.SetCell(x, BLACK_TILES_POS_Y, (int) Tiles.Black);
                for (var y = TOP_POS_Y; y < BOTTOM_POS_Y + 1; y++)
                {
                    x = y % 2 == 0 ? i + 1 : i;
                    var tile = Tiles.Wall;
                    if (y >= PLANK_START_POS_Y)
                        tile = y % 2 == 0 ? Tiles.PlankEven : Tiles.PlankOdd;

                    _extendedTileMap.SetCell(x, y, (int) tile);
                }

                var rightBorderPosition = _rightBorder.Position;
                rightBorderPosition.x = 38 + CHEST_GAP_X * (saveData.GuildLevel - 1);
                _rightBorder.Position = rightBorderPosition;

                _player.camera.LimitRight =
                    32 + CHEST_GAP_X * (saveData.GuildLevel - 1);
            }
        }
    }
}