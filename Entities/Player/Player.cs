using System.Collections.Generic;
using Godot;
using HeroesGuild.Utility;

namespace HeroesGuild.Entities.Player
{
    public class Player : BaseEntity.BaseEntity
    {
        [Signal] public delegate void EnemyDetected(BaseEntity.BaseEntity player, BaseEntity.BaseEntity enemy);

        [Signal] public delegate void InventoryButtonPressed(BaseEntity.BaseEntity player);

        // TODO: Change Node to more specific type
        [Signal] public delegate void OpenChestInputReceived(Node2D chest);

        // TODO: Change Node to more specific type
        [Signal] public delegate void GuildHallDeskInputReceived(Node2D desk);

        public int Coins { get; set; }
        public List<string> Inventory { get; set; } = new List<string>();
        public string EquippedWeapon { get; set; }
        public string EquippedArmor { get; set; }

        public MarginContainer hudMargin;
        private Label hudHealthLabel;
        private Area2D chestDetector;
        private Area2D deskDetector;
        private Camera2D camera;
        private Area2D collisionDetector;

        // TODO: Bird/Cloud System
        private Node birdSystem;
        private Node cloudSystem;

        public override void _Ready()
        {
            hudMargin = GetNode<MarginContainer>("HUD/MarginContainer");
            hudHealthLabel = GetNode<Label>("HUD/MarginContainer/HealthMargin/MarginContainer/HBoxContainer/Health");
            chestDetector = GetNode<Area2D>("ChestDetector");
            deskDetector = GetNode<Area2D>("DeskDetector");
            camera = GetNode<Camera2D>("Camera2D");
            collisionDetector = GetNode<Area2D>("CollisionDetector");
            // TODO: Bird/Cloud System
            birdSystem = GetNode<Node>("BirdsSystem");
            cloudSystem = GetNode<Node>("CloudsSystem");

            var saveData = Singleton.Get<SaveData>(this);
            Coins = saveData.Coins;
            Inventory = saveData.Inventory;
            EquippedWeapon = saveData.EquippedWeapon;
            EquippedArmor = saveData.EquippedArmor;
            MaxHealth = saveData.MaxHealth;
            Health = saveData.Health;

            hudHealthLabel.Text = $"{Health}/{MaxHealth}";
            var texture =
                ResourceLoader.Load<Texture>(
                    $"res://Entities/Player/spritesheets/{saveData.CharacterName}_Overworld.png");
            sprite.Texture = texture;
        }

        private void OnPlayerTree_Exiting()
        {
            var saveData = Singleton.Get<SaveData>(this);
            saveData.Coins = Coins;
            saveData.Inventory = Inventory;
            saveData.EquippedWeapon = EquippedWeapon;
            saveData.EquippedArmor = EquippedArmor;
            saveData.MaxHealth = MaxHealth;
            saveData.Health = Health;
        }

        public override void _PhysicsProcess(float delta)
        {
            var inputVelocity = Vector2.Zero;
            inputVelocity.x = Input.GetActionStrength("player_move_right")
                              - Input.GetActionStrength("player_move_left");
            inputVelocity.y = Input.GetActionStrength("player_move_down")
                              - Input.GetActionStrength("player_move_up");
            Velocity = inputVelocity.Normalized() * MoveSpeed;
        }

        public override void _Process(float delta)
        {
            hudHealthLabel.Text = $"{Health}/{MaxHealth}";
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
                var chests = chestDetector.GetOverlappingBodies();
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
                var desks = deskDetector.GetOverlappingBodies();
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