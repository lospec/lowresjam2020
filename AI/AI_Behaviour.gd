extends Resource
class_name AI_Behaviour

const AI_State = preload("res://AI/AI_State.gd")

export (Array) var states
export (int) var start_state_index

func get_starting_state() -> AI_State:
	return states[start_state_index]
