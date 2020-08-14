extends Node

class_name CombatChar

# Signals
signal damage_taken(damage, damage_type)
#signal attack_char(target, damage)
#signal died()

# Constants


# Public Variables
var hit_combo: int = 0
var char_instance: BaseEntity

# warning-ignore:unused_argument
func take_damage(damage: int, damage_type: String):	
	print("TAKE DAMAGE: %s type %s" % [damage, damage_type])
	emit_signal("damage_taken", damage, damage_type)
	
	# HARDCODED INTERACTION: Should make a system to make this more flexible
	if char_instance.status_effects.has("Frozen") and damage_type == "Fire":
		char_instance.status_effects.erase("Frozen")
	
	char_instance.health -= damage
	
	if char_instance.health <= 0:
		char_instance.health = 0
	
	hit_combo = 0

func attack(target: CombatChar, action: int, damage: int):
	var damage_type = get_damage_type(action)
	target.take_damage(damage, damage_type)
	
	# Apply Stat Effect
	var se: String = get_status_effect(action)
	var sePercent: float = get_effect_chance(action)
	
	if randf() < sePercent:
		target.char_instance.add_status_effect(se)
		if "enemy_name" in target.char_instance:
			print("Applied %s to %s" % [se, target.char_instance.enemy_name])

# warning-ignore:unused_argument
func get_base_damage(action: int) -> int:
	return 0

# warning-ignore:unused_argument
func get_damage_type(action: int) -> String:
	return "None"

# warning-ignore:unused_argument
func get_status_effect(action: int) -> String:
	return "None"

# warning-ignore:unused_argument
func get_effect_chance(action: int) -> float:
	return 0.0

func get_action() -> int:
	return -1
