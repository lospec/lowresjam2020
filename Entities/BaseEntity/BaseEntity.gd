extends KinematicBody2D

# Exported Variables
export(int) var max_health

# Public Variables
var current_health: int

func _ready():
	if max_health <= 0:
		return
	current_health = max_health
