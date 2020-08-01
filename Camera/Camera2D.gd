extends Camera2D

# Exported Variables
export (NodePath) var node_to_follow

# Public Variables
var node_to_follow_reference

func _ready():
	node_to_follow_reference = get_node(node_to_follow)
	get_parent().call_deferred("remove_child", self)
	node_to_follow_reference.call_deferred("add_child", self)

func _process(delta):
	print(position)
