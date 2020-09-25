using System.Collections.Generic;
using Godot;
using HeroesGuild.Utility;

namespace HeroesGuild.Entities.Player
{
    public class Player : BaseEntity.BaseEntity
    {
        private const string OverWorldSprite =
            "res://entities/player/spritesheets/{0}_overworld.png";

        [Signal] public delegate void EnemyDetected(BaseEntity.BaseEntity player,
            BaseEntity.BaseEntity enemy);

        [Signal] public delegate void InventoryButtonPressed(
            BaseEntity.BaseEntity player);

        [Signal] public delegate void OpenChestInputReceived(Node2D chest);
        
        [Signal] public delegate void GuildHallDeskInputReceived(Node2D desk);

        public int Coins { get; set; }
        public List<string> Inventory { get; set; } = new List<string>();
        public string EquippedWeapon { get; set; }
        public string EquippedArmor { get; set; }

        public MarginContainer hudMargin;
        private Label _hudHealthLabel;
        private Area2D _chestDetector;
        private Area2D _deskDetector;
        public Camera2D camera;
        private Area2D _collisionDetector;

        public Node2D birdsSystem;
        public Node2D cloudsSystem;

        public override void _Ready()
        {
            hudMargin = GetNode<MarginContainer>("HUD/MarginContainer");
            _hudHealthLabel =
                GetNode<Label>(
                    "HUD/MarginContainer/HealthMargin/MarginContainer/HBoxContainer/Health");
            _chestDetector = GetNode<Area2D>("ChestDetector");
            _deskDetector = GetNode<Area2D>("DeskDetector");
            camera = GetNode<Camera2D>("Camera2D");
            _collisionDetector = GetNode<Area2D>("CollisionDetector");
            birdsSystem = GetNode<Node2D>("BirdsSystem");
            cloudsSystem = GetNode<Node2D>("CloudsSystem");

            var saveData = Autoload.Get<SaveData>();
            Coins = saveData.Coins;
            Inventory = saveData.Inventory;
            EquippedWeapon = saveData.EquippedWeapon;
            EquippedArmor = saveData.EquippedArmor;
            maxHealth = saveData.MaxHealth;
            Health = saveData.Health;

            base._Ready();

            _hudHealthLabel.Text = $"{Health}/{maxHealth}";


            var texture =
                ResourceLoader.Load<Texture>(string.Format(OverWorldSprite,
                    saveData.CharacterName.ToLower()));

            sprite.Texture = texture;
        }

        private void OnPlayerTree_Exiting()
        {
            var saveData = Autoload.Get<SaveData>();
            saveData.Coins = Coins;
            saveData.Inventory = Inventory;
            saveData.EquippedWeapon = EquippedWeapon;
            saveData.EquippedArmor = EquippedArmor;
            saveData.MaxHealth = maxHealth;
            saveData.Health = Health;
        }

        public override void _PhysicsProcess(float delta)
        {
            var inputVelocity = Vector2.Zero;
            inputVelocity.x = Input.GetActionStrength("player_move_right")
                              - Input.GetActionStrength("player_move_left");
            inputVelocity.y = Input.GetActionStrength("player_move_down")
                              - Input.GetActionStrength("player_move_up");
            Velocity = inputVelocity.Normalized() * moveSpeed;
            base._PhysicsProcess(delta);
        }

        public override void _Process(float delta)
        {
            _hudHealthLabel.Text = $"{Health}/{maxHealth}";
        }

        private void OnEntityDetector_BodyEntered(Node body)
        {
            if (body.IsInGroup("enemies"))
            {
                EmitSignal(nameof(EnemyDetected), this, body);
            }
        }

        private void OnInventory_Pressed()
        {
            EmitSignal(nameof(InventoryButtonPressed), this);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("open_chest"))
            {
                var chests = _chestDetector.GetOverlappingBodies();
                if (chests.Count > 0)
                {
                    Node2D closestChest = null;
                    var closestDistance = float.MaxValue;
                    foreach (Node2D chest in chests)
                    {
                        var distance = Position.DistanceTo(chest.Position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestChest = chest;
                        }
                    }

                    EmitSignal(nameof(OpenChestInputReceived), closestChest);
                }
            }

            if (Input.IsActionJustPressed("guild_hall_desk_interact"))
            {
                var desks = _deskDetector.GetOverlappingBodies();
                if (desks.Count == 0)
                {
                    return;
                }

                var desk = desks[0];
                EmitSignal(nameof(GuildHallDeskInputReceived), desk);
            }
        }
    }
}