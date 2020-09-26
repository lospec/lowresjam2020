using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HeroesGuild.Data;
using HeroesGuild.Entities.Player;
using HeroesGuild.UI.Inventory;
using HeroesGuild.Utility;
using Array = Godot.Collections.Array;

namespace HeroesGuild.GuildHall.GuildInterface
{
    public class GuildInterface : CanvasLayer
    {
        private static readonly PackedScene PopupItemScene = ResourceLoader
            .Load<PackedScene>("res://UI/inventory/InventoryItem.tscn");

        public enum Menu
        {
            Deposit,
            Sell,
            Market
        }

        private static Texture GetMenuBackgrounds(Menu menu)
        {
            var path = menu switch
            {
                Menu.Deposit =>
                    "res://guild_hall/guild_interface/deposit/background.png",
                Menu.Sell => "res://guild_hall/guild_interface/sell/background.png",
                Menu.Market => "res://guild_hall/guild_interface/market/background.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        private static readonly Color LightOrange = new Color("C97F35");
        private static readonly Color DarkOrange = new Color("661E25");
        private static readonly Color LightCyan = new Color("35B78F");
        private static readonly Color DarkCyan = new Color("1B6E83");
        private static readonly Color LightPurple = new Color("7F35C9");
        private static readonly Color DarkPurple = new Color("553361");

        private static Color GetMenuButtonNormalColor(Menu menu) => menu switch
        {
            Menu.Deposit => DarkOrange,
            Menu.Sell => DarkCyan,
            Menu.Market => DarkPurple,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static Color GetMenuButtonPressedColor(Menu menu) => menu switch
        {
            Menu.Deposit => LightOrange,
            Menu.Sell => LightCyan,
            Menu.Market => LightPurple,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static Color GetMenuHeaderDecorColor(Menu menu) => menu switch
        {
            Menu.Deposit => LightOrange,
            Menu.Sell => LightCyan,
            Menu.Market => LightPurple,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static Color GetMenuCoinDecorColor(Menu menu) => menu switch
        {
            Menu.Deposit => LightOrange,
            Menu.Sell => LightCyan,
            Menu.Market => LightPurple,
            _ => throw new ArgumentOutOfRangeException()
        };

        private static readonly int[] DepositAmounts =
        {
            1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000
        };

        private const int MAX_MARKET_ITEMS = 12;
        private const int MAX_MARKET_PRICE_LEVEL_MULTIPLIER = 25;

        [Signal] public delegate void GuildHallLevelUp();


        public Menu currentMenu;
        public InventoryItem selectedItem;
        public InventoryItem selectedMarketItems;

        private Player _playerInstance;
        private int _depositAmountIndex = 0;

        private SceneTree _tree;

        private MarginContainer _margin;
        private TextureRect _background;
        private Node _vbox;
        private Node _top;
        private Node _buttons;
        private BaseButton _depositTabButton;
        private BaseButton _sellTabButton;
        private BaseButton _shopTabButton;
        private Node _headerMargin;
        private TextureRect _headerDecor;
        private Label _headerLabel;
        private Node _secondRow;
        private Node _coinsMargin;
        private TextureRect _coinsDecor;
        private Label _coinsLabel;
        private MarginContainer _inventoryMargin;
        private Node _inventoryScroll;
        private Node _inventoryGrid;
        private MarginContainer _itemPriceMargin;
        private MarginContainer _marketItemPriceMargin;
        private MarginContainer _depositMargin;
        private Node _depositVbox;
        private Node _depositHBox;
        private Node _depositButtonMargin;
        private Node _depositButtonVbox;
        private Button _depositButton;
        private Node _depositProgressVbox;
        private Label _nextLevelLabel;
        private Label _nextLevelProgressText;
        private Node _wholeProgressBarMargin;
        private Node _progressBarMargin;
        private TextureProgress _depositProgressBar;
        private MarginContainer _marketMargin;
        private Node _marketItemsGrid;

        public override void _Ready()
        {
            _tree = GetTree();

            _margin = GetNode<MarginContainer>("Margin");
            _background = _margin.GetNode<TextureRect>("Background");
            _vbox = _margin.GetNode<Node>("VBoxContainer");
            _top = _vbox.GetNode<Node>("Top");
            _buttons = _top.GetNode<Node>("Buttons");
            _depositTabButton = _buttons.GetNode<BaseButton>("Deposit");
            _sellTabButton = _buttons.GetNode<BaseButton>("Sell");
            _shopTabButton = _buttons.GetNode<BaseButton>("Market");
            _headerMargin = _top.GetNode<Node>("HeaderMargin");
            _headerDecor = _headerMargin.GetNode<TextureRect>("HeaderDecor");
            _headerLabel = _headerMargin.GetNode<Label>("Header");
            _secondRow = _vbox.GetNode<Node>("SecondRow");
            _coinsMargin = _secondRow.GetNode<Node>("CoinsMargin");
            _coinsDecor = _coinsMargin.GetNode<TextureRect>("CoinsDecor");
            _coinsLabel = _coinsMargin.GetNode<Label>("CoinAmount");
            _inventoryMargin = _vbox.GetNode<MarginContainer>("InventoryMargin");
            _inventoryScroll =
                _inventoryMargin.GetNode<Node>("InventoryScrollContainer");
            _inventoryGrid = _inventoryScroll.GetNode<Node>("InventoryItems");
            _itemPriceMargin = GetNode<MarginContainer>("ItemPriceMargin");
            _marketItemPriceMargin = GetNode<MarginContainer>("MarketItemPriceMargin");
            _depositMargin = _vbox.GetNode<MarginContainer>("DepositMargin");
            _depositVbox = _depositMargin.GetNode<Node>("DepositVBox");
            _depositHBox = _depositVbox.GetNode<Node>("DepositHBox");
            _depositButtonMargin = _depositHBox.GetNode<Node>("DepositButtonMargin");
            _depositButtonVbox =
                _depositButtonMargin.GetNode<Node>("DepositButtonVBox");
            _depositButton = _depositButtonVbox.GetNode<Button>("Deposit");
            _depositProgressVbox = _depositVbox.GetNode<Node>("DepositProgressVBox");
            _nextLevelLabel = _depositProgressVbox.GetNode<Label>("NextLevel");
            _nextLevelProgressText =
                _depositProgressVbox.GetNode<Label>("ProgressText");
            _wholeProgressBarMargin =
                _depositProgressVbox.GetNode<Node>("WholeProgressBarMargin");
            _progressBarMargin =
                _wholeProgressBarMargin.GetNode<Node>("ProgressBarMargin");
            _depositProgressBar =
                _progressBarMargin.GetNode<TextureProgress>("Progress");
            _marketMargin = _vbox.GetNode<MarginContainer>("MarketMargin");
            _marketItemsGrid = _marketMargin.GetNode<Node>("MarketItems");

            _margin.Visible = false;
            _itemPriceMargin.Visible = false;

            foreach (BaseButton button in _buttons.GetChildren())
            {
                button.Connect("button_down", this, nameof(OnButton_ButtonDown),
                    new Array {button});
                button.Connect("button_up", this, nameof(OnButton_ButtonUp),
                    new Array {button});
            }
        }

        private void OnButton_ButtonDown(BaseButton button)
        {
            currentMenu = button switch
            {
                var b when b == _depositTabButton => Menu.Deposit,
                var b when b == _sellTabButton => Menu.Sell,
                var b when b == _shopTabButton => Menu.Market,
                _ => throw new ArgumentOutOfRangeException()
            };
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, null, -15);
            UpdateMenuState();
        }

        private void OnButton_ButtonUp(BaseButton button)
        {
            button.Pressed = true;
        }

        private void UpdateMenuState()
        {
            switch (currentMenu)
            {
                case Menu.Deposit:
                    _shopTabButton.Pressed = false;
                    _sellTabButton.Pressed = false;
                    _depositTabButton.Pressed = true;
                    _headerLabel.Text = "Deposit";
                    _inventoryMargin.Visible = false;
                    _depositMargin.Visible = true;
                    _marketMargin.Visible = false;
                    break;
                case Menu.Sell:
                    _shopTabButton.Pressed = false;
                    _sellTabButton.Pressed = true;
                    _depositTabButton.Pressed = false;
                    _headerLabel.Text = "Sell";
                    _inventoryMargin.Visible = true;
                    _depositMargin.Visible = false;
                    _marketMargin.Visible = false;
                    break;
                case Menu.Market:
                    _shopTabButton.Pressed = true;
                    _sellTabButton.Pressed = false;
                    _depositTabButton.Pressed = false;
                    _headerLabel.Text = "Market";
                    _inventoryMargin.Visible = false;
                    _depositMargin.Visible = false;
                    _marketMargin.Visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _background.Texture = GetMenuBackgrounds(currentMenu);
            foreach (BaseButton button in _buttons.GetChildren())
            {
                button.Modulate = button.Pressed
                    ? GetMenuButtonPressedColor(currentMenu)
                    : GetMenuButtonNormalColor(currentMenu);
            }

            _headerDecor.Modulate = GetMenuHeaderDecorColor(currentMenu);
            _coinsDecor.Modulate = GetMenuCoinDecorColor(currentMenu);
        }

        public void Toggle(Player player)
        {
            _playerInstance = player;
            _tree.Paused = !_tree.Paused;
            _margin.Visible = _tree.Paused;
            _itemPriceMargin.Visible = false;
            _marketItemPriceMargin.Visible = false;

            _playerInstance.hudMargin.Visible = !_tree.Paused;
            if (_tree.Paused)
            {
                UpdateInventory();
                UpdateMenuState();
                UpdateCoins();
                UpdateGuildLevel();
                UpdateProgressBarInstantly();
                UpdateMarket();
            }
        }

        private void UpdateInventory()
        {
            foreach (Node child in _inventoryGrid.GetChildren())
            {
                child.QueueFree();
            }

            var sellableItems = new List<string>(_playerInstance.Inventory);
            sellableItems.Remove(_playerInstance.EquippedWeapon);
            sellableItems.Remove(_playerInstance.EquippedArmor);

            foreach (var itemName in sellableItems)
            {
                var sellableItem = (InventoryItem) PopupItemScene.Instance();
                _inventoryGrid.AddChild(sellableItem);
                sellableItem.itemName = itemName;
                sellableItem.itemTextureButton.TextureNormal = Utility.Utility
                    .GetInventoryItemResource(itemName);
                sellableItem.Connect("mouse_entered", this,
                    nameof(OnInventoryItem_MouseEntered), new Array {sellableItem});
                sellableItem.Connect("mouse_exited", this,
                    nameof(OnInventoryItem_MouseExited));
                sellableItem.itemTextureButton.Connect("pressed", this, nameof
                    (OnInventoryItem_Pressed), new Array {sellableItem});
            }
        }

        private void UpdateCoins()
        {
            _coinsLabel.Text = $"{_playerInstance.Coins}:COIN";
        }

        private void OnInventoryItem_MouseEntered(InventoryItem item)
        {
            selectedItem = item;
            _itemPriceMargin.RectPosition = item.RectGlobalPosition + new Vector2
                (item.RectSize.x, 0);
            var rectPosition = _itemPriceMargin.RectPosition;
            if (rectPosition.x >= 32)
            {
                rectPosition.x -= _itemPriceMargin.RectSize.x + item
                    .RectSize.x;
                _itemPriceMargin.RectPosition = rectPosition;
            }

            var itemName = item.itemName;
            var itemRecord = Autoload.Get<Data.Data>().itemData[itemName];
            var itemSellValue = itemRecord.sellValue;
            var itemPriceLabel =
                _itemPriceMargin.GetNode<Label>("ItemPriceTextMargin/ItemPrice");
            itemPriceLabel.Text = $"{itemSellValue}:c";
            _itemPriceMargin.Visible = true;
        }

        private void OnInventoryItem_MouseExited()
        {
            selectedItem = null;
            _itemPriceMargin.Visible = false;
        }

        private void OnInventoryItem_Pressed(InventoryItem item)
        {
            if (selectedItem == item)
            {
                selectedItem = null;
                _itemPriceMargin.Visible = false;
            }

            SellItem(item);
        }

        private void SellItem(InventoryItem item)
        {
            var itemName = item.itemName;
            var itemRecord = Autoload.Get<Data.Data>().itemData[itemName];
            var itemSellValue = itemRecord.sellValue;
            _playerInstance.Inventory.Remove(itemName);
            _playerInstance.Coins += itemSellValue;

            UpdateInventory();
            UpdateCoins();
        }

        private void OnIncreaseAmount_Pressed()
        {
            _depositAmountIndex = (_depositAmountIndex + 1) % DepositAmounts.Length;
            UpdateDepositAmount();
        }

        private void OnDecreaseAmount_Pressed()
        {
            _depositAmountIndex =
                Mathf.PosMod(_depositAmountIndex - 1, DepositAmounts.Length);
            UpdateDepositAmount();
        }

        private void UpdateDepositAmount()
        {
            _depositButton.Text = $"{DepositAmounts[_depositAmountIndex]}";
        }

        private void OnDeposit_Pressed()
        {
            var depositAmount = DepositAmounts[_depositAmountIndex];
            if (_playerInstance.Coins == 0)
            {
                return;
            }

            var newCoinsAmount = Mathf.Max(_playerInstance.Coins - depositAmount, 0);

            var saveData = Autoload.Get<SaveData>();
            saveData.CoinsDeposited += _playerInstance.Coins - newCoinsAmount;
            _playerInstance.Coins = newCoinsAmount;

            var totalCoinsForNextLevel = saveData.GuildLevel * 250;
            _depositProgressBar.MaxValue = totalCoinsForNextLevel;

            var levelledUp = false;
            while (saveData.CoinsDeposited > totalCoinsForNextLevel)
            {
                saveData.CoinsDeposited -= totalCoinsForNextLevel;
                saveData.GuildLevel += 1;
                levelledUp = true;
                totalCoinsForNextLevel = saveData.GuildLevel * 250;
            }

            if (levelledUp)
            {
                EmitSignal(nameof(GuildHallLevelUp));
            }

            UpdateCoins();
            UpdateGuildLevel();
            UpdateProgressBarInstantly();
        }

        private void UpdateGuildLevel()
        {
            var saveData = Autoload.Get<SaveData>();
            _nextLevelLabel.Text = $"To Level {saveData.GuildLevel + 1}";
            var totalCoinsForNextLevel = saveData.GuildLevel * 250;
            _nextLevelProgressText.Text =
                $"{saveData.CoinsDeposited}/{totalCoinsForNextLevel}";
        }

        private void UpdateProgressBarInstantly()
        {
            var saveData = Autoload.Get<SaveData>();
            var totalCoinsForNextLevel = saveData.GuildLevel * 250;
            _depositProgressBar.MaxValue = totalCoinsForNextLevel;
            _depositProgressBar.Value = saveData.CoinsDeposited;
        }

        private async void UpdateMarket()
        {
            foreach (Node child in _marketItemsGrid.GetChildren())
            {
                child.QueueFree();
            }

            var childCount = _marketItemsGrid.GetChildCount();
            if (childCount > 0)
            {
                await ToSignal(_marketItemsGrid.GetChild(childCount - 1),
                    "tree_exited");
            }

            var saveData = Autoload.Get<SaveData>();
            var buyableItems = (from pair in Autoload.Get<Data.Data>().itemData
                let itemRecord = pair.Value
                where itemRecord.buyValue > 0 && itemRecord.buyValue <=
                    saveData.GuildLevel * MAX_MARKET_PRICE_LEVEL_MULTIPLIER
                select pair.Key).ToList();

            while (_marketItemsGrid.GetChildCount() < MAX_MARKET_ITEMS)
            {
                var marketItem = (InventoryItem) PopupItemScene.Instance();
                _marketItemsGrid.AddChild(marketItem);
                var itemName = buyableItems.RandomElement();
                marketItem.itemName = itemName;
                marketItem.itemTextureButton.TextureNormal = Utility.Utility
                    .GetInventoryItemResource(itemName);
                marketItem.Connect("mouse_entered", this,
                    nameof(OnMarketItem_MouseEntered), new Array {marketItem});
                marketItem.Connect("mouse_exited", this,
                    nameof(OnMarketItem_MouseExited));
                marketItem.itemTextureButton.Connect("pressed", this, nameof
                    (OnMarketItem_Pressed), new Array {marketItem});
            }
        }

        private void OnMarketItem_MouseEntered(InventoryItem item)
        {
            selectedMarketItems = item;
            _marketItemPriceMargin.RectPosition = item.RectGlobalPosition + new
                Vector2(item.RectSize.x, 0);
            var rectPosition = _marketItemPriceMargin.RectPosition;
            if (rectPosition.x >= 32)
            {
                rectPosition.x -= _marketItemPriceMargin
                    .RectSize.x + item.RectSize.x;
                _marketItemPriceMargin.RectPosition = rectPosition;
            }

            var itemName = item.itemName;
            var itemRecord = Autoload.Get<Data.Data>().itemData[itemName];
            var itemPriceLabel =
                _marketItemPriceMargin.GetNode<Label>("ItemPriceTextMargin/ItemPrice");
            itemPriceLabel.Text = $"{itemRecord.buyValue}:c";
            _marketItemPriceMargin.Visible = true;
        }

        private void OnMarketItem_MouseExited()
        {
            selectedMarketItems = null;
            _marketItemPriceMargin.Visible = false;
        }

        private void OnMarketItem_Pressed(InventoryItem item)
        {
            if (BuyItem(item) && selectedMarketItems == item)
            {
                selectedMarketItems = null;
                _marketItemPriceMargin.Visible = false;
            }
        }

        private bool BuyItem(InventoryItem item)
        {
            var itemName = item.itemName;
            var itemRecord = Autoload.Get<Data.Data>().itemData[itemName];
            if (_playerInstance.Coins < itemRecord.buyValue)
            {
                return false;
            }

            _playerInstance.Inventory.Add(itemName);
            _playerInstance.Coins -= itemRecord.buyValue;
            UpdateInventory();
            UpdateCoins();
            return true;
        }
    }
}