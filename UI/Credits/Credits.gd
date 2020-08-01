extends MarginContainer

# Constants
const SCROLL_SPEED = 5
const MAX_SCROLL = 140
const ON_RESET_SCROLL = 64

# Public Variables
var can_scroll = false

func _process(delta):
	rect_position.y -= SCROLL_SPEED * delta
	if rect_position.y <= -MAX_SCROLL:
		rect_position.y = ON_RESET_SCROLL

func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		exit_to_main_menu()

func _on_Start_Scrolling_Delay_timeout():
	can_scroll = true

func exit_to_main_menu():
	if get_tree().change_scene("res://UI/Main Menu/Main Menu.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")
