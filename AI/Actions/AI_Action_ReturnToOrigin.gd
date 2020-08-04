extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed = 1

func perform(_stateMachine, _delta, _interrupt):
	var origin = _stateMachine.origin_position

	_stateMachine.entity.velocity = (
		(origin - _stateMachine.entity.position).normalized()
		* move_speed
	)

