using System;
using Godot;
using HeroesGuild.data;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.inventory
{
    public class ItemStatPopUp : MarginContainer
    {
        private const int TOP_POS_Y = -19;
        private const int BOTTOM_POS_Y = 0;
        private const float ANIMATION_DURATION = 0.15f;

        private static Texture GetItemTypeBackground(string itemType)
        {
            var path = itemType switch
            {
                "item" => "res://UI/inventory/item_stats_popup.png",
                "weapon" => "res://UI/inventory/weapon_stats_popup.png",
                "usable" => "res://UI/inventory/usable_armor_stats_popup.png",
                "armor" => "res://UI/inventory/usable_armor_stats_popup.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        public bool popupToBeVisible = false;

        private Tween tween;
        private Timer popupDisappearDelayTimer;
        private TextureRect background;
        private MarginContainer textMargin;
        private VBoxContainer textVBox;
        private Label itemNameLabel;
        private MarginContainer damageMargin;
        private HBoxContainer damageValuesHBox;
        private Label quickDamageLabel;
        private Label heavyDamageLabel;
        private Label counterDamageLabel;
        private MarginContainer healthGainedMargin;
        private Label healthGainedLabel;

        public override void _Ready()
        {
            tween = GetNode<Tween>("Tween");
            popupDisappearDelayTimer = GetNode<Timer>("PopupDisappearDelay");
            background = GetNode<TextureRect>("Background");
            textMargin = GetNode<MarginContainer>("TextMargin");
            textVBox = textMargin.GetNode<VBoxContainer>("VBoxContainer");
            itemNameLabel = textVBox.GetNode<Label>("Name");
            damageMargin = textVBox.GetNode<MarginContainer>("DamageMargin");
            damageValuesHBox = damageMargin.GetNode<HBoxContainer>("DamageValues");
            quickDamageLabel = damageValuesHBox.GetNode<Label>("QuickDamage");
            heavyDamageLabel = damageValuesHBox.GetNode<Label>("HeavyDamage");
            counterDamageLabel = damageValuesHBox.GetNode<Label>("CounterDamage");
            healthGainedMargin = textVBox.GetNode<MarginContainer>("HealthGainedMargin");
            healthGainedLabel = healthGainedMargin.GetNode<Label>("HealthGained");
        }

        public void OnItem_MouseEntered(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                GD.PushError($"Item name must not be empty");
                return;
            }

            var data = Singleton.Get<Data>(this);
            if (!data.ItemData.ContainsKey(itemName))
            {
                GD.PushError($"Item name {itemName} not found");
                return;
            }

            var itemRecord = data.ItemData[itemName];
            var itemType = itemRecord.Type;
            itemNameLabel.Text = itemName;
            background.Texture = GetItemTypeBackground(itemType);
            switch (itemType)
            {
                case "item":
                    damageMargin.Visible = false;
                    healthGainedLabel.Visible = false;
                    break;
                case "weapon":
                    damageMargin.Visible = true;
                    healthGainedMargin.Visible = false;
                    quickDamageLabel.Text = $"{itemRecord.QuickDamage}";
                    heavyDamageLabel.Text = $"{itemRecord.HeavyDamage}";
                    counterDamageLabel.Text = $"{itemRecord.CounterDamage}";
                    break;
                case "usable":
                    damageMargin.Visible = false;
                    healthGainedMargin.Visible = true;
                    healthGainedLabel.Text = $"{itemRecord.HealthGained}";
                    break;
                case "armor":
                    damageMargin.Visible = false;
                    healthGainedMargin.Visible = true;
                    healthGainedLabel.Text = $"{itemRecord.HealthAdded}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            tween.InterpolateProperty(this, "rect_position", RectPosition, new Vector2(RectPosition.x, BOTTOM_POS_Y),
                ANIMATION_DURATION, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.Start();
            popupToBeVisible = true;
        }

        public void OnItem_MouseExited()
        {
            popupToBeVisible = false;
            popupDisappearDelayTimer.Start();
        }

        private void OnPopupDisappearDelay_Timeout()
        {
            if (!popupToBeVisible)
            {
                tween.InterpolateProperty(this, "rect_position", RectPosition, new Vector2(RectPosition.x, TOP_POS_Y),
                    ANIMATION_DURATION, Tween.TransitionType.Cubic, Tween.EaseType.Out);
                tween.Start();
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