using Godot;
using HeroesGuild.utility;

namespace HeroesGuild.guild_hall.chest
{
    public class ChestItems : MarginContainer
    {
        [Signal] public delegate void LeftClicked(Node chestItemInstance);

        private string _itemName;
        public CenterContainer itemCenter;


        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                var textureRect = GetNode<TextureRect>("ItemCenter/Item");
                textureRect.Texture = string.IsNullOrWhiteSpace(_itemName)
                    ? null
                    : Utility.GetInventoryItemResource(value);
            }
        }

        public override void _Ready()
        {
            itemCenter = GetNode<CenterContainer>("ItemCenter");
        }

        private void OnItem_GUIInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton &&
                mouseButton.ButtonIndex == (int) ButtonList.Left &&
                mouseButton.IsPressed())
                EmitSignal(nameof(LeftClicked), this);
        }
    }
}