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
        private const string InventorySprite = "res://items/inventory_sprites/{0}.png";

        [Signal] public delegate void DetailedItemInfoMenuAppeared();

        [Signal] public delegate void DetailedItemInfoMenuDisappeared();

        [Signal] public delegate void EquippedWeaponChanged();

        [Signal] public delegate void EquippedArmorChanged();

        [Signal] public delegate void YesOrNoPressed(bool yesPressed);

        private static Texture GetDamageTypeIcon(string damageType)
        {
            var path = damageType switch
            {
                "none" => "res://ui/inventory/item_info/type_icons/none_type_icon.png",
                "Pierce" =>
                    "res://ui/inventory/item_info/type_icons/pierce_type_icon.png",
                "Blunt" =>
                    "res://ui/inventory/item_info/type_icons/blunt_type_icon.png",
                "Fire" => "res://ui/inventory/item_info/type_icons/fire_type_icon.png",
                "Water" =>
                    "res://ui/inventory/item_info/type_icons/water_type_icon.png",
                "Electricity" =>
                    "res://ui/inventory/item_info/type_icons/electricity_type_icon.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        private static Texture GetDamageTypeStatusIcon(string damageType)
        {
            var path = damageType switch
            {
                "none" =>
                    "res://UI/inventory/item_info/status_icons/none_status_icon.png",
                "Asleep" =>
                    "res://UI/inventory/item_info/status_icons/asleep_status_icon.png",
                "OnFire" =>
                    "res://UI/inventory/item_info/type_icons/fire_type_icon.png",
                "Frozen" =>
                    "res://UI/inventory/item_info/status_icons/frozen_status_icon.png",
                "Weak" =>
                    "res://UI/inventory/item_info/status_icons/weak_status_icon.png",
                "Confused" =>
                    "res://UI/inventory/item_info/status_icons/confused_status_icon.png",
                "Poison" =>
                    "res://UI/inventory/item_info/status_icons/poison_status_icon.png",
                _ => throw new ArgumentOutOfRangeException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

        public string selectedItem;
        public Player playerInstance;

        private MarginContainer _margin;
        private VBoxContainer _vBox;
        private HBoxContainer _topHBox;
        private MarginContainer _nameMargin;
        private MarginContainer _nameLabelMargin;
        private Label _nameLabel;
        private HBoxContainer _secondRowHBox;
        private MarginContainer _itemMargin;
        private MarginContainer _iconMargin;
        private TextureRect _itemIcon;
        private MarginContainer _buttonMargin;
        private TextureButton _equipButton;
        private TextureButton _useButton;
        private HBoxContainer _damageValues;
        private VBoxContainer _quickVBox;
        private Label _quickDamageLabel;
        private VBoxContainer _quickIconVBox;
        private TextureRect _quickTypeIcon;
        private TextureRect _quickStatusIcon;
        private Label _quickStatusEffectChanceLabel;
        private VBoxContainer _heavyVBox;
        private Label _heavyDamageLabel;
        private VBoxContainer _heavyIconVBox;
        private TextureRect _heavyTypeIcon;
        private TextureRect _heavyStatusIcon;
        private Label _heavyStatusEffectChanceLabel;
        private VBoxContainer _counterVBox;
        private Label _counterDamageLabel;
        private VBoxContainer _counterIconVBox;
        private TextureRect _counterTypeIcon;
        private TextureRect _counterStatusIcon;
        private Label _counterStatusEffectChanceLabel;
        private HBoxContainer _healthGainedHBox;
        private Label _healthGainedValueLabel;
        private MarginContainer _confirmUsePopup;

        private TextureButton _backButton;
        private TextureButton _yesButton;
        private TextureButton _noButton;

        public override void _Ready()
        {
            _margin = GetNode<MarginContainer>("MarginContainer");
            _vBox = _margin.GetNode<VBoxContainer>("VBoxContainer");
            _topHBox = _vBox.GetNode<HBoxContainer>("Top");
            _nameMargin = _topHBox.GetNode<MarginContainer>("NameMargin");
            _nameLabelMargin = _nameMargin.GetNode<MarginContainer>("NameLabelMargin");
            _nameLabel = _nameLabelMargin.GetNode<Label>("Name");
            _secondRowHBox = _vBox.GetNode<HBoxContainer>("SecondRow");
            _itemMargin = _secondRowHBox.GetNode<MarginContainer>("ItemMargin");
            _iconMargin = _itemMargin.GetNode<MarginContainer>("IconMargin");
            _itemIcon = _iconMargin.GetNode<TextureRect>("ItemIcon");
            _buttonMargin = _secondRowHBox.GetNode<MarginContainer>("ButtonMargin");
            _equipButton = _buttonMargin.GetNode<TextureButton>("Equip");
            _useButton = _buttonMargin.GetNode<TextureButton>("Use");
            _damageValues = _vBox.GetNode<HBoxContainer>("DamageValues");
            _quickVBox = _damageValues.GetNode<VBoxContainer>("Quick");
            _quickDamageLabel = _quickVBox.GetNode<Label>("DamageValue");
            _quickIconVBox = _quickVBox.GetNode<VBoxContainer>("IconVBoxContainer");
            _quickTypeIcon = _quickIconVBox.GetNode<TextureRect>("TypeIcon");
            _quickStatusIcon = _quickIconVBox.GetNode<TextureRect>("StatusIcon");
            _quickStatusEffectChanceLabel =
                _quickIconVBox.GetNode<Label>("StatusEffectChance");
            _heavyVBox = _damageValues.GetNode<VBoxContainer>("Heavy");
            _heavyDamageLabel = _heavyVBox.GetNode<Label>("DamageValue");
            _heavyIconVBox = _heavyVBox.GetNode<VBoxContainer>("IconVBoxContainer");
            _heavyTypeIcon = _heavyIconVBox.GetNode<TextureRect>("TypeIcon");
            _heavyStatusIcon = _heavyIconVBox.GetNode<TextureRect>("StatusIcon");
            _heavyStatusEffectChanceLabel =
                _heavyIconVBox.GetNode<Label>("StatusEffectChance");
            _counterVBox = _damageValues.GetNode<VBoxContainer>("Counter");
            _counterDamageLabel = _counterVBox.GetNode<Label>("DamageValue");
            _counterIconVBox = _counterVBox.GetNode<VBoxContainer>("IconVBoxContainer");
            _counterTypeIcon = _counterIconVBox.GetNode<TextureRect>("TypeIcon");
            _counterStatusIcon = _counterIconVBox.GetNode<TextureRect>("StatusIcon");
            _counterStatusEffectChanceLabel =
                _counterIconVBox.GetNode<Label>("StatusEffectChance");
            _healthGainedHBox = _vBox.GetNode<HBoxContainer>("HealthGainedHBox");
            _healthGainedValueLabel =
                _healthGainedHBox.GetNode<Label>("HealthGainedValueLabel");
            _confirmUsePopup = _margin.GetNode<MarginContainer>("ConfirmUsePopup");
            _backButton =
                GetNode<TextureButton>("MarginContainer/VBoxContainer/Top/Back");
            _yesButton = GetNode<TextureButton>(
                "MarginContainer/ConfirmUsePopup/TextMargin/VBoxContainer/HBoxContainer/Yes");
            _noButton = GetNode<TextureButton>(
                "MarginContainer/ConfirmUsePopup/TextMargin/VBoxContainer/HBoxContainer/No");

            _confirmUsePopup.Visible = false;
        }

        public void OnItem_Pressed(InventoryItem item, Player player)
        {
            var itemName = item.itemName;
            selectedItem = itemName;
            playerInstance = player;
            Visible = true;
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonHover, item.RectGlobalPosition,
                -15);
            EmitSignal(nameof(DetailedItemInfoMenuAppeared));
            _nameLabel.Text = itemName;

            _itemIcon.Texture =
                GD.Load<Texture>(string.Format(InventorySprite,
                    itemName.ToLower().Replace(" ", "_")));


            var data = Autoload.Get<Data>();
            if (!data.itemData.ContainsKey(itemName))
            {
                return;
            }

            var itemRecord = data.itemData[itemName];
            var itemType = itemRecord.type;
            switch (itemType)
            {
                case "item":
                    _damageValues.Visible = false;
                    _equipButton.Visible = false;
                    _useButton.Visible = false;
                    _healthGainedHBox.Visible = false;
                    break;
                case "weapon":
                    _damageValues.Visible = true;
                    _equipButton.Visible = playerInstance.EquippedWeapon != itemName;
                    _useButton.Visible = false;
                    _healthGainedHBox.Visible = false;
                    _quickDamageLabel.Text = $"{itemRecord.quickDamage}";
                    _heavyDamageLabel.Text = $"{itemRecord.heavyDamage}";
                    _counterDamageLabel.Text = $"{itemRecord.counterDamage}";
                    _quickTypeIcon.Texture =
                        GetDamageTypeIcon(itemRecord.quickDamageType);
                    _heavyTypeIcon.Texture =
                        GetDamageTypeIcon(itemRecord.heavyDamageType);
                    _counterTypeIcon.Texture =
                        GetDamageTypeIcon(itemRecord.counterDamageType);

                    _quickStatusIcon.Texture =
                        GetDamageTypeStatusIcon(itemRecord.quickStatusEffect);
                    _quickStatusEffectChanceLabel.Text =
                        itemRecord.quickStatusEffect == "none"
                            ? string.Empty
                            : $"{itemRecord.quickEffectChance}";

                    _heavyStatusIcon.Texture =
                        GetDamageTypeStatusIcon(itemRecord.heavyStatusEffect);
                    _heavyStatusEffectChanceLabel.Text =
                        itemRecord.heavyStatusEffect == "none"
                            ? string.Empty
                            : $"{itemRecord.heavyEffectChance}";

                    _counterStatusIcon.Texture =
                        GetDamageTypeStatusIcon(itemRecord.counterStatusEffect);
                    _counterStatusEffectChanceLabel.Text =
                        itemRecord.counterStatusEffect == "none"
                            ? string.Empty
                            : $"{itemRecord.counterEffectChance}";
                    break;
                case "usable":
                    _damageValues.Visible = false;
                    _equipButton.Visible = false;
                    _useButton.Visible = true;
                    _healthGainedHBox.Visible = true;
                    _healthGainedValueLabel.Text = $"{itemRecord.healthGained}";
                    break;
                case "armor":
                    _damageValues.Visible = false;
                    _equipButton.Visible = playerInstance.EquippedArmor != itemName;
                    _useButton.Visible = false;
                    _healthGainedHBox.Visible = true;
                    _healthGainedValueLabel.Text = $"{itemRecord.healthAdded}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnBack_Pressed()
        {
            Visible = false;
            AudioSystem.PlaySFX(AudioSystem.SFX.Deny, _backButton.RectGlobalPosition,
                -15);
            EmitSignal(nameof(DetailedItemInfoMenuDisappeared));
        }

        private void OnEquip_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick,
                _equipButton.RectGlobalPosition, -15);
            var data = Autoload.Get<Data>().itemData;
            if (!data.ContainsKey(selectedItem))
            {
                return;
            }

            var itemRecord = data[selectedItem];
            var selectedItemType = itemRecord.type;
            switch (selectedItemType)
            {
                case "weapon":
                    playerInstance.EquippedWeapon = selectedItem;
                    EmitSignal(nameof(EquippedWeaponChanged));
                    _equipButton.Visible = false;
                    break;
                case "armor":
                    playerInstance.EquippedArmor = selectedItem;
                    playerInstance.maxHealth = 10 + itemRecord.healthAdded;
                    playerInstance.Health += itemRecord.healthAdded;
                    EmitSignal(nameof(EquippedArmorChanged));
                    _equipButton.Visible = false;
                    break;
                default:
                    GD.PushError(
                        "The equip button was pressed for a non-equippable item.");
                    break;
            }
        }

        private async void OnUse_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick,
                _useButton.RectGlobalPosition, -15);
            var data = Autoload.Get<Data>().itemData;
            if (!data.ContainsKey(selectedItem))
            {
                return;
            }

            var itemRecord = data[selectedItem];
            var selectedItemType = itemRecord.type;
            if (selectedItemType == "usable")
            {
                if (playerInstance.Health >= playerInstance.maxHealth)
                {
                    _confirmUsePopup.Visible = true;
                    var yesPressed =
                        (bool) (await ToSignal(this, nameof(YesOrNoPressed)))[0];
                    _confirmUsePopup.Visible = false;
                    if (!yesPressed)
                    {
                        return;
                    }
                }
                else
                {
                    playerInstance.Health = Mathf.Min(
                        playerInstance.Health + itemRecord.healthGained,
                        playerInstance.maxHealth);
                }

                playerInstance.Inventory.Remove(selectedItem);
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
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick,
                _yesButton.RectGlobalPosition, -15);
            EmitSignal(nameof(YesOrNoPressed), true);
        }

        private void OnNo_Pressed()
        {
            AudioSystem.PlaySFX(AudioSystem.SFX.Deny, _noButton.RectGlobalPosition,
                -15);
            EmitSignal(nameof(YesOrNoPressed), false);
        }
    }
}