#extends Resource
#class_name StatusEffectController
#
#var char_instance: BaseEntity setget _no_set
#var _status_effects = {}
#
## warning-ignore:unused_argument
#func _no_set(val):
#	pass
#
#func init(_char_instance: BaseEntity):
#	char_instance = _char_instance
#
#func add_effect(name: String):
#	var se = StatEffectCollection.get_status_effect(name)
#	if se == null:
#		print("Cannot find StatusEffect named %s" % name)
#		return
#
#	se.controller = self
#	_status_effects[name] = se
#
#func remove_effect(name: String):
#	_status_effects.erase(name)
#
#
