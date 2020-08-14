extends MarginContainer

# Onready Variables
onready var start_signifier_label = $StartSigniferMargin/StartSignifier
onready var start_signifier_animation_player = $StartSigniferMargin/AnimationPlayer

# Private Variables
var _changing_scene = false


func _ready():
	start_signifier_label.visible = false


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


func _on_StartSignifierDelay_timeout():
	start_signifier_animation_player.play("flash")
