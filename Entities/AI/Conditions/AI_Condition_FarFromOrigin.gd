extends AI_State_Condition
class_name AI_Condition_FarFromOrigin

export (float) var distance_from_origin

func evaluate(stateMachine):
	return stateMachine.distance_to_origin > distance_from_origin
