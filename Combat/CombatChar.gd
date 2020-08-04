extends Node

class_name CombatChar

# Signals
signal health_changed(new_health)

# Exported Variables
var health setget set_health

func set_health(val):
	health = val
	emit_signal("health_changed", health)

func GetDamage():
	return Utility.randomRange(15, 21)
