extends MarginContainer

func _on_Play_Button_pressed():
	print_debug("There is no game, what do you expect to happen?")

func _on_Credits_Button_pressed():
	if get_tree().change_scene("res://UI/Credits/Credits.tscn") != OK:
		print_debug("An error occured while attempting to change to the credits scene")
