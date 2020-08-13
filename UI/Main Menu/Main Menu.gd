extends MarginContainer

# Onready Variables
onready var buttons = $ButtonMargin/Buttons


func _ready():
	for button in buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed",
				[button])
		button.connect("mouse_entered", self, "_on_Button_mouse_entered",
				[button])


func _on_Play_pressed():
	Transitions.change_scene_double_transition(
			"res://UI/character_selection/character_selector.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE, 0.2)


func _on_Credits_pressed():
	Transitions.change_scene_double_transition(
			"res://UI/Credits/Credits.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE, 0.2)


func _unhandled_input(event):
	if event is InputEventKey and OS.is_debug_build() and \
			event.scancode == KEY_F9 and event.pressed and event.shift:
		if get_tree().change_scene("res://AI/Editor/AI_Editor.tscn") != OK:
			print_debug("An error occured while attempting to change to the AI scene")


func _on_Button_pressed(button):
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
			button.rect_global_position, -15)


func _on_Button_mouse_entered(button):
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK_SHORT,
			button.rect_global_position, -20)
