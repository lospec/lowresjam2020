extends Node

# Constants
enum Music {
	NONE = -1,
	OVERWORLD,
}
const MUSIC_RESOURCES = {
	Music.OVERWORLD: preload("res://World/music/overworld.ogg"),
}

enum SFX {
	BUTTON_CLICK,
	BUTTON_CLICK_SHORT,
	TOKEN_3,
}
const SFX_RESOURCES = {
	SFX.BUTTON_CLICK: preload("res://sfx/click.wav"),
	SFX.BUTTON_CLICK_SHORT: preload("res://sfx/click_short.wav"),
	SFX.TOKEN_3: preload("res://sfx/token_3.wav"),
}

const FADE_IN_START_VOLUME = -80
const FADE_IN_DURATION = 0.5

# Public Variables
var currently_playing_music: int = Music.NONE

# Onready Variables
onready var music_player = $Music
onready var tween = $Tween


func play_music(music: int,
		volume_db: float = 0, pitch_scale: float = 1,
		fade_in: bool = true) -> void:
	currently_playing_music = music
	music_player.stream = MUSIC_RESOURCES[music]
	if fade_in:
		music_player.volume_db = FADE_IN_START_VOLUME
		tween.interpolate_property(music_player, "volume_db",
		FADE_IN_START_VOLUME, volume_db, FADE_IN_DURATION)
		tween.start()
	else:
		music_player.volume_db = volume_db
	music_player.pitch_scale = pitch_scale
	music_player.play()


func play_sfx(sfx: int, audio_position: Vector2 = Vector2.ZERO,
		volume_db: float = 0, pitch_scale: float = 1) -> void:
	var sfx_player = AudioStreamPlayer2D.new()
	sfx_player.position = audio_position
	sfx_player.stream = SFX_RESOURCES[sfx]
	sfx_player.volume_db = volume_db
	sfx_player.pitch_scale = pitch_scale
	add_child(sfx_player)
	sfx_player.play()
	sfx_player.connect("finished", self, "_on_SFXPlayer_finished", [sfx_player])

func _on_SFXPlayer_finished(sfx_player):
	sfx_player.queue_free()
