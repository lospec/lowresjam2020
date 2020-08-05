extends Node

class_name CombatChar

# Signals
#signal health_changed(new_health)

# Constants

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var char_instance

func get_base_damage(action) -> int:
	return 0

func get_action() -> int:
	return -1
