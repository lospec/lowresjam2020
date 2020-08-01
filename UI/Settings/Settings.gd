extends MarginContainer

func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		exit_to_main_menu()

func _on_Audio_pressed():
	if get_tree().change_scene("res://UI/Settings/Audio Settings/Audio Settings.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")

func _on_Controls_pressed():
	if get_tree().change_scene("res://UI/Settings/Control Settings/Control Settings.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")

func _on_Accessibility_pressed():
	if get_tree().change_scene("res://UI/Settings/Accessibility Settings/Accessibility Settings.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")

func exit_to_main_menu():
	if get_tree().change_scene("res://UI/Main Menu/Main Menu.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")
