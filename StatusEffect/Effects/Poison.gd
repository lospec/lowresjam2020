extends StatusEffect

var duration = 3
var damage = 1

func _init():
	name = "Poison"

func on_turn_end(combat_char: CombatChar):
	combat_char.take_damage(damage, "None")
	duration -= 1
	
	if duration <= 0:
		expired = true

#extends TurnBasedEffect
#class_name DamagePerTurn
#
#export(int) var damage = 1
#export(int) var frequency = 1
#export(String) var damage_type = "none"
#
#var _next_damage_turn = 0
#
#func on_turn_end():
#	if "resistance" in combat_char.char_instance and \
#		combat_char.char_instance.resistance == damage_type:
#		return
#
#	_next_damage_turn -= 1
#
#	if (_next_damage_turn <= 0):
#		combat_char.take_damage(damage)
#		_next_damage_turn = frequency
