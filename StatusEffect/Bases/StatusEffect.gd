extends Resource
class_name StatusEffect

var name: String = "none"
var expired: bool = false
var initialized: bool = false

func on_turn_end(combat_char: CombatChar):
	pass

#extends Resource
#class_name StatusEffect
#
#export(String) var name = "none"
#
#var controller: StatusEffectController
#var char_instance setget ,get_char_instance
#
#func get_char_instance():
#	return controller.char_instance
#
#func is_finished() -> bool:
#	return false
#
#func on_turn_end():
#	pass
#
#func on_turn_start():
#	pass
#
#func modify_action(char_action):
#	return char_action
#
#func modify_damage(origin_damage):
#	return origin_damage
