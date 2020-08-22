extends Node

# Public Variables
var going_to_main_menu := false


func _ready():
	PaletteSwap.enabled = false


func _unhandled_input(event):
	if event is InputEventMouseButton or event is InputEventKey:
		if not going_to_main_menu:
			go_to_main_menu()


func _on_AnimationPlayer_animation_finished(_anim_name):
	if not going_to_main_menu:
		going_to_main_menu = true
		go_to_main_menu()


func go_to_main_menu():
	going_to_main_menu = true
	yield(Transitions.change_scene_double_transition("res://UI/Main Menu/Main Menu.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.15), "completed")
	PaletteSwap.enabled = true
