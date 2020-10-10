using Godot;
using HeroesGuild.utility;

namespace HeroesGuild.ui.pause_menu
{
    public class SettingsMargin : MarginContainer
    {
        [Signal] public delegate void SaveDataReset();

        [Signal] public delegate void SaveDataLoadedManually();

        private const float VOLUME_DIVISOR = 5.0f;
        private readonly float _volumeSubtracted = GD.Linear2Db(100f / VOLUME_DIVISOR);

        private void OnMusicVolume_VolumeValueUpdated(float newVolume)
        {
            AudioSystem.MusicVolume =
                GD.Linear2Db(newVolume / VOLUME_DIVISOR) - _volumeSubtracted;
        }

        private void OnSFXVolume_VolumeValueUpdated(float newVolume)
        {
            AudioSystem.SFXVolume =
                GD.Linear2Db(newVolume / VOLUME_DIVISOR) - _volumeSubtracted;
        }

        private void _on_ResetSave_pressed()
        {
            var saveData = Autoload.Get<SaveData>();
            saveData.ResetSave();
            EmitSignal(nameof(SaveDataReset));
        }

        private void _on_LoadSave_pressed()
        {
            var saveData = Autoload.Get<SaveData>();
            saveData.LoadGame();
            EmitSignal(nameof(SaveDataLoadedManually));
        }
    }
}