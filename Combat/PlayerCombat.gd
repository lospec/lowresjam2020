extends "res://Combat/CombatChar.gd"

func get_base_damage(action) -> int:
	var damage
	match action:
		combat_util.Combat_Action.QUICK:
			damage = char_instance.equipped_weapon.quick_damage
		
		combat_util.Combat_Action.HEAVY:
			damage = char_instance.equipped_weapon.heavy_damage
		
		combat_util.Combat_Action.COUNTER:
			damage = char_instance.equipped_weapon.counter_damage
	return damage
