extends MarginContainer

# Constants
const SCROLL_SPEED = 5
const THANKS_GAP = 20
const MAX_THANKS_POS_Y = 0
const START_SCROLL_POS = 50

# Onready Variables
onready var main_credits_vbox = $"Main Credits"
onready var thanks_label = $Thanks

# Feels hacky but it works - Should be refactored if possible

func _on_Credits_sort_children():
	main_credits_vbox.rect_position.y = START_SCROLL_POS
	thanks_label.rect_position.y = main_credits_vbox.rect_size.y + THANKS_GAP + START_SCROLL_POS

func _process(delta):
	main_credits_vbox.rect_position.y -= SCROLL_SPEED * delta
	if thanks_label.rect_position.y > MAX_THANKS_POS_Y:
		thanks_label.rect_position.y -= SCROLL_SPEED * delta

func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		exit_to_main_menu()

func exit_to_main_menu():
	if get_tree().change_scene("res://UI/Main Menu/Main Menu.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")
