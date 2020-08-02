extends Camera2D

class_name CameraController

# Exported Variables
export (NodePath) var node_to_follow

# Public Variables
var node_to_follow_reference

func _enter_tree():
	node_to_follow_reference = get_node(node_to_follow)

func _process(delta):
	global_position = node_to_follow_reference.global_position
