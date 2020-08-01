extends MarginContainer

func _on_Play_Button_pressed():
	if get_tree().change_scene("res://Levels/Level.tscn") != OK:
		print_debug("An error occured while attempting to change to the level scene")

func _on_Credits_Button_pressed():
	if get_tree().change_scene("res://UI/Credits/Credits.tscn") != OK:
		print_debug("An error occured while attempting to change to the credits scene")

func _on_Settings_Button_pressed():
	if get_tree().change_scene("res://UI/Settings/Settings.tscn") != OK:
		print_debug("An error occured while attempting to change to the settings scene")
