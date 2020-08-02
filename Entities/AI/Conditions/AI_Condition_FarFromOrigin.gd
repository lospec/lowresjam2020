extends AI_State_Condition
class_name AI_Condition_FarFromOrigin

export (float) var max_distance

func evaluate(stateMachine):
	return stateMachine.distance_to_origin > max_distance
