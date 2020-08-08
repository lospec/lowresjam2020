extends Node

# Public Variables
var going_to_main_menu = false

# Onready Variables
onready var animated_sprite = $AnimatedSprite

func _ready():
	PaletteSwap.enabled = false
	
	animated_sprite.frame = 0
	animated_sprite.playing = true

func _unhandled_input(event):
	if event is InputEventMouseButton or event is InputEventKey:
		if not going_to_main_menu:
			go_to_main_menu()

func _on_AnimatedSprite_animation_finished():
	if not going_to_main_menu:
		go_to_main_menu()

func go_to_main_menu():
	going_to_main_menu = true
	yield(Transitions.change_scene_double_transition("res://UI/Main Menu/Main Menu.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.3), "completed")
	PaletteSwap.enabled = true
