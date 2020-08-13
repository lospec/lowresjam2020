extends MarginContainer

# Constants
const VOLUME_DIVISOR := 5.0

# Onready Variables
onready var volume_subtracted = linear2db(100.0 / VOLUME_DIVISOR)


func _on_Music_Volume_volume_value_updated(new_volume):
	AudioSystem.music_volume = linear2db(new_volume / VOLUME_DIVISOR) - \
			volume_subtracted


func _on_SFX_Volume_volume_value_updated(new_volume):
	AudioSystem.sfx_volume = linear2db(new_volume / VOLUME_DIVISOR) - \
			volume_subtracted
