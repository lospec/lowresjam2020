extends "res://Combat/CombatChar.gd"
#var CombatUtil = preload("res://Combat/CombatUtil.gd") # Used for auto-complete

# Public Variable
var pool_index = 0

func pool_char_to_action(c: String) -> int:
	c = c.to_lower()
	match c:
		"q":
			return CombatUtil.Combat_Action.QUICK
		
		"c":
			return CombatUtil.Combat_Action.COUNTER
		
		"h":
			return CombatUtil.Combat_Action.HEAVY
	
	return CombatUtil.Combat_Action.INVALID

func get_base_damage(action) -> int:
	var damage
	match action:
		CombatUtil.Combat_Action.QUICK:
			damage = char_instance.quick_damage
		
		CombatUtil.Combat_Action.HEAVY:
			damage = char_instance.heavy_damage
		
		CombatUtil.Combat_Action.COUNTER:
			damage = char_instance.counter_damage
	
	# Should probably make better calculation for this but this'll do for now
	damage = damage * (1 + CombatUtil.MULTIPLIER_PER_COMBO * hit_combo)
	return damage

func get_damage_type(action) -> String:
	match action:
		CombatUtil.Combat_Action.QUICK:
			print(char_instance.enemy_name)
			print(char_instance.quick_damage_type)
			return char_instance.quick_damage_type
		
		CombatUtil.Combat_Action.HEAVY:
			print(char_instance.enemy_name)
			print(char_instance.quick_damage_type)
			return char_instance.heavy_damage_type
		
		CombatUtil.Combat_Action.COUNTER:
			print(char_instance.enemy_name)
			print(char_instance.quick_damage_type)
			return char_instance.counter_damage_type
	
	return "None"

func get_status_effect(action) -> String:
	match action:
		CombatUtil.Combat_Action.QUICK:
			return char_instance.quick_status_effect
		
		CombatUtil.Combat_Action.HEAVY:
			return char_instance.heavy_status_effect
		
		CombatUtil.Combat_Action.COUNTER:
			return char_instance.counter_status_effect
	
	return "none"
	
func get_effect_chance(action) -> float:
	match action:
		CombatUtil.Combat_Action.QUICK:
			return char_instance.quick_effect_chance
		
		CombatUtil.Combat_Action.HEAVY:
			return char_instance.heavy_effect_chance
		
		CombatUtil.Combat_Action.COUNTER:
			return char_instance.counter_effect_chance
	
	return 0.0

#func take_damage(damage, type):
#	pass

# get_action() method can also be inherited to make different kinds of Combat AI, ex:
# - Enemy that decides from percentages of some enemy combat data
# - Enemy that checks what the player is doing on the previous move and act accordingly
func get_action():
	# HARDCODED INTERACTION: Should make a system to make this more flexible
	if char_instance.status_effects.has("Confused"):
		return CombatUtil.Combat_Action.NONE
		
	if char_instance.status_effects.has("Asleep"):
		return CombatUtil.Combat_Action.NONE
		
	if char_instance.status_effects.has("Frozen"):
		return CombatUtil.Combat_Action.NONE
	
	if "attack_pool" in char_instance:
		print(char_instance.attack_pool)
		var action = pool_char_to_action(char_instance.attack_pool[pool_index])
		
		if action != CombatUtil.Combat_Action.INVALID:
			pool_index = (pool_index + 1) % char_instance.attack_pool.length()
			return action
	
	return randi() % 3














