using System;
using System.Collections.Generic;
using Godot;
using HeroesGuild.Entities.Player;
using HeroesGuild.UI.inventory;
using HeroesGuild.UI.inventory.item_info;
using Array = Godot.Collections.Array;

namespace HeroesGuild.UI.pause_menu
{
    public class PauseMenu : CanvasLayer
    {
        public enum Menu
        {
            Settings,
            Info,
            Inventory
        }

        private static readonly Color LightGreen = new Color("35AF35");
        private static readonly Color DarkGreen = new Color("1B651B");
        private static readonly Color LightRed = new Color("BC3535");
        private static readonly Color DarkRed = new Color("661E25");
        private static readonly Color LightBlue = new Color("3535C9");
        private static readonly Color DarkBlue = new Color("1B1B65");

        private static Color GetMenuHeaderColor(Menu menu)
        {
            return menu switch
            {
                Menu.Settings => LightGreen,
                Menu.Info => LightRed,
                Menu.Inventory => LightBlue,
                _ => throw new ArgumentException()
            };
        }

        private static Color GetMenuButtonNormalColor(Menu menu)
        {
            return menu switch
            {
                Menu.Settings => DarkGreen,
                Menu.Info => DarkRed,
                Menu.Inventory => DarkBlue,
                _ => throw new ArgumentException()
            };
        }

        private static Color GetMenuButtonPressedColor(Menu menu)
        {
            return menu switch
            {
                Menu.Settings => LightGreen,
                Menu.Info => LightRed,
                Menu.Inventory => LightBlue,
                _ => throw new ArgumentException()
            };
        }

        private static readonly Dictionary<Menu, Color> MenuCoinDecorColor = new Dictionary<Menu, Color>
        {
            {Menu.Info, DarkRed}, {Menu.Inventory, DarkBlue}
        };


        private static Texture GetMenuBackground(Menu menu)
        {
            var path = menu switch
            {
                Menu.Settings => "res://UI/Settings/background.png",
                Menu.Info => "res://UI/information/background.png",
                Menu.Inventory => "res://UI/inventory/background.png",
                _ => throw new ArgumentException()
            };

            return ResourceLoader.Load<Texture>(path);
        }

        public static readonly PackedScene InventoryItemScene =
            ResourceLoader.Load<PackedScene>("res://UI/inventory/InventoryItem.tscn");
        
        
        public Menu currentMenu = Menu.Inventory;
        public Player playerInstance;

        public Control pauseMenuControl;
        private MarginContainer _pauseMenuMargin;
        private VBoxContainer _pauseMenuVBox;
        private MarginContainer _inventoryItemsMargin;
        private ScrollContainer _inventoryItemsGrid;
        private BoxContainer _topHBox;
        private BoxContainer _buttons;
        private TextureButton _settingsButton;
        private TextureButton _infoButton;
        private TextureButton _inventoryButton;
        private MarginContainer _headerMargin;
        private Label _headerLabel;
        private TextureRect _headerDecor;
        private BoxContainer _secondRowHBox;
        private MarginContainer _coinsMargin;
        private Label _coinsLabel;
        private TextureRect _coinsDecor;
        private TextureRect _background;
        private MarginContainer _equippedItemsMargin;
        private HBoxContainer _equippedItemsHBox;
        private InventoryItem _equippedWeapon;
        private InventoryItem _equippedArmor;
        private ItemStatPopUp _itemStatsPopup;
        private ItemInfoMenu _itemInfoMenu;
        private MarginContainer _settingsMargin;

        public override void _Ready()
        {
            pauseMenuControl = GetNode<Control>("PauseMenuControl");
            _pauseMenuMargin = pauseMenuControl.GetNode<MarginContainer>("PauseMenuMargin");
            _pauseMenuVBox = _pauseMenuMargin.GetNode<VBoxContainer>("VBoxContainer");
            _inventoryItemsMargin = _pauseMenuVBox.GetNode<MarginContainer>("InventoryItemsMargin");
            _inventoryItemsGrid = _inventoryItemsMargin.GetNode<ScrollContainer>("InventoryItemsScrollContainer");
            _topHBox = _pauseMenuVBox.GetNode<BoxContainer>("Top");
            _buttons = _topHBox.GetNode<BoxContainer>("Buttons");
            _settingsButton = _buttons.GetNode<TextureButton>("Settings");
            _infoButton = _buttons.GetNode<TextureButton>("Info");
            _inventoryButton = _buttons.GetNode<TextureButton>("Inventory");
            _headerMargin = _topHBox.GetNode<MarginContainer>("MarginContainer");
            _headerLabel = _headerMargin.GetNode<Label>("Header");
            _headerDecor = _headerMargin.GetNode<TextureRect>("HeaderDecor");
            _secondRowHBox = _pauseMenuVBox.GetNode<BoxContainer>("SecondRow");
            _coinsMargin = _secondRowHBox.GetNode<MarginContainer>("Coins");
            _coinsLabel = _coinsMargin.GetNode<Label>("CoinAmount");
            _coinsDecor = _coinsMargin.GetNode<TextureRect>("CoinsDecor");
            _background = _pauseMenuMargin.GetNode<TextureRect>("Background");
            _equippedItemsMargin = _secondRowHBox.GetNode<MarginContainer>("EquippedMargin");
            _equippedItemsHBox = _equippedItemsMargin.GetNode<HBoxContainer>("Equipped");
            _equippedWeapon = _equippedItemsHBox.GetNode<InventoryItem>("EquippedWeapon");
            _equippedArmor = _equippedItemsHBox.GetNode<InventoryItem>("EquippedArmor");
            _itemStatsPopup = pauseMenuControl.GetNode<ItemStatPopUp>("ItemStatsPopup");
            _itemInfoMenu = _pauseMenuMargin.GetNode<ItemInfoMenu>("ItemInfoMenu");
            _settingsMargin = _pauseMenuVBox.GetNode<MarginContainer>("SettingsMargin");

            pauseMenuControl.Visible = false;
            _itemStatsPopup.RectPosition = new Vector2(_itemStatsPopup.RectPosition.x, -19);
            foreach (BaseButton button in _buttons.GetChildren())
            {
                button.Connect("button_down", this, nameof(OnButton_ButtonDown), new Array {button});
                button.Connect("button_up", this, nameof(OnButton_ButtonUp), new Array {button});
            }
        }

        private void OnPlayerInventoryButton_Pressed(Player player)
        {
            TogglePause(player);
        }

        public void TogglePause(Player player)
        {
            playerInstance = player;
            var paused = GetTree().Paused;
            paused = GetTree().Paused = !paused;
            pauseMenuControl.Visible = paused;
            playerInstance.hudMargin.Visible = !paused;
            if (paused)
            {
                UpdateInventory();
                UpdateEquippedItem();
                UpdateMenuState();
                _coinsLabel.Text = $"{playerInstance.Coins}:COIN";
            }
        }

        public void UpdateInventory()
        {
            foreach (Node child in _inventoryItemsGrid.GetChildren())
            {
                child.QueueFree();
            }

            var inventory = new List<string>(playerInstance.Inventory);
            inventory.Remove(playerInstance.EquippedWeapon);
            inventory.Remove(playerInstance.EquippedArmor);
            foreach (var item in inventory)
            {
                var inventoryItem = (InventoryItem) InventoryItemScene.Instance();
                inventoryItem.itemName = item;
                var itemTextureButton = inventoryItem.GetNode<TextureButton>("MarginContainer/Item");
                itemTextureButton.TextureNormal = inventoryItem.InventoryItemResource;
                inventoryItem.Connect("mouse_entered", _itemStatsPopup, nameof(_itemStatsPopup.OnItem_MouseEntered),
                    new Array {item});
                inventoryItem.Connect("mouse_exited", _itemStatsPopup, nameof(_itemStatsPopup.OnItem_MouseExited));
                itemTextureButton.Connect("pressed", _itemInfoMenu, nameof(_itemInfoMenu.OnItem_Pressed), new
                    Array {inventoryItem, playerInstance});
                _inventoryItemsGrid.AddChild(inventoryItem);
            }
        }

        private void UpdateEquippedItem()
        {
            if (playerInstance.EquippedWeapon == null)
            {
                return;
            }

            _equippedWeapon.itemName = playerInstance.EquippedWeapon;
            if (playerInstance.EquippedWeapon != "")
            {
                var equippedWeaponTextureButton = _equippedWeapon.GetNode<TextureButton>("MarginContainer/Item");
                equippedWeaponTextureButton.TextureNormal = _equippedWeapon.InventoryItemResource;
            }

            foreach (InventoryItem equippedItem in _equippedItemsHBox.GetChildren())
            {
                var itemButton = equippedItem.GetNode<TextureButton>("MarginContainer/Item");
                equippedItem.Visible = !string.IsNullOrWhiteSpace(equippedItem.itemName);

                if (equippedItem.IsConnected("mouse_entered", _itemStatsPopup,
                    nameof(_itemStatsPopup.OnItem_MouseEntered)))
                {
                    equippedItem.Disconnect("mouse_entered", _itemStatsPopup,
                        nameof(_itemStatsPopup.OnItem_MouseEntered));
                }

                equippedItem.Connect("mouse_entered", _itemStatsPopup, nameof(_itemStatsPopup.OnItem_MouseEntered), new
                    Array {equippedItem.itemName});

                if (equippedItem.IsConnected("mouse_exited", _itemStatsPopup, nameof(_itemStatsPopup.OnItem_MouseExited)))
                {
                    equippedItem.Disconnect("mouse_exited", _itemStatsPopup, nameof(_itemStatsPopup.OnItem_MouseExited));
                }

                equippedItem.Connect("mouse_exited", _itemStatsPopup, nameof(_itemStatsPopup.OnItem_MouseExited));

                if (itemButton.IsConnected("pressed", _itemInfoMenu, nameof(_itemInfoMenu.OnItem_Pressed)))
                {
                    itemButton.Disconnect("pressed", _itemInfoMenu, nameof(_itemInfoMenu.OnItem_Pressed));
                }

                itemButton.Connect("pressed", _itemInfoMenu, nameof(_itemInfoMenu.OnItem_Pressed), new
                    Array {equippedItem, playerInstance});
            }
        }

        private void UpdateMenuState()
        {
            switch (currentMenu)
            {
                case Menu.Settings:
                    _settingsButton.Pressed = true;
                    _infoButton.Pressed = false;
                    _inventoryButton.Pressed = false;
                    _headerLabel.Text = "Settings";
                    _coinsMargin.Visible = false;
                    _inventoryItemsMargin.Visible = false;
                    _equippedItemsMargin.Visible = false;
                    _itemStatsPopup.Visible = false;
                    _settingsMargin.Visible = true;
                    _secondRowHBox.Visible = false;
                    break;
                case Menu.Info:
                    _settingsButton.Pressed = false;
                    _infoButton.Pressed = true;
                    _inventoryButton.Pressed = false;
                    _headerLabel.Text = "Info";
                    _coinsMargin.Visible = true;
                    _inventoryItemsMargin.Visible = false;
                    _equippedItemsMargin.Visible = false;
                    _itemStatsPopup.Visible = false;
                    _settingsMargin.Visible = false;
                    _secondRowHBox.Visible = true;
                    break;
                case Menu.Inventory:
                    _settingsButton.Pressed = false;
                    _infoButton.Pressed = false;
                    _inventoryButton.Pressed = true;
                    _headerLabel.Text = "Inventory";
                    _coinsMargin.Visible = false;
                    _inventoryItemsMargin.Visible = true;
                    _equippedItemsMargin.Visible = true;
                    _itemStatsPopup.Visible = true;
                    _settingsMargin.Visible = false;
                    _secondRowHBox.Visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _background.Texture = GetMenuBackground(currentMenu);
            foreach (BaseButton button in _buttons.GetChildren())
            {
                button.Modulate = button.Pressed
                    ? GetMenuButtonPressedColor(currentMenu)
                    : GetMenuButtonNormalColor(currentMenu);
                _headerDecor.Modulate = GetMenuHeaderColor(currentMenu);
                if (MenuCoinDecorColor.ContainsKey(currentMenu))
                {
                    _coinsDecor.Modulate = MenuCoinDecorColor[currentMenu];
                }
            }
        }

        private void OnButton_ButtonDown(BaseButton button)
        {
            currentMenu = button switch
            {
                var b when b == _settingsButton => Menu.Settings,
                var b when b == _infoButton => Menu.Info,
                var b when b == _inventoryButton => Menu.Inventory,
                _ => throw new ArgumentOutOfRangeException()
            };
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, null, -15);
            UpdateMenuState();
        }

        private void OnButton_ButtonUp(BaseButton button)
        {
            button.Pressed = true;
        }

        private void OnItemInfoMenu_EquippedArmorChanged()
        {
            UpdateEquippedItem();
            UpdateInventory();
        }

        private void OnItemInfoMenu_EquippedWeaponChanged()
        {
            UpdateEquippedItem();
            UpdateInventory();
        }

        private void OnCombat_BagOpened(Player playerInstance)
        {
            TogglePauseCombat(playerInstance);
        }

        public void TogglePauseCombat(Player player)
        {
            playerInstance = player;
            pauseMenuControl.Visible = !pauseMenuControl.Visible;
            playerInstance.hudMargin.Visible = false;
            UpdateInventory();
            UpdateEquippedItem();
            UpdateMenuState();
            _coinsLabel.Text = $"{playerInstance.Coins}:COIN";
        }
    }
}