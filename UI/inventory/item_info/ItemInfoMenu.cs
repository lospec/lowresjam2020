using System;
using Godot;
using HeroesGuild.data;
using HeroesGuild.Entities.Player;
using HeroesGuild.UI.pause_menu;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.inventory.item_info
{
    public class ItemInfoMenu : MarginContainer
    {
        [Signal] public delegate void DetailedItemInfoMenuAppeared();

        [Signal] public delegate void DetailedItemInfoMenuDisappeared();

        [Signal] public delegate void EquippedWeaponChanged();

        [Signal] public delegate void EquippedArmorChanged();

        [Signal] public delegate void YesOrNoPressed(bool yesPressed);

        private static Texture GetDamageTypeIcon(string damageType)
        {
            var path = damageType switch
            {
                "none" => "res://UI/inventory/item_info/type_icons/none_type_icon.png",
                "Pierce" => "res://UI/inventory/item_info/type_icons/pierce_type_icon.png",
                "Blunt" => "res://UI/inventory/item_info/type_icons/blunt_type_icon.png",
                "Fire" => "res://UI/inventory/item_info/type_icons/fire_type_icon.png",
                "Water" => "res://UI/inventory/item_info/type_icons/water_type_icon.png",
                "Electricity" => "res://UI/inventory/item_info/type_icons/electricity_type_icon.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        private static Texture GetDamageTypeStatusIcon(string damageType)
        {
            var path = damageType switch
            {
                "none" => "res://UI/inventory/item_info/status_icons/none_status_icon.png",
                "Asleep" => "res://UI/inventory/item_info/status_icons/asleep_status_icon.png",
                "OnFire" => "res://UI/inventory/item_info/type_icons/fire_type_icon.png",
                "Frozen" => "res://UI/inventory/item_info/status_icons/frozen_status_icon.png",
                "Weak" => "res://UI/inventory/item_info/status_icons/weak_status_icon.png",
                "Confused" => "res://UI/inventory/item_info/status_icons/confused_status_icon.png",
                "Poison" => "res://UI/inventory/item_info/status_icons/poison_status_icon.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        public string SelectedItem;
        public Player PlayerInstance;

        private MarginContainer margin;
        private VBoxContainer vBox;
        private HBoxContainer topHBox;
        private MarginContainer nameMargin;
        private MarginContainer nameLabelMargin;
        private Label nameLabel;
        private HBoxContainer secondRowHBox;
        private MarginContainer itemMargin;
        private MarginContainer iconMargin;
        private TextureRect itemIcon;
        private MarginContainer buttonMargin;
        private TextureButton equipButton;
        private TextureButton useButton;
        private HBoxContainer damageValues;
        private VBoxContainer quickVBox;
        private Label quickDamageLabel;
        private VBoxContainer quickIconVBox;
        private TextureRect quickTypeIcon;
        private TextureRect quickStatusIcon;
        private Label quickStatusEffectChanceLabel;
        private VBoxContainer heavyVBox;
        private Label heavyDamageLabel;
        private VBoxContainer heavyIconVBox;
        private TextureRect heavyTypeIcon;
        private TextureRect heavyStatusIcon;
        private Label heavyStatusEffectChanceLabel;
        private VBoxContainer counterVBox;
        private Label counterDamageLabel;
        private VBoxContainer counterIconVBox;
        private TextureRect counterTypeIcon;
        private TextureRect counterStatusIcon;
        private Label counterStatusEffectChanceLabel;
        private HBoxContainer healthGainedHBox;
        private Label healthGainedValueLabel;
        private MarginContainer confirmUsePopup;

        private TextureButton backButton;
        private TextureButton yesButton;
        private TextureButton noButton;

        public override void _Ready()
        {
            margin = GetNode<MarginContainer>("MarginContainer");
            vBox = margin.GetNode<VBoxContainer>("VBoxContainer");
            topHBox = vBox.GetNode<HBoxContainer>("Top");
            nameMargin = topHBox.GetNode<MarginContainer>("NameMargin");
            nameLabelMargin = nameMargin.GetNode<MarginContainer>("NameLabelMargin");
            nameLabel = nameLabelMargin.GetNode<Label>("Name");
            secondRowHBox = vBox.GetNode<HBoxContainer>("SecondRow");
            itemMargin = secondRowHBox.GetNode<MarginContainer>("ItemMargin");
            iconMargin = itemMargin.GetNode<MarginContainer>("IconMargin");
            itemIcon = iconMargin.GetNode<TextureRect>("ItemIcon");
            buttonMargin = secondRowHBox.GetNode<MarginContainer>("ButtonMargin");
            equipButton = buttonMargin.GetNode<TextureButton>("Equip");
            useButton = buttonMargin.GetNode<TextureButton>("Use");
            damageValues = vBox.GetNode<HBoxContainer>("DamageValues");
            quickVBox = damageValues.GetNode<VBoxContainer>("Quick");
            quickDamageLabel = quickVBox.GetNode<Label>("DamageValue");
            quickIconVBox = quickVBox.GetNode<VBoxContainer>("IconVBoxContainer");
            quickTypeIcon = quickIconVBox.GetNode<TextureRect>("TypeIcon");
            quickStatusIcon = quickIconVBox.GetNode<TextureRect>("StatusIcon");
            quickStatusEffectChanceLabel = quickIconVBox.GetNode<Label>("StatusEffectChance");
            heavyVBox = damageValues.GetNode<VBoxContainer>("Heavy");
            heavyDamageLabel = heavyVBox.GetNode<Label>("DamageValue");
            heavyIconVBox = heavyVBox.GetNode<VBoxContainer>("IconVBoxContainer");
            heavyTypeIcon = heavyIconVBox.GetNode<TextureRect>("TypeIcon");
            heavyStatusIcon = heavyIconVBox.GetNode<TextureRect>("StatusIcon");
            heavyStatusEffectChanceLabel = heavyIconVBox.GetNode<Label>("StatusEffectChance");
            counterVBox = damageValues.GetNode<VBoxContainer>("Counter");
            counterDamageLabel = counterVBox.GetNode<Label>("DamageValue");
            counterIconVBox = counterVBox.GetNode<VBoxContainer>("IconVBoxContainer");
            counterTypeIcon = counterIconVBox.GetNode<TextureRect>("TypeIcon");
            counterStatusIcon = counterIconVBox.GetNode<TextureRect>("StatusIcon");
            counterStatusEffectChanceLabel = counterIconVBox.GetNode<Label>("StatusEffectChance");
            healthGainedHBox = vBox.GetNode<HBoxContainer>("HealthGainedHBox");
            healthGainedValueLabel = healthGainedHBox.GetNode<Label>("HealthGainedValueLabel");
            confirmUsePopup = margin.GetNode<MarginContainer>("ConfirmUsePopup");
            backButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/Top/Back");
            yesButton = GetNode<TextureButton>(
                "MarginContainer/ConfirmUsePopup/TextMargin/VBoxContainer/HBoxContainer/Yes");
            noButton = GetNode<TextureButton>(
                "MarginContainer/ConfirmUsePopup/TextMargin/VBoxContainer/HBoxContainer/No");

            confirmUsePopup.Visible = false;
        }

        public void OnItem_Pressed(InventoryItem item, Player player)
        {
            var itemName = item.ItemName;
            SelectedItem = itemName;
            PlayerInstance = player;
            Visible = true;
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonHover, item.RectGlobalPosition, -15);
            EmitSignal(nameof(DetailedItemInfoMenuAppeared));
            nameLabel.Text = itemName;
            itemIcon.Texture =
                GD.Load<Texture>($"res://items/inventory_sprites/{itemName.ToLower().Replace(" ", "_")}.png");

            var data = Singleton.Get<Data>(this);
            if (!data.ItemData.ContainsKey(itemName))
            {
                return;
            }

            var itemRecord = data.ItemData[itemName];
            var itemType = itemRecord.Type;
            switch (itemType)
            {
                case "item":
                    damageValues.Visible = false;
                    equipButton.Visible = false;
                    useButton.Visible = false;
                    healthGainedHBox.Visible = false;
                    break;
                case "weapon":
                    damageValues.Visible = true;
                    equipButton.Visible = PlayerInstance.EquippedWeapon != itemName;
                    useButton.Visible = false;
                    healthGainedHBox.Visible = false;
                    quickDamageLabel.Text = $"{itemRecord.QuickDamage}";
                    heavyDamageLabel.Text = $"{itemRecord.HeavyDamage}";
                    counterDamageLabel.Text = $"{itemRecord.CounterDamage}";
                    quickTypeIcon.Texture = GetDamageTypeIcon(itemRecord.QuickDamageType);
                    heavyTypeIcon.Texture = GetDamageTypeIcon(itemRecord.HeavyDamageType);
                    counterTypeIcon.Texture = GetDamageTypeIcon(itemRecord.CounterDamageType);

                    quickStatusIcon.Texture = GetDamageTypeStatusIcon(itemRecord.QuickStatusEffect);
                    quickStatusEffectChanceLabel.Text = itemRecord.QuickStatusEffect == "none"
                        ? string.Empty
                        : $"{itemRecord.QuickEffectChance}";

                    heavyStatusIcon.Texture = GetDamageTypeStatusIcon(itemRecord.HeavyStatusEffect);
                    heavyStatusEffectChanceLabel.Text = itemRecord.HeavyStatusEffect == "none"
                        ? string.Empty
                        : $"{itemRecord.HeavyEffectChance}";

                    counterStatusIcon.Texture = GetDamageTypeStatusIcon(itemRecord.CounterStatusEffect);
                    counterStatusEffectChanceLabel.Text = itemRecord.CounterStatusEffect == "none"
                        ? string.Empty
                        : $"{itemRecord.CounterEffectChance}";
                    break;
                case "usable":
                    damageValues.Visible = false;
                    equipButton.Visible = false;
                    useButton.Visible = true;
                    healthGainedHBox.Visible = true;
                    healthGainedValueLabel.Text = $"{itemRecord.HealthGained}";
                    break;
                case "armor":
                    damageValues.Visible = false;
                    equipButton.Visible = PlayerInstance.EquippedArmor != itemName;
                    useButton.Visible = false;
                    healthGainedHBox.Visible = true;
                    healthGainedValueLabel.Text = $"{itemRecord.HealthAdded}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnBack_Pressed()
        {
            Visible = false;
            AudioSystem.PlaySFX(AudioSystem.SFX.Deny, backButton.RectGlobalPosition, -15);
            EmitSignal(nameof(DetailedItemInfoMenuDisappeared));
        }

        private void OnEquip_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, equipButton.RectGlobalPosition, -15);
            var data = Singleton.Get<Data>(this).ItemData;
            if (!data.ContainsKey(SelectedItem))
            {
                return;
            }

            var itemRecord = data[SelectedItem];
            var selectedItemType = itemRecord.Type;
            switch (selectedItemType)
            {
                case "weapon":
                    PlayerInstance.EquippedWeapon = SelectedItem;
                    EmitSignal(nameof(EquippedWeaponChanged));
                    equipButton.Visible = false;
                    break;
                case "armor":
                    PlayerInstance.EquippedArmor = SelectedItem;
                    PlayerInstance.MaxHealth = 10 + itemRecord.HealthAdded;
                    PlayerInstance.Health += itemRecord.HealthAdded;
                    EmitSignal(nameof(EquippedArmorChanged));
                    equipButton.Visible = false;
                    break;
                default:
                    GD.PushError("The equip button was pressed for a non-equippable item.");
                    break;
            }
        }

        private async void OnUse_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, useButton.RectGlobalPosition, -15);
            var data = Singleton.Get<Data>(this).ItemData;
            if (!data.ContainsKey(SelectedItem))
            {
                return;
            }

            var itemRecord = data[SelectedItem];
            var selectedItemType = itemRecord.Type;
            if (selectedItemType == "usable")
            {
                if (PlayerInstance.Health >= PlayerInstance.MaxHealth)
                {
                    confirmUsePopup.Visible = true;
                    var yesPressed = (bool) (await ToSignal(this, nameof(YesOrNoPressed)))[0];
                    confirmUsePopup.Visible = false;
                    if (!yesPressed)
                    {
                        return;
                    }
                }
                else
                {
                    PlayerInstance.Health = Mathf.Min(PlayerInstance.Health + itemRecord.HealthGained,
                        PlayerInstance.MaxHealth);
                }

                PlayerInstance.Inventory.Remove(SelectedItem);
                GetParent().GetParent().GetParent<PauseMenu>().UpdateInventory();
                Visible = false;
            }
            else
            {
                GD.PushError("The use button was pressed for a non-usable item");
            }
        }

        private void OnYes_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, yesButton.RectGlobalPosition, -15);
            EmitSignal(nameof(YesOrNoPressed), true);
        }

        private void OnNo_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.Deny, noButton.RectGlobalPosition, -15);
            EmitSignal(nameof(YesOrNoPressed), false);
        }
    }
}