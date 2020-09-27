using System;
using Godot;
using HeroesGuild.data;
using HeroesGuild.utility;

namespace HeroesGuild.ui.inventory
{
    public class ItemStatPopUp : MarginContainer
    {
        private const int TOP_POS_Y = -19;
        private const int BOTTOM_POS_Y = 0;
        private const float ANIMATION_DURATION = 0.15f;
        private TextureRect _background;
        private Label _counterDamageLabel;
        private MarginContainer _damageMargin;
        private HBoxContainer _damageValuesHBox;
        private Label _healthGainedLabel;
        private MarginContainer _healthGainedMargin;
        private Label _heavyDamageLabel;
        private Label _itemNameLabel;
        private Timer _popupDisappearDelayTimer;
        private Label _quickDamageLabel;
        private MarginContainer _textMargin;
        private VBoxContainer _textVBox;

        private Tween _tween;

        public bool popupToBeVisible = false;

        private static Texture GetItemTypeBackground(string itemType)
        {
            var path = itemType switch
            {
                "item" => "res://ui/inventory/item_stats_popup.png",
                "weapon" => "res://ui/inventory/weapon_stats_popup.png",
                "usable" => "res://ui/inventory/usable_armor_stats_popup.png",
                "armor" => "res://ui/inventory/usable_armor_stats_popup.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        public override void _Ready()
        {
            _tween = GetNode<Tween>("Tween");
            _popupDisappearDelayTimer = GetNode<Timer>("PopupDisappearDelay");
            _background = GetNode<TextureRect>("Background");
            _textMargin = GetNode<MarginContainer>("TextMargin");
            _textVBox = _textMargin.GetNode<VBoxContainer>("VBoxContainer");
            _itemNameLabel = _textVBox.GetNode<Label>("Name");
            _damageMargin = _textVBox.GetNode<MarginContainer>("DamageMargin");
            _damageValuesHBox = _damageMargin.GetNode<HBoxContainer>("DamageValues");
            _quickDamageLabel = _damageValuesHBox.GetNode<Label>("QuickDamage");
            _heavyDamageLabel = _damageValuesHBox.GetNode<Label>("HeavyDamage");
            _counterDamageLabel = _damageValuesHBox.GetNode<Label>("CounterDamage");
            _healthGainedMargin =
                _textVBox.GetNode<MarginContainer>("HealthGainedMargin");
            _healthGainedLabel = _healthGainedMargin.GetNode<Label>("HealthGained");
        }

        public void OnItem_MouseEntered(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                GD.PushError("Item name must not be empty");
                return;
            }

            var data = Autoload.Get<Data>();
            if (!data.itemData.ContainsKey(itemName))
            {
                GD.PushError($"Item name {itemName} not found");
                return;
            }

            var itemRecord = data.itemData[itemName];
            var itemType = itemRecord.Type;
            _itemNameLabel.Text = itemName;
            _background.Texture = GetItemTypeBackground(itemType);
            switch (itemType)
            {
                case "item":
                    _damageMargin.Visible = false;
                    _healthGainedLabel.Visible = false;
                    break;
                case "weapon":
                    _damageMargin.Visible = true;
                    _healthGainedMargin.Visible = false;
                    _quickDamageLabel.Text = $"{itemRecord.QuickDamage}";
                    _heavyDamageLabel.Text = $"{itemRecord.HeavyDamage}";
                    _counterDamageLabel.Text = $"{itemRecord.CounterDamage}";
                    break;
                case "usable":
                    _damageMargin.Visible = false;
                    _healthGainedMargin.Visible = true;
                    _healthGainedLabel.Text = $"{itemRecord.HealthGained}";
                    break;
                case "armor":
                    _damageMargin.Visible = false;
                    _healthGainedMargin.Visible = true;
                    _healthGainedLabel.Text = $"{itemRecord.HealthAdded}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _tween.InterpolateProperty(this, "rect_position", RectPosition,
                new Vector2(RectPosition.x, BOTTOM_POS_Y),
                ANIMATION_DURATION, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            _tween.Start();
            popupToBeVisible = true;
        }

        public void OnItem_MouseExited()
        {
            popupToBeVisible = false;
            _popupDisappearDelayTimer.Start();
        }

        private void OnPopupDisappearDelay_Timeout()
        {
            if (!popupToBeVisible)
            {
                _tween.InterpolateProperty(this, "rect_position", RectPosition,
                    new Vector2(RectPosition.x, TOP_POS_Y),
                    ANIMATION_DURATION, Tween.TransitionType.Cubic, Tween.EaseType.Out);
                _tween.Start();
            }
        }

        private void OnItemInfoMenu_DetailedItemInfoMenuAppeared()
        {
            Visible = false;
        }

        private void OnItemInfoMenu_DetailedItemInfoMenuDisappeared()
        {
            Visible = true;
        }
    }
}