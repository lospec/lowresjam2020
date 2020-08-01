extends KinematicBody2D

# Constants
const SPEED = 1#10#20#20#1#20#0.25

# Public Variables
var velocity = Vector2()
var velocity_leftover = Vector2()

func _unhandled_input(_event):
	var input_vel = Vector2()
	input_vel.x = Input.get_action_strength("player_move_right") - Input.get_action_strength("player_move_left")
	input_vel.y = Input.get_action_strength("player_move_down") - Input.get_action_strength("player_move_up")
	#print("Input Vel: %s" % input_vel)
	velocity = input_vel.normalized() * SPEED

func _physics_process(delta):
	pass
	#position += velocity
	#print(position)
	#print("Velocity Before: %s" % velocity)
	#position += velocity
	#move_and_slide(velocity)
	#position = position.ceil()
	#move_and_slide(velocity)#(velocity * delta)
	#move_and_collide(velocity)
	#position = position.ceil()
	#print("Position: %s" % position)
	#position = stepify_vec(position, 0.1)#position.step
	#print("Position: %s" % position)

#func stepify_vec(vec, step):
#	return Vector2(stepify(vec.x, step), stepify(vec.y, step))
