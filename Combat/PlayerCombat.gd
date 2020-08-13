extends "res://Combat/CombatChar.gd"

class_name PlayerCombat

signal action_chosen(action)

#func take_damage(damage, type):
#	pass

func get_base_damage(action) -> int:
	var player_weapon_data = Data.item_data[char_instance.equipped_weapon]
	
	var damage
	match action:
		CombatUtil.Combat_Action.QUICK:
			damage = player_weapon_data.quick_damage
		
		CombatUtil.Combat_Action.HEAVY:
			damage = player_weapon_data.heavy_damage
		
		CombatUtil.Combat_Action.COUNTER:
			damage = player_weapon_data.counter_damage
	
	# Should probably make better calculation for this but this'll do for now
	damage = damage * (1 + CombatUtil.MULTIPLIER_PER_COMBO * hit_combo)
	return damage / 2

func get_damage_type(action) -> String:
	var player_weapon_data = Data.item_data[char_instance.equipped_weapon]
	
	match action:
		CombatUtil.Combat_Action.QUICK:
			return player_weapon_data.quick_damage_type
		
		CombatUtil.Combat_Action.HEAVY:
			return player_weapon_data.heavy_damage_type
		
		CombatUtil.Combat_Action.COUNTER:
			return player_weapon_data.counter_damage_type
	
	return "None"

func get_status_effect(action) -> String:
	var player_weapon_data = Data.item_data[char_instance.equipped_weapon]
	
	match action:
		CombatUtil.Combat_Action.QUICK:
			return player_weapon_data.quick_status_effect
		
		CombatUtil.Combat_Action.HEAVY:
			return player_weapon_data.heavy_status_effect
		
		CombatUtil.Combat_Action.COUNTER:
			return player_weapon_data.counter_status_effect
	
	return "None"
	
func get_effect_chance(action) -> float:
	var player_weapon_data = Data.item_data[char_instance.equipped_weapon]
	
	match action:
		CombatUtil.Combat_Action.QUICK:
			return player_weapon_data.quick_effect_chance
		
		CombatUtil.Combat_Action.HEAVY:
			return player_weapon_data.heavy_effect_chance
		
		CombatUtil.Combat_Action.COUNTER:
			return player_weapon_data.counter_effect_chance
	
	return 0.0

func get_action() -> int:
	# HARDCODED INTERACTION: Should make a system to make this more flexible
	if char_instance.status_effects.has("Confused"):
		return CombatUtil.Combat_Action.NONE
		
	if char_instance.status_effects.has("Asleep"):
		return CombatUtil.Combat_Action.NONE
		
	if char_instance.status_effects.has("Frozen"):
		return CombatUtil.Combat_Action.NONE
	
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

