extends Node

class_name CombatChar

# Signals
signal damage_taken(new_health)

# Constants

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var hit_combo = 0

var char_instance

func take_damage(dmg):
	char_instance.health -= dmg
	hit_combo = 0

func get_base_damage(action) -> int:
	return 0

func get_action() -> int:
	return -1
