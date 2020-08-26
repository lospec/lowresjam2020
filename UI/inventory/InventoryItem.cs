using Godot;

namespace HeroesGuild.UI.inventory
{
    public class InventoryItem : MarginContainer
    {
        public string itemName;

        private TextureRect _hoverSignifier;
        public TextureButton itemTextureButton;

        public Texture InventoryItemResource =>
            GD.Load<Texture>($"res://items/inventory_sprites/{itemName.ToLower().Replace(" ", "_")}.png");

        public override void _Ready()
        {
            _hoverSignifier = GetNode<TextureRect>("HoverSignifier");
            itemTextureButton = GetNode<TextureButton>("MarginContainer/Item");

            _hoverSignifier.Visible = false;
        }

        private void OnInventoryItem_MouseEntered()
        {
            _hoverSignifier.Visible = true;
        }

        private void OnInventoryItem_MouseExited()
        {
            _hoverSignifier.Visible = false;
        }
    }
}