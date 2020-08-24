using Godot;

namespace HeroesGuild.UI.pause_menu
{
    public class SettingsMargin : MarginContainer
    {
        private const float VOLUME_DIVISOR = 5.0f;

        private float volumeSubtracted = GD.Linear2Db(100f / VOLUME_DIVISOR);

        private void OnMusicVolume_VolumeValueUpdated(float newVolume)
        {
            AudioSystem.MusicVolume = GD.Linear2Db(newVolume / VOLUME_DIVISOR) - volumeSubtracted;
        }

        private void OnSFXVolume_VolumeValueUpdated(float newVolume)
        {
            AudioSystem.SFXVolume = GD.Linear2Db(newVolume / VOLUME_DIVISOR) - volumeSubtracted;
        }
    }
}