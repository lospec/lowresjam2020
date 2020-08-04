extends Node

# Onready Variables
onready var animated_sprite = $AnimatedSprite

func _ready():
	animated_sprite.frame = 0
	animated_sprite.playing = true

func _unhandled_input(event):
	if event is InputEventMouseButton:
		go_to_main_menu()

func _unhandled_key_input(_event):
	go_to_main_menu()

func _on_AnimatedSprite_animation_finished():
	go_to_main_menu()

func go_to_main_menu():
	PaletteSwap.enabled = true
	if get_tree().change_scene("res://UI/Main Menu/Main Menu.tscn") != OK:
		print_debug("An error occured while attempting to change to the main menu scene")
