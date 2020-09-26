using Godot;

namespace HeroesGuild.guild_hall.guild_interface
{
    public class PopupItem : MarginContainer
    {
        private TextureRect _hoverSignifier;
        private TextureButton _itemTextureButton;
        public string itemName;

        public override void _Ready()
        {
            _hoverSignifier =
                GetNode<TextureRect>("HBoxContainer/ItemMargin/HoverSignifier");
            _itemTextureButton =
                GetNode<TextureButton>("HBoxContainer/ItemMargin/IconMargin/Item");
            _hoverSignifier.Visible = false;
        }

        private void OnItem_MouseEntered()
        {
            _hoverSignifier.Visible = true;
        }

        private void OnItem_MouseExited()
        {
            _hoverSignifier.Visible = false;
        }
    }
}