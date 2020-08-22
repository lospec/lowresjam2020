using Godot;

namespace HeroesGuild.UI.Settings.Audio_Settings
{
    public class VolumeControl : HBoxContainer
    {
        private const int INCREMENT_AMOUNT = 10;
        private const int DECREMENT_AMOUNT = 10;
        private const int STARTING_VOLUME = 100;
        private const int MAX_VOLUME = 100;
        private const int MIN_VOLUME = 0;

        [Signal] public delegate void VolumeValueUpdated(float volumeValue);

        public int volumeValue = STARTING_VOLUME;
        private Label valueLabel;

        public override void _Ready()
        {
            valueLabel = GetNode<Label>("Value");
        }

        private void OnIncreaseValue_Pressed()
        {
            if (volumeValue >= MAX_VOLUME)
            {
                return;
            }

            volumeValue += INCREMENT_AMOUNT;
            UpdateVolumeValue();
        }

        private void OnDecreaseValue_Pressed()
        {
            if (volumeValue <= MIN_VOLUME)
            {
                return;
            }

            volumeValue -= DECREMENT_AMOUNT;
            UpdateVolumeValue();
        }

        private void UpdateVolumeValue()
        {
            valueLabel.Text = $"{volumeValue}";
            EmitSignal(nameof(VolumeValueUpdated), volumeValue);
        }
    }
}