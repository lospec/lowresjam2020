extends Camera2D

class_name CameraController

# Constants
const Utility = preload("../Utility/Utility.gd")
const SMOOTH_SPEED = 5
const SPEED = 10

# Public Variables
var vertical_movement_enabled = true
var horizontal_movement_enabled = true
var is_locked = false # Toggle this to lock the camera
var object_to_follow # The object the camera will follow
# Leftovers vector2 are added when it's not possible to move
# the camera because it's not in an integer position
var leftover = Vector2()

func _ready():
	print(get_tree().root)
	object_to_follow = get_parent()

func _process(delta):
	# The camera will update its position only if it isn't locked
	if !is_locked:
		var difference = Vector2()
		var other_position = object_to_follow.global_position
		var current_position = global_position
		
		# Changing y position only if it isn't locked
		if vertical_movement_enabled:
			difference.y = other_position.y - current_position.y
		else:
			difference.y = 0
		
		# Same for x position
		if horizontal_movement_enabled:
			difference.x = other_position.x - current_position.x
		else:
			difference.x = 0
		
		# Multiplying the difference for delta time and smoothspeed
		difference *= SMOOTH_SPEED * delta
		# Setting the position
		global_position = (global_position + difference + leftover).round()
		
		# If I didn't move, I still have a leftover movement
		if global_position.distance_to(current_position) < 0.01:
			# Adding to the leftover vec
			leftover += difference
		# If I moved, I have to reset the leftover vec
		else:
			leftover = Vector2(0, 0)
