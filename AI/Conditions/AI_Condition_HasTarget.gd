extends "res://AI/Conditions/AI_State_Condition.gd"

export (bool) var has_target

func evaluate(stateMachine):
	if has_target:
		return stateMachine.target != null
	else:
		return stateMachine.target == null
