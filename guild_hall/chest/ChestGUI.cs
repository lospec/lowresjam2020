using System.Collections.Generic;
using System.Linq;
using Godot;
using HeroesGuild.Entities.Player;
using HeroesGuild.Utility;

namespace HeroesGuild.guild_hall.chest
{
    public class ChestGUI : Node
    {
        private static readonly PackedScene ChestItemResource =
            ResourceLoader.Load<PackedScene>("res://guild_hall/chest/chest_item.tscn");

        private Player _playerInstance;

        private Chest _chestInstance;
        private int _openedChestID = -1;

        private ChestItems _heldChestItemInstance;
        private ChestItems _heldInventoryItemInstance;

        private SceneTree _tree;
        private MarginContainer _margin;
        private GridContainer _inventoryContents;
        private GridContainer _chestContents;
        private TextureRect _draggableItem;

        public override void _Ready()
        {
            _tree = GetTree();
            _margin = GetNode<MarginContainer>("MarginContainer");
            _inventoryContents = GetNode<GridContainer>(
                "MarginContainer/MarginContainer/HBoxContainer/InventoryScroll/InventoryContents");
            _chestContents = GetNode<GridContainer>(
                "MarginContainer/MarginContainer/HBoxContainer/ChestContents");
            _draggableItem = GetNode<TextureRect>("DraggableItem");

            _margin.Visible = false;
            foreach (ChestItems chestItems in _chestContents.GetChildren())
            {
                chestItems.Connect(nameof(ChestItems.LeftClicked), this,
                    nameof(OnChestItem_LeftClicked));
            }
        }

        private async void UpdateInventoryUI()
        {
            if (_chestInstance == null)
            {
                GD.PushError("Chest instance not found");
                return;
            }

            var movableInventory = new List<string>(_playerInstance.Inventory);
            movableInventory.Remove(_playerInstance.EquippedWeapon);
            movableInventory.Remove(_playerInstance.EquippedArmor);

            foreach (Node child in _inventoryContents.GetChildren())
            {
                child.QueueFree();
            }

            var childCount = _inventoryContents.GetChildCount();
            if (childCount > 0)
            {
                await ToSignal(_inventoryContents.GetChild(childCount - 1),
                    "tree_exited");
            }

            foreach (var itemName in movableInventory)
            {
                var inventoryItem = (ChestItems) ChestItemResource.Instance();
                inventoryItem.ItemName = itemName;
                _inventoryContents.AddChild(inventoryItem);
            }

            for (var i = 0; i < 8; i++)
            {
                var inventoryItem = (ChestItems) ChestItemResource.Instance();
                _inventoryContents.AddChild(inventoryItem);
            }

            foreach (ChestItems inventoryItems in _inventoryContents.GetChildren())
            {
                inventoryItems.Connect(nameof(ChestItems.LeftClicked), this,
                    nameof(OnInventoryItem_LeftClicked));
            }
        }


        public void Open(Player playerInstance, Chest chestInstance)
        {
            _playerInstance = playerInstance;
            _chestInstance = chestInstance;

            _openedChestID = chestInstance.chestID;
            _chestInstance.contents =
                Autoload.Get<SaveData>().ChestContent[_openedChestID];
            _tree.Paused = true;
            _playerInstance.hudMargin.Visible = false;
            UpdateChestUI();
            UpdateInventoryUI();
            _margin.Visible = true;
        }

        public void Close()
        {
            _margin.Visible = false;
            Autoload.Get<SaveData>().ChestContent[_openedChestID] =
                _chestInstance.contents;
            _openedChestID = -1;
            _chestInstance = null;

            _tree.Paused = false;
            _playerInstance.hudMargin.Visible = true;
        }

        private void UpdateChestUI()
        {
            if (_chestInstance == null)
            {
                GD.PushError("Chest instance not found");
                return;
            }

            for (var i = 0; i < _chestInstance.contents.Count; i++)
            {
                var itemName = _chestInstance.contents[i];
                _chestContents.GetChild<ChestItems>(i).ItemName = itemName;
            }
        }

        private void OnInventoryItem_LeftClicked(ChestItems inventoryItemInstance)
        {
            _heldInventoryItemInstance = inventoryItemInstance;
            _heldInventoryItemInstance.itemCenter.Visible = false;
            _draggableItem.Texture = Utility.Utility.GetInventoryItemResource
                (_heldInventoryItemInstance.ItemName);
        }

        private void OnChestItem_LeftClicked(ChestItems chestItemInstance)
        {
            _heldChestItemInstance = chestItemInstance;
            _heldChestItemInstance.itemCenter.Visible = false;
            _draggableItem.Texture = Utility.Utility.GetInventoryItemResource
                (_heldChestItemInstance.ItemName);
        }

        public override void _Process(float delta)
        {
            if (_heldChestItemInstance != null)
            {
                _draggableItem.RectGlobalPosition =
                    _draggableItem.GetGlobalMousePosition() -
                    _heldInventoryItemInstance.itemCenter.RectSize / 2;
            }

            if (_heldInventoryItemInstance != null)
            {
                _draggableItem.RectGlobalPosition =
                    _draggableItem.GetGlobalMousePosition() -
                    _heldInventoryItemInstance.itemCenter.RectSize / 2;
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton
                .ButtonIndex == (int) ButtonList.Left && !mouseButton.IsPressed())
            {
                var mousePosition = _draggableItem.GetGlobalMousePosition();
                if (_heldChestItemInstance != null && _heldChestItemInstance != null)
                {
                    GD.PushError(
                        "Found both held chest item instance and held inventory item instance");
                }
                else if (_heldChestItemInstance != null)
                {
                    StopDragginFromChest(mousePosition);
                }
                else if (_heldInventoryItemInstance != null)
                {
                    StopDraggingFromInventory(mousePosition);
                }
            }
        }

        private void StopDragginFromChest(Vector2 stopPosition)
        {
            _draggableItem.Texture = null;
            var chestSlot = GetEmptySlotAtPosition(stopPosition, _chestContents);
            if (chestSlot != null)
            {
                var newSlotIndex = chestSlot.GetIndex();
                _chestInstance.contents[newSlotIndex] = _heldChestItemInstance.ItemName;
                var slotIndex = _heldChestItemInstance.GetIndex();
                _chestInstance.contents[slotIndex] = string.Empty;

                UpdateChestUI();
            }
            else
            {
                var inventorySlot =
                    GetEmptySlotAtPosition(stopPosition, _inventoryContents);
                if (inventorySlot != null)
                {
                    _playerInstance.Inventory.Add(_heldChestItemInstance.ItemName);
                    UpdateInventoryUI();

                    var slotIndex = _heldChestItemInstance.GetIndex();
                    _chestInstance.contents[slotIndex] = string.Empty;
                    UpdateChestUI();
                }
            }

            _heldChestItemInstance.itemCenter.Visible = true;
            _heldChestItemInstance = null;
        }


        private void StopDraggingFromInventory(Vector2 stopPosition)
        {
            _draggableItem.Texture = null;
            var chestSlot = GetEmptySlotAtPosition(stopPosition, _chestContents);
            if (chestSlot != null)
            {
                var newSlotIndex = chestSlot.GetIndex();
                _chestInstance.contents[newSlotIndex] =
                    _heldInventoryItemInstance.ItemName;
                UpdateChestUI();

                _playerInstance.Inventory.Remove(_heldInventoryItemInstance.ItemName);
                UpdateInventoryUI();
            }

            _heldInventoryItemInstance.itemCenter.Visible = true;
            _heldInventoryItemInstance = null;
        }

        private static ChestItems GetEmptySlotAtPosition(Vector2 position,
            Node grid)
        {
            return grid.GetChildren().Cast<ChestItems>()
                .FirstOrDefault(slot =>
                    slot.GetGlobalRect().HasPoint(position) &&
                    string.IsNullOrWhiteSpace(slot.ItemName));
        }
    }
}