extends "res://AI/Actions/AI_State_Action.gd"

export (float) var search_range = 1

func perform(_stateMachine, _delta, _interrupt):
	var target = null
	
	for body in _stateMachine.find_bodies_in_range(search_range):
		if body.is_in_group("PlayerGroup"):
			target = body as BaseEntity
			break
	
	_stateMachine.target = target
