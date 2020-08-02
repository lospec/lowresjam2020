extends AI_State_Condition
class_name AI_Condition_HasTarget

func evaluate(stateMachine):
	if stateMachine.target != null:
		return true
	return false
