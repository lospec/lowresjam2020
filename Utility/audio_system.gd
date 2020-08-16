extends Node

# Constants
enum Music {
	NONE = -1,
	OVERWORLD,
	GUILD,
	BATTLE_BEAST,
	BATTLE_DEMON,
	BATTLE_FLORA,
	BATTLE_GNOME,
	BATTLE_HUMAN,
	BATTLE_ROBOT,
	BATTLE_SLIME,
	GAME_OVER,
}
const MUSIC_RESOURCES = {
	# Overworld
	Music.OVERWORLD: preload("res://music/overworld.ogg"),
	# Guild
	Music.GUILD: preload("res://music/guild.ogg"),
	# Battle
	Music.BATTLE_BEAST: preload("res://music/battle beast.ogg"),
	Music.BATTLE_DEMON: preload("res://music/battle demon.ogg"),
	Music.BATTLE_FLORA: preload("res://music/battle flora.ogg"),
	Music.BATTLE_GNOME: preload("res://music/battle gnome.ogg"),
	Music.BATTLE_HUMAN: preload("res://music/battle human.ogg"),
	Music.BATTLE_ROBOT: preload("res://music/battle robot.ogg"),
	Music.BATTLE_SLIME: preload("res://music/battle slime.ogg"),
	# Game Over
	Music.GAME_OVER: preload("res://music/game over.ogg"),
}

enum SFX {
	# UI
	BUTTON_CLICK,
	BUTTON_CLICK_SHORT,
	BUTTON_HOVER,
	DENY,
	TOKEN_3,
	# Ingame
	FOOTSTEP_1,
	FOOTSTEP_2,
	DOOR_OPEN,
	CHEST_OPEN,
	# Voiceover : title greetings
	HEROES_GUILD_NOX_1,
	HEROES_GUILD_NOX_3,
	HEROES_GUILD_NOX_4,
	HEROES_GUILD_PUREASBESTOS_1,
	HEROES_GUILD_UNSETTLED_1,
	HEROES_GUILD_WILDLEOKNIGHT_2,
	# Intros
	BATTLE_INTRO,
	# Jingles
	VICTORY_JINGLE,
	# Battle
	# Enemy takes damage
	BEAST_HURT,
	DEMON_HURT,
	FLORA_HURT,
	HUMAN_HURT,
	ROBOT_HURT,
	SLIME_HURT,
	# Player takes damage
	PLAYER_HURT,
	# Deal damage
	QUICK_ATTACK,
	HEAVY_ATTACK,
	COUNTER_ATTACK,
	# Status effect applied
	CHARGED_STATUS,
	CONFUSION_STATUS,
	ONFIRE_STATUS,
	FROZEN_STATUS,
	POISONED_STATUS,
	WEAK_STATUS,
}
const SFX_RESOURCES = {
	# UI
	SFX.BUTTON_CLICK: preload("res://sfx/ui_confirm.wav"),
	SFX.BUTTON_CLICK_SHORT: preload("res://sfx/click_short.wav"),
	SFX.BUTTON_HOVER: preload("res://sfx/ui_hover.wav"),
	SFX.DENY: preload("res://sfx/ui_deny.wav"),
	SFX.TOKEN_3: preload("res://sfx/token_3.wav"),
	# Ingame
	SFX.FOOTSTEP_1: preload("res://sfx/footstep_1.wav"),
	SFX.FOOTSTEP_2: preload("res://sfx/footstep_2.wav"),
	SFX.DOOR_OPEN: preload("res://sfx/door_open.wav"),
	SFX.CHEST_OPEN: preload("res://sfx/chest_open.wav"),
	# Voiceover : title greetings
	SFX.HEROES_GUILD_NOX_1: preload("res://sfx/Heroes_Guild_Nox.wav"),
	SFX.HEROES_GUILD_NOX_3: preload("res://sfx/Heroes_Guild_Nox-take3.wav"),
	SFX.HEROES_GUILD_NOX_4: preload("res://sfx/Heroes_Guild_Nox-take4.wav"),
	SFX.HEROES_GUILD_PUREASBESTOS_1: preload("res://sfx/Heroes_Guild_PureAsbestos_take1.wav"),
	SFX.HEROES_GUILD_UNSETTLED_1: preload("res://sfx/Heroes_Guild_Unsettled_take1.wav"),
	SFX.HEROES_GUILD_WILDLEOKNIGHT_2: preload("res://sfx/Heroes_Guild_WildLeoKnight_take2.wav"),
	# Intros
	SFX.BATTLE_INTRO: preload("res://sfx/battle intro.ogg"),
	# Jingles
	SFX.VICTORY_JINGLE: preload("res://sfx/Victory jingle.ogg"),
	# Battle
	# Enemy takes damage
	SFX.BEAST_HURT: preload("res://sfx/Beast_hit.wav"),
	SFX.DEMON_HURT: preload("res://sfx/Demon_hit.wav"),
	SFX.FLORA_HURT: preload("res://sfx/Flora_Hit.wav"),
	SFX.HUMAN_HURT: preload("res://sfx/Human_Hit.wav"),
	SFX.ROBOT_HURT: preload("res://sfx/Robot_hit.wav"),
	SFX.SLIME_HURT: preload("res://sfx/Slime_hit.wav"),
	# Player takes damage
	SFX.PLAYER_HURT: preload("res://sfx/Player_Hit.wav"),
	# Deal damage
	SFX.QUICK_ATTACK: preload("res://sfx/Quick.wav"),
	SFX.HEAVY_ATTACK: preload("res://sfx/Heavy.wav"),
	SFX.COUNTER_ATTACK: preload("res://sfx/Counter.wav"),
	# Status effect applied
	SFX.CHARGED_STATUS: preload("res://sfx/Charged.wav"),
	SFX.CONFUSION_STATUS: preload("res://sfx/Confusion.wav"),
	SFX.ONFIRE_STATUS: preload("res://sfx/Fire3.wav"),
	SFX.FROZEN_STATUS: preload("res://sfx/Ice.wav"),
	SFX.POISONED_STATUS: preload("res://sfx/Poison.wav"),
	SFX.WEAK_STATUS: preload("res://sfx/Weak.wav"),
}

const FADE_IN_START_VOLUME := -80
const FADE_IN_DURATION := 0.5

# Public Variables
var currently_playing_music: int = Music.NONE
var music_volume := 0.0 setget set_music_volume
var sfx_volume := 0.0 setget set_sfx_volume

# Onready Variables
onready var music_player = $Music
onready var tween = $Tween


func play_music(music: int,
		volume_db: float = 0, pitch_scale: float = 1,
		fade_in: bool = true) -> void:
	currently_playing_music = music
	music_player.stream = MUSIC_RESOURCES[music]
	volume_db += music_volume
	if fade_in:
		music_player.volume_db = FADE_IN_START_VOLUME
		tween.interpolate_property(music_player, "volume_db",
		FADE_IN_START_VOLUME, volume_db, FADE_IN_DURATION)
		tween.start()
	else:
		music_player.volume_db = volume_db
	music_player.pitch_scale = pitch_scale
	music_player.play()


func stop_music():
	currently_playing_music = Music.NONE
	music_player.stop()


func play_sfx(sfx: int, audio_position = null,
		volume_db: float = 0, pitch_scale: float = 1) -> AudioStreamPlayer2D:
	var sfx_player = null
	if audio_position:
		sfx_player = AudioStreamPlayer2D.new()
		sfx_player.position = audio_position
	else:
		sfx_player = AudioStreamPlayer.new()
	sfx_player.stream = SFX_RESOURCES[sfx]
	volume_db += sfx_volume
	sfx_player.volume_db = volume_db
	sfx_player.pitch_scale = pitch_scale
	add_child(sfx_player)
	sfx_player.play()
	sfx_player.connect("finished", self, "_on_SFXPlayer_finished", [sfx_player])
	sfx_player.add_to_group("sfx_players")
	
	return sfx_player


func _on_SFXPlayer_finished(sfx_player):
	sfx_player.queue_free()


func set_music_volume(value):
	value = max(-80, value)
	
	music_player.volume_db += value - music_volume
	
	music_volume = value


func set_sfx_volume(value):
	value = max(-80, value)
	
	for sfx_player in get_tree().get_nodes_in_group("sfx_players"):
		sfx_player.volume_db += value - sfx_volume
	
	sfx_volume = value
