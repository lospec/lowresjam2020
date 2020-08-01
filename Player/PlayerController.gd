extends Sprite
# PLACEHOLDER CLASS MADE JUST TO TEST THE CAMERA MOVEMENT
class_name PlayerController

const SPEED = 20

func _process(delta):
	global_position += Vector2(
		Input.get_action_strength("player_move_right") -
		Input.get_action_strength("player_move_left"),
		Input.get_action_strength("player_move_down") - 
		Input.get_action_strength("player_move_up")
	) * delta * SPEED
	
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
