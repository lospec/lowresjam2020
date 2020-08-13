extends "res://Combat/CombatChar.gd"
#var combat_util = preload("res://Combat/CombatUtil.gd") # Used for auto-complete

# Public Variable
var pool_index = 0

func pool_char_to_action(c: String) -> int:
	match c:
		"q":
			return combat_util.Combat_Action.QUICK
		
		"c":
			return combat_util.Combat_Action.COUNTER
		
		"h":
			return combat_util.Combat_Action.HEAVY
	
	return combat_util.Combat_Action.INVALID

func get_base_damage(action) -> int:
	var damage
	match action:
		combat_util.Combat_Action.QUICK:
			damage = char_instance.quick_damage
		
		combat_util.Combat_Action.HEAVY:
			damage = char_instance.heavy_damage
		
		combat_util.Combat_Action.COUNTER:
			damage = char_instance.counter_damage
	
	# Should probably make better calculation for this but this'll do for now
	damage = damage * (1 + combat_util.MULTIPLIER_PER_COMBO * hit_combo)
	return damage

# get_action() method can also be inherited to make different kinds of Combat AI, ex:
# - Enemy that decides from percentages of some enemy combat data
# - Enemy that checks what the player is doing on the previous move and act accordingly
func get_action():
	if "attack_pool" in char_instance:
		var action = pool_char_to_action(char_instance.attack_pool[pool_index])
		
		if action != combat_util.Combat_Action.INVALID:
			pool_index = (pool_index + 1) % char_instance.attack_pool.length()
			return action
	
	return randi() % 3 # Placeholder: Should be integrated with attack pool system
