extends Camera2D

class_name CameraController

# Constants
const Utility = preload("../Utility/Utility.gd")
const SMOOTH_SPEED = 5
const SPEED = 10

# Exported Variables
export (NodePath) var node_to_follow

# Public Variables
var vertical_movement_enabled = true
var horizontal_movement_enabled = true
var is_locked = false # Toggle this to lock the camera
var object_to_follow # The object the camera will follow
# Leftovers vector2 are added when it's not possible to move
# the camera because it's not in an integer position
var leftover = Vector2()
var node_to_follow_reference

func _enter_tree():
    node_to_follow_reference = get_node(node_to_follow)

func _process(delta):
    # The camera will update its position only if it isn't locked
    if !is_locked:
        var difference = Vector2()
        var other_position = node_to_follow_reference.global_position
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
        
        if (abs(node_to_follow_reference.global_position.x - global_position.x) <= 0.03 and
                abs(node_to_follow_reference.global_position.y - global_position.y) <= 0.03):
            return
        
        # Multiplying the difference for delta time and smoothspeed
        difference *= SMOOTH_SPEED * delta
        # Setting the position
        global_position = global_position + difference + leftover
        
        # If I didn't move, I still have a leftover movement
        if global_position.distance_to(current_position) < 0.01:
            # Adding to the leftover vec
            leftover += difference
        # If I moved, I have to reset the leftover vec
        else:
            leftover = Vector2(0, 0)
