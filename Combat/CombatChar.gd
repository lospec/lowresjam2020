extends Node

class_name CombatChar

# Signals
signal damage_taken(new_health)
signal died()

# Constants

# Exported Variables
export(Resource) var statEffect

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var hit_combo = 0

var char_instance

func take_damage(dmg):
	char_instance.health -= dmg
	
	if char_instance.health <= 0:
		char_instance.health = 0
	
	hit_combo = 0

#warning-ignore:unused_argument
func get_base_damage(action) -> int:
	return 0

func get_action() -> int:
	return -1
