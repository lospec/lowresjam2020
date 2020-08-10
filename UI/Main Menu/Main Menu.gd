extends MarginContainer

# Onready Variables
onready var buttons = $Middle/Buttons


func _ready():
	for button in buttons.get_children():
		button.connect("pressed", self, "_on_Button_pressed",
				[button])
		button.connect("mouse_entered", self, "_on_Button_mouse_entered",
				[button])


func _on_Play_Button_pressed():
	if get_tree().change_scene("res://World/World.tscn") != OK:
		print_debug("An error occured while attempting to change to the world scene")


func _on_Credits_Button_pressed():
	if get_tree().change_scene("res://UI/Credits/Credits.tscn") != OK:
		print_debug("An error occured while attempting to change to the credits scene")


func _on_Settings_Button_pressed():
	if get_tree().change_scene("res://UI/settings/Settings.tscn") != OK:
		print_debug("An error occured while attempting to change to the settings scene")


# DEBUG AI SCENE. REMOVE FOR RELEASE BUILD
func _unhandled_input(event):
	if event is InputEventKey:
		if event.pressed and event.scancode == KEY_F9:
			if event.shift:
				if get_tree().change_scene("res://AI/Editor/AI_Editor.tscn") != OK:
					print_debug("An error occured while attempting to change to the AI scene")


func _on_TransitionTest_Button_pressed():
	if get_tree().change_scene("res://Shaders/Testing/scenes/TransitionTest.tscn") != OK:
		print_debug("An error occured while attempting to change to the transition test scene")


func _on_Button_pressed(button):
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
			button.rect_global_position)


func _on_Button_mouse_entered(button):
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK_SHORT,
			button.rect_global_position)
