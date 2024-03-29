using Godot;

namespace HeroesGuild.ui.inventory
{
    public class InventoryItem : MarginContainer
    {
        private const string SpritePath = "res://items/inventory_sprites/{0}.png";

        private TextureRect _hoverSignifier;
        public string itemName;
        public TextureButton itemTextureButton;

        public Texture InventoryItemResource =>
            GD.Load<Texture>(string.Format(SpritePath,
                itemName.ToLower().Replace(" ", "_")));


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