extends MarginContainer

func _on_Audio_pressed():
	print_debug("Nothin' to see here.")

func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		exit_to_main_menu()

func exit_to_main_menu():
	if get_tree().change_scene("res://UI/Main Menu/Main Menu.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")
