extends Resource
class_name AIState

export (Array) var actions

func update_state(stateMachine):
	_perform_actions(stateMachine)
	
func _perform_actions(stateMachine):
	for item in actions:
		if item.get_type() != typeof(StateAction):
			print("%s is not of StateAction type." % item)
		
		var action: StateAction = item
		action.perform(stateMachine)
		
