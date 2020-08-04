extends "res://AI/Conditions/AI_State_Condition.gd"

export (float) var distance_from_origin

func evaluate(stateMachine):
	return stateMachine.distance_to_origin > distance_from_origin
