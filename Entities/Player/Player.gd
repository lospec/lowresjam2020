extends "res://Entities/BaseEntity/BaseEntity.gd"

# constants
const SPEED = 20

# Public Variables
var velocity = Vector2()
var velocity_leftover = Vector2()

func _unhandled_input(_event):
	var input_vel = Vector2()
	input_vel.x = Input.get_action_strength("player_move_right") - Input.get_action_strength("player_move_left")
	input_vel.y = Input.get_action_strength("player_move_down") - Input.get_action_strength("player_move_up")
	velocity = input_vel.normalized() * SPEED

func _physics_process(_delta):
	velocity = move_and_slide(velocity)
