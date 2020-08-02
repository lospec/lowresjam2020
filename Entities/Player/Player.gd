extends "res://Entities/BaseEntity/BaseEntity.gd"


func _unhandled_input(_event):
	var input_vel = Vector2()
	input_vel.x = Input.get_action_strength("player_move_right") - Input.get_action_strength("player_move_left")
	input_vel.y = Input.get_action_strength("player_move_down") - Input.get_action_strength("player_move_up")
	velocity = input_vel.normalized() * SPEED
