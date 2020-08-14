extends Area2D

# Exported Variables
export(Array, String) var enemies

# Onready Variables
onready var collision_shape = $CollisionShape2D


func get_random_global_pos() -> Vector2:
	var center = collision_shape.global_position
	
	var size = collision_shape.shape.extents
	
	var pos = Vector2()
	pos.x = rand_range(center.x - size.x, center.x + size.x)
	pos.y = rand_range(center.y - size.y, center.y + size.y)
	
	return pos
