extends "res://Combat/CombatChar.gd"

class_name PlayerCombat

signal action_chosen(action)

func get_base_damage(action) -> int:
	var player_weapon_data = Data.item_data[char_instance.equipped_weapon]
	
	var damage
	match action:
		combat_util.Combat_Action.QUICK:
			damage = player_weapon_data.quick_damage
		
		combat_util.Combat_Action.HEAVY:
			damage = player_weapon_data.heavy_damage
		
		combat_util.Combat_Action.COUNTER:
			damage = player_weapon_data.counter_damage
	
	# Should probably make better calculation for this but this'll do for now
	damage = damage * (1 + combat_util.MULTIPLIER_PER_COMBO * hit_combo)
	return damage

func get_damage_type(action) -> int:
	var player_weapon_data = Data.item_data[char_instance.equipped_weapon]
	
	match action:
		combat_util.Combat_Action.QUICK:
			return player_weapon_data.quick_damage_type
		
		combat_util.Combat_Action.HEAVY:
			return player_weapon_data.heavy_damage_type
		
		combat_util.Combat_Action.COUNTER:
			return player_weapon_data.counter_damage_type
	
	return -1

func get_status_effect(action):
	pass
	
func get_effect_chance(action):
	pass

func get_action() -> int:
	var action = yield(self, "action_chosen")
	return action

func _on_CombatMenu_action_selected(action):
	emit_signal("action_chosen", action)

# Notes: A&
# My original plan was to make this scirpt to use the get_action() to determine the player action
# and the Combat.gd will wait for this script to use the CombatMenu.gd signals i talked about
# but the problem is i can't get the Combat.gd to yield for get_action() while also returning action value to use
# my solution was to make the CombatChar scripts to emit something like turn_set(action) and the Combat.gd 
# will listen for that signal, if the signal is received and both the the enemy and the player has set their turn
# then the TakeTurn method is run but i'm not confident enough to pull this off
# aha, i found a better way, should i try to implement it? (see: TakeTurn flee implementation)

