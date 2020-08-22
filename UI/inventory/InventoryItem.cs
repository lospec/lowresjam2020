using Godot;

namespace HeroesGuild.UI.inventory
{
    public class InventoryItem : MarginContainer
    {
        public string ItemName;

        private TextureRect hoverSignifier;
        private TextureButton itemTextureButton;

        public Texture InventoryItemResource =>
            GD.Load<Texture>($"res://items/inventory_sprites/{ItemName.ToLower().Replace(" ", "_")}.png");

        public override void _Ready()
        {
            hoverSignifier = GetNode<TextureRect>("HoverSignifier");
            itemTextureButton = GetNode<TextureButton>("MarginContainer/Item");

            hoverSignifier.Visible = false;
        }

        private void OnInventoryItem_MouseEntered()
        {
            hoverSignifier.Visible = true;
        }

        private void OnInventoryItem_MouseExited()
        {
            hoverSignifier.Visible = false;
        }
    }
}