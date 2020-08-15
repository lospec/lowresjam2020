extends MarginContainer

# Onready Variables
onready var start_signifier_label = $StartSigniferMargin/StartSignifier
onready var start_signifier_animation_player = $StartSigniferMargin/AnimationPlayer

# Private Variables
var _changing_scene = false


func _ready():
	start_signifier_label.visible = false
	
	var heroes_guild_sfx = {
		AudioSystem.SFX.HEROES_GUILD_NOX_1: -10,
		AudioSystem.SFX.HEROES_GUILD_NOX_3: -10,
		AudioSystem.SFX.HEROES_GUILD_NOX_4: -10,
		AudioSystem.SFX.HEROES_GUILD_PUREASBESTOS_1: -10,
		AudioSystem.SFX.HEROES_GUILD_UNSETTLED_1: -12,
		AudioSystem.SFX.HEROES_GUILD_WILDLEOKNIGHT_2: -8,
	}
	
	var sfx = Utility.rand_element(heroes_guild_sfx.keys())
	var volume = heroes_guild_sfx[sfx]
	var sfx_player = AudioSystem.play_sfx(sfx,
			null, volume)
	
	yield(sfx_player, "finished")
	start_signifier_animation_player.play("flash")


func _unhandled_input(event):
	if event is InputEventKey and event.pressed and not _changing_scene:
		if OS.is_debug_build() and \
				event.scancode == KEY_F9 and event.shift:
			if get_tree().change_scene("res://AI/Editor/AI_Editor.tscn") != OK:
				print_debug("An error occured while attempting to change to the AI scene")
		else:
			AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
					Vector2.ZERO, -15)
			
			_changing_scene = true
			
			Transitions.change_scene_double_transition(
					"res://UI/character_selection/character_selector.tscn",
					Transitions.Transition_Type.SHRINKING_CIRCLE, 0.2)
