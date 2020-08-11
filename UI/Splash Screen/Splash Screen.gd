extends Node

# Public Variables
var going_to_main_menu := false

# Onready Variables
onready var animated_sprite := $AnimatedSprite
onready var flag_animated_sprite := $Flag


func _ready():
	PaletteSwap.enabled = false
	
	animated_sprite.play("walk")
	flag_animated_sprite.visible = false


func _unhandled_input(event):
	if event is InputEventMouseButton or event is InputEventKey:
		if not going_to_main_menu:
			go_to_main_menu()


func _on_AnimatedSprite_animation_finished():
	var next_anim: String
	match animated_sprite.animation:
		"walk":
			next_anim = "plant_flag"
		"plant_flag":
			flag_animated_sprite.visible = true
			flag_animated_sprite.play()
	if next_anim != "":
		animated_sprite.play(next_anim)


func _on_Flag_animation_finished():
	if not going_to_main_menu:
		going_to_main_menu = true
		go_to_main_menu()


func go_to_main_menu():
	going_to_main_menu = true
	yield(Transitions.change_scene_double_transition("res://UI/Main Menu/Main Menu.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.15), "completed")
	PaletteSwap.enabled = true
