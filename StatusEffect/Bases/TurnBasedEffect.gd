#extends StatusEffect
#class_name TurnBasedEffect
#
#export(int) var duration = -1
#
#var turn_elapsed = 0
#
#func is_finished() -> bool:
#	return turn_elapsed >= duration
