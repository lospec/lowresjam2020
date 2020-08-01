extends Sprite
# PLACEHOLDER CLASS MADE JUST TO TEST THE CAMERA MOVEMENT
class_name PlayerController

const SPEED = 20

var movement_leftover = Vector2(0, 0)

func _process(delta):
	var current_position = global_position
	var movement = Vector2(
		Input.get_action_strength("player_move_right") -
		Input.get_action_strength("player_move_left"),
		Input.get_action_strength("player_move_down") - 
		Input.get_action_strength("player_move_up")
	) * delta * SPEED

	global_position = (global_position + movement + movement_leftover).round()
	
	if current_position.distance_to(global_position) < 0.01:
		movement_leftover += movement
	else:
		 movement_leftover = Vector2(0, 0)
	
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
