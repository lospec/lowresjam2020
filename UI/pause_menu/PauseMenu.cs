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

        public PackedScene InventoryItemScene =
            ResourceLoader.Load<PackedScene>("res://UI/inventory/InventoryItem.tscn");
        public Menu CurrentMenu = Menu.Inventory;
        public Player PlayerInstance;

        public Control pauseMenuControl;
        private MarginContainer pauseMenuMargin;
        private VBoxContainer pauseMenuVBox;
        private MarginContainer inventoryItemsMargin;
        private ScrollContainer inventoryItemsGrid;
        private BoxContainer topHBox;
        private BoxContainer buttons;
        private TextureButton settingsButton;
        private TextureButton infoButton;
        private TextureButton inventoryButton;
        private MarginContainer headerMargin;
        private Label headerLabel;
        private TextureRect headerDecor;
        private BoxContainer secondRowHBox;
        private MarginContainer coinsMargin;
        private Label coinsLabel;
        private TextureRect coinsDecor;
        private TextureRect background;
        private MarginContainer equippedItemsMargin;
        private HBoxContainer equippedItemsHBox;
        private InventoryItem equippedWeapon;
        private InventoryItem equippedArmor;
        private ItemStatPopUp itemStatsPopup;
        private ItemInfoMenu itemInfoMenu;
        private MarginContainer settingsMargin;

        public override void _Ready()
        {
            pauseMenuControl = GetNode<Control>("PauseMenuControl");
            pauseMenuMargin = pauseMenuControl.GetNode<MarginContainer>("PauseMenuMargin");
            pauseMenuVBox = pauseMenuMargin.GetNode<VBoxContainer>("VBoxContainer");
            inventoryItemsMargin = pauseMenuVBox.GetNode<MarginContainer>("InventoryItemsMargin");
            inventoryItemsGrid = inventoryItemsMargin.GetNode<ScrollContainer>("InventoryItemsScrollContainer");
            topHBox = pauseMenuVBox.GetNode<BoxContainer>("Top");
            buttons = topHBox.GetNode<BoxContainer>("Buttons");
            settingsButton = buttons.GetNode<TextureButton>("Settings");
            infoButton = buttons.GetNode<TextureButton>("Info");
            inventoryButton = buttons.GetNode<TextureButton>("Inventory");
            headerMargin = topHBox.GetNode<MarginContainer>("MarginContainer");
            headerLabel = headerMargin.GetNode<Label>("Header");
            headerDecor = headerMargin.GetNode<TextureRect>("HeaderDecor");
            secondRowHBox = pauseMenuVBox.GetNode<BoxContainer>("SecondRow");
            coinsMargin = secondRowHBox.GetNode<MarginContainer>("Coins");
            coinsLabel = coinsMargin.GetNode<Label>("CoinAmount");
            coinsDecor = coinsMargin.GetNode<TextureRect>("CoinsDecor");
            background = pauseMenuMargin.GetNode<TextureRect>("Background");
            equippedItemsMargin = secondRowHBox.GetNode<MarginContainer>("EquippedMargin");
            equippedItemsHBox = equippedItemsMargin.GetNode<HBoxContainer>("Equipped");
            equippedWeapon = equippedItemsHBox.GetNode<InventoryItem>("EquippedWeapon");
            equippedArmor = equippedItemsHBox.GetNode<InventoryItem>("EquippedArmor");
            itemStatsPopup = pauseMenuControl.GetNode<ItemStatPopUp>("ItemStatsPopup");
            itemInfoMenu = pauseMenuMargin.GetNode<ItemInfoMenu>("ItemInfoMenu");
            settingsMargin = pauseMenuVBox.GetNode<MarginContainer>("SettingsMargin");

            pauseMenuControl.Visible = false;
            itemStatsPopup.RectPosition = new Vector2(itemStatsPopup.RectPosition.x, -19);
            foreach (BaseButton button in buttons.GetChildren())
            {
                button.Connect("button_down", this, nameof(OnButton_ButtonDown), new Array {button});
                button.Connect("button_up", this, nameof(OnButton_ButtonUp), new Array {button});
            }
        }

        private void OnPlayerInventory_ButtonPressed(Player player)
        {
            TogglePause(player);
        }

        private void TogglePause(Player player)
        {
            PlayerInstance = player;
            var paused = GetTree().Paused;
            paused = GetTree().Paused = !paused;
            pauseMenuControl.Visible = paused;
            PlayerInstance.hudMargin.Visible = !paused;
            if (paused)
            {
                UpdateInventory();
                UpdateEquippedItem();
                UpdateMenuState();
                coinsLabel.Text = $"{PlayerInstance.Coins}:COIN";
            }
        }

        public void UpdateInventory()
        {
            foreach (Node child in inventoryItemsGrid.GetChildren())
            {
                child.QueueFree();
            }

            var inventory = new List<string>(PlayerInstance.Inventory);
            inventory.Remove(PlayerInstance.EquippedWeapon);
            inventory.Remove(PlayerInstance.EquippedArmor);
            foreach (var item in inventory)
            {
                var inventoryItem = (InventoryItem) InventoryItemScene.Instance();
                inventoryItem.ItemName = item;
                var itemTextureButton = inventoryItem.GetNode<TextureButton>("MarginContainer/Item");
                itemTextureButton.TextureNormal = inventoryItem.InventoryItemResource;
                inventoryItem.Connect("mouse_entered", itemStatsPopup, nameof(itemStatsPopup.OnItem_MouseEntered),
                    new Array {item});
                inventoryItem.Connect("mouse_exited", itemStatsPopup, nameof(itemStatsPopup.OnItem_MouseExited));
                itemTextureButton.Connect("pressed", itemInfoMenu, nameof(itemInfoMenu.OnItem_Pressed), new
                    Array {inventoryItem, PlayerInstance});
                inventoryItemsGrid.AddChild(inventoryItem);
            }
        }

        private void UpdateEquippedItem()
        {
            if (PlayerInstance.EquippedWeapon == null)
            {
                return;
            }

            equippedWeapon.ItemName = PlayerInstance.EquippedWeapon;
            if (PlayerInstance.EquippedWeapon != "")
            {
                var equippedWeaponTextureButton = equippedWeapon.GetNode<TextureButton>("MarginContainer/Item");
                equippedWeaponTextureButton.TextureNormal = equippedWeapon.InventoryItemResource;
            }

            foreach (InventoryItem equippedItem in equippedItemsHBox.GetChildren())
            {
                var itemButton = equippedItem.GetNode<TextureButton>("MarginContainer/Item");
                equippedItem.Visible = !string.IsNullOrWhiteSpace(equippedItem.ItemName);

                if (equippedItem.IsConnected("mouse_entered", itemStatsPopup,
                    nameof(itemStatsPopup.OnItem_MouseEntered)))
                {
                    equippedItem.Disconnect("mouse_entered", itemStatsPopup,
                        nameof(itemStatsPopup.OnItem_MouseEntered));
                }

                equippedItem.Connect("mouse_entered", itemStatsPopup, nameof(itemStatsPopup.OnItem_MouseEntered), new
                    Array {equippedItem.ItemName});

                if (equippedItem.IsConnected("mouse_exited", itemStatsPopup, nameof(itemStatsPopup.OnItem_MouseExited)))
                {
                    equippedItem.Disconnect("mouse_exited", itemStatsPopup, nameof(itemStatsPopup.OnItem_MouseExited));
                }

                equippedItem.Connect("mouse_exited", itemStatsPopup, nameof(itemStatsPopup.OnItem_MouseExited));

                if (itemButton.IsConnected("pressed", itemInfoMenu, nameof(itemInfoMenu.OnItem_Pressed)))
                {
                    itemButton.Disconnect("pressed", itemInfoMenu, nameof(itemInfoMenu.OnItem_Pressed));
                }

                itemButton.Connect("pressed", itemInfoMenu, nameof(itemInfoMenu.OnItem_Pressed), new
                    Array {equippedItem, PlayerInstance});
            }
        }

        private void UpdateMenuState()
        {
            switch (CurrentMenu)
            {
                case Menu.Settings:
                    settingsButton.Pressed = true;
                    infoButton.Pressed = false;
                    inventoryButton.Pressed = false;
                    headerLabel.Text = "Settings";
                    coinsMargin.Visible = false;
                    inventoryItemsMargin.Visible = false;
                    equippedItemsMargin.Visible = false;
                    itemStatsPopup.Visible = false;
                    settingsMargin.Visible = true;
                    secondRowHBox.Visible = false;
                    break;
                case Menu.Info:
                    settingsButton.Pressed = false;
                    infoButton.Pressed = true;
                    inventoryButton.Pressed = false;
                    headerLabel.Text = "Info";
                    coinsMargin.Visible = true;
                    inventoryItemsMargin.Visible = false;
                    equippedItemsMargin.Visible = false;
                    itemStatsPopup.Visible = false;
                    settingsMargin.Visible = false;
                    secondRowHBox.Visible = true;
                    break;
                case Menu.Inventory:
                    settingsButton.Pressed = false;
                    infoButton.Pressed = false;
                    inventoryButton.Pressed = true;
                    headerLabel.Text = "Inventory";
                    coinsMargin.Visible = false;
                    inventoryItemsMargin.Visible = true;
                    equippedItemsMargin.Visible = true;
                    itemStatsPopup.Visible = true;
                    settingsMargin.Visible = false;
                    secondRowHBox.Visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            background.Texture = GetMenuBackground(CurrentMenu);
            foreach (BaseButton button in buttons.GetChildren())
            {
                button.Modulate = button.Pressed
                    ? GetMenuButtonPressedColor(CurrentMenu)
                    : GetMenuButtonNormalColor(CurrentMenu);
                headerDecor.Modulate = GetMenuHeaderColor(CurrentMenu);
                if (MenuCoinDecorColor.ContainsKey(CurrentMenu))
                {
                    coinsDecor.Modulate = MenuCoinDecorColor[CurrentMenu];
                }
            }
        }

        private void OnButton_ButtonDown(BaseButton button)
        {
            CurrentMenu = button switch
            {
                var b when b == settingsButton => Menu.Settings,
                var b when b == infoButton => Menu.Info,
                var b when b == inventoryButton => Menu.Inventory,
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
            PlayerInstance = player;
            pauseMenuControl.Visible = !pauseMenuControl.Visible;
            PlayerInstance.hudMargin.Visible = false;
            UpdateInventory();
            UpdateEquippedItem();
            UpdateMenuState();
            coinsLabel.Text = $"{PlayerInstance.Coins}:COIN";
        }
    }
}