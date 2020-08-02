extends AI_State_Condition
class_name AI_Condition_TargetInRange

export (float) var max_range


func evaluate(stateMachine):
	if not stateMachine.target:
		return false
	return stateMachine.distance_to_target < max_range
