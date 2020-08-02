extends Resource
class_name AI_State

export (Array, Resource) var actions
export (Array, Resource) var transitions


func update_state(stateMachine, _delta):
	_perform_actions(stateMachine, _delta)
	_check_all_transitions(stateMachine)


func _perform_actions(stateMachine, _delta):
	var _interrupt = false

	for item in actions:
		if typeof(item) != typeof(AI_State_Action):
			print("%s is not of StateAction type." % item)

		var action: AI_State_Action = item
		action.perform(stateMachine, _delta, _interrupt)
		if _interrupt:
			return


func _check_all_transitions(stateMachine):
	for item in transitions:
		if typeof(item) != typeof(AI_Transition):
			print("%s is not of StateTransition type." % item)
		var transition: AI_Transition = item
		var is_true = transition.condition.evaluate(stateMachine)
		if stateMachine.transition_to_state(
			transition.trueState if is_true else transition.falseState
		):
			return
