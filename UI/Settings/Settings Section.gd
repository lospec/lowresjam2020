extends Node


func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		exit_to_main_settings()


func exit_to_main_settings():
	if get_tree().change_scene("res://UI/settings/Settings.tscn") != OK:
		print_debug("An error occured while attempting to change to the settings scene")
