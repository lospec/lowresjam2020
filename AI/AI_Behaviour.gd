extends Resource
class_name AI_Behaviour

const AI_State = preload("res://AI/AI_State.gd")

export (Array) var states
export (int) var start_state_index

func set_starting_state(_stateMachine):
	_stateMachine.transition_to_state(start_state_index)
