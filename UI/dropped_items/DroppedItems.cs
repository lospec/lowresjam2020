using System.Collections.Generic;
using Godot;
using HeroesGuild.data;
using HeroesGuild.Entities.Player;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.dropped_items
{
    public class DroppedItems : CanvasLayer
    {
        private const float MAX_COIN_DROP = 1.2f;
        private const float MIN_COIN_DROP = 0.8f;
        private static readonly PackedScene ItemDroppedResource =
            ResourceLoader.Load<PackedScene>("res://UI/dropped_items/item_dropped.tscn");

        private Player _playerInstance;

        public MarginContainer margin;
        private Label _coinAmountLabel;
        private GridContainer _itemsDroppedGrid;
        private Timer _autoCloseTimer;

        public override void _Ready()
        {
            margin = GetNode<MarginContainer>("MarginContainer");
            _coinAmountLabel = GetNode<Label>
                ("MarginContainer/MarginContainer/VBoxContainer/CoinsDropped/HBoxContainer/CoinAmount");
            _itemsDroppedGrid = GetNode<GridContainer>("MarginContainer/MarginContainer/VBoxContainer/ItemsDropped");
            _autoCloseTimer = GetNode<Timer>("AutoClose");
        }

        

        public void DropItems(string enemyName, Player playerInstance)
        {
            GetTree().Paused = true;
            _playerInstance = playerInstance;
            var enemyRecord = Autoload.Get<Data>().enemyData[enemyName];
            var coinsDropped = (int) GD.RandRange(enemyRecord.CoinDropAmount * MIN_COIN_DROP, enemyRecord
                .CoinDropAmount * MAX_COIN_DROP + 1);
            if (coinsDropped <= 0 && enemyRecord.CoinDropAmount > 0)
            {
                coinsDropped = 1;
            }

            _coinAmountLabel.Text = $"{coinsDropped}";
            _playerInstance.Coins += coinsDropped;

            foreach (Node child in _itemsDroppedGrid.GetChildren())
            {
                child.QueueFree();
            }

            var items = new Dictionary<string, float>();
            if (!string.IsNullOrWhiteSpace(enemyRecord.ItemDrop1))
            {
                items.Add(enemyRecord.ItemDrop1, enemyRecord.ItemDrop1Chance);
            }

            if (!string.IsNullOrWhiteSpace(enemyRecord.ItemDrop2))
            {
                items.Add(enemyRecord.ItemDrop2, enemyRecord.ItemDrop2Chance);
            }

            if (!string.IsNullOrWhiteSpace(enemyRecord.ItemDrop3))
            {
                items.Add(enemyRecord.ItemDrop3, enemyRecord.ItemDrop3Chance);
            }

            for (var i = 0; i < enemyRecord.MaxItemDropped; i++)
            {
                var chance = GD.Randf();
                foreach (var item in items)
                {
                    if (chance < item.Value)
                    {
                        var itemDropped = ItemDroppedResource.Instance();
                        itemDropped.GetNode<TextureRect>("Item").Texture = Utility.Utility.GetInventoryItemResource(item.Key);
                        _itemsDroppedGrid.AddChild(itemDropped);
                        playerInstance.Inventory.Add(item.Key);
                    }
                }
            }

            playerInstance.hudMargin.Visible = false;
            margin.Visible = true;
            _autoCloseTimer.Start();
        }

        public void Close()
        {
            margin.Visible = false;
            _playerInstance.hudMargin.Visible = true;
            GetTree().Paused = false;
        }

        private void OnAutoClose_Timeout()
        {
            Close();
        }
    }
}