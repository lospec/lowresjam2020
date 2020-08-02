extends Node

export(Array) var states
export(bool) var active

export(Resource) var currentState

func _update_current_state():
	if not active:
		return
	currentState.update_state(self)
	
	
	

func _process(delta):
	_update_current_state()
