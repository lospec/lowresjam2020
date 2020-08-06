extends Node

class_name CombatChar

# Signals
signal health_changed(new_health)

# Constants
#const DAMAGE_VARIATION_DOWN = 2
#const DAMAGE_VARIATION_UP = 3

# Public Variables
var COMBAT_ACTION = preload("res://Combat/CombatAction.gd")
var health setget set_health
var base_quick_damage
var base_heavy_damage
var base_counter_damage

func set_health(val):
	health = val
	emit_signal("health_changed", health)

func GetBaseDamage(action):
	var damage
	match action:
		COMBAT_ACTION.QUICK:
			damage = base_quick_damage
		COMBAT_ACTION.HEAVY:
			damage = base_heavy_damage
		COMBAT_ACTION.COUNTER:
			damage = base_counter_damage
	print(damage)
	print(damage)
	return damage
