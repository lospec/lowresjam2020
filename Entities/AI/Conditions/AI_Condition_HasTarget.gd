extends "res://Entities/AI/Conditions/AI_State_Condition.gd"

func evaluate(stateMachine):
	if stateMachine.target != null:
		return true
	return false
