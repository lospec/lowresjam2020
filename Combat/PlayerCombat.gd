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
	
	# Should probably make better calculation for this but this'll do for now
	damage = damage * (1 + combat_util.MULTIPLIER_PER_COMBO * hit_combo)
	return damage

# Notes: A&
# My original plan was to make this scirpt to use the get_action() to determine the player action
# and the Combat.gd will wait for this script to use the CombatMenu.gd signals i talked about
# but the problem is i can't get the Combat.gd to yield for get_action() while also returning action value to use
# my solution was to make the CombatChar scripts to emit something like turn_set(action) and the Combat.gd 
# will listen for that signal, if the signal is received and both the the enemy and the player has set their turn
# then the TakeTurn method is run but i'm not confident enough to pull this off
# aha, i found a better way, should i try to implement it? (see: TakeTurn flee implementation)
