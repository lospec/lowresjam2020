extends Camera2D

class_name CameraController

const Utility = preload("../Utility/Utility.gd")
const SMOOTH_SPEED = 5
const SPEED = 15

var vertical_movement_enabled = true
var horizontal_movement_enabled = true
# Toggle this to lock the camera
var is_locked = false
# The object the camera will follow
var object_to_follow
# Leftovers vector2 are added when it's not possible to move
# the camera because it's not in an integer position
var leftover = Vector2()

# Called when the node enters the scene tree for the first time.
func _ready():
	print(get_tree().root)
	object_to_follow = get_node("/root/Node2D/16x16")

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
			
		# Adding to the leftover vec
		leftover += difference * SMOOTH_SPEED * delta
		# Setting the position
		global_position = (global_position + leftover).round()
		
		# Checking if I have to reset the leftover vec
		
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
