extends "res://Entities/AI/Conditions/AI_State_Condition.gd"

export (float) var max_range


func evaluate(stateMachine):
	if not stateMachine.target:
		return false
	return stateMachine.distance_to_target < max_range
