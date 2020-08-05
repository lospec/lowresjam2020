extends "res://Combat/CombatChar.gd"

func get_base_damage(action) -> int:
	var damage
	match action:
		combat_util.Combat_Action.QUICK:
			damage = char_instance.quick_damage
		
		combat_util.Combat_Action.HEAVY:
			damage = char_instance.heavy_damage
		
		combat_util.Combat_Action.COUNTER:
			damage = char_instance.counter_damage
	return damage

func get_action():
	return randi() % 3 # Placeholder: Should be integrated with attack pool system
	# Can also be inherited to make different kinds of Combat AI, ex:
	# - Enemy that decides from percentages of some enemy combat data
	# - Enemy that checks what the player is doing on the previous move and act accordingly
