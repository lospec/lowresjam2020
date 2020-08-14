extends Node

#enum Status_Effect {
#	NONE     = 0,
#	ASLEEP   = 1 << 0,
#	ONFIRE   = 1 << 1,
#	FROZEN   = 1 << 2,
#	CHARGED  = 1 << 3,
#	WEAK     = 1 << 4,
#	CONFUSED = 1 << 5,
#	POISONED = 1 << 6,
#}
#
#static func has_effect(flag: int, effect: int) -> bool:
#	return (flag & effect) != 0
#
#static func effect_added(flag: int, effect: int) -> int:
#	return flag | effect
#
#static func effect_removed(flag: int, effect: int) -> int:
#	return flag & ~effect
#
#static func effect_by_name(name: String) -> int:
#	return Status_Effect.get(name.to_upper(), 0)

func get_status_effect(script_name: String) -> StatusEffect:
	var path = "res://StatusEffect/Effects/%s.gd" % script_name
	var status_effect = load(path).new()
	return status_effect

#var _stat_effects = {}
#
#func _add_stat_effect_from(path: String):
#	var dir = Directory.new()
#
#	if dir.open(path) == OK:
#		dir.list_dir_begin(true)
#		var file_name = dir.get_next()
#
#		while file_name != "":
#			var file_path = dir.get_current_dir() + "/" + file_name
#
#			if dir.current_is_dir():
#				#print("Found directory: " + file_name)
#				_add_stat_effect_from(file_path)
#			else:
#				#print("Found file: " + file_name)
#				var stat_effect: StatusEffect = load(file_path)
#
#				if _stat_effects.has(stat_effect.name):
#					print("WARNING: Multiple StatEffect named %s is found" % stat_effect.name)
#
#				_stat_effects[stat_effect.name] = stat_effect
#
#			file_name = dir.get_next()
#
#		dir.list_dir_end()
#	else:
#		print("An error occurred when trying to access status effects resource folder.")
#
#func _ready():
#	var path = "res://StatusEffect/EffectRes/"
#	_add_stat_effect_from(path)
#
#func get_status_effect(name: String) -> StatusEffect:
#	var effect = _stat_effects.get(name, null)
#
#	if effect != null:
#		effect = effect.duplicate()
#
#	return effect
