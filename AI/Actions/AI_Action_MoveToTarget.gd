extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed = 1

func perform(_stateMachine, _delta, _interrupt):
	var target = _stateMachine.target
	if not target:
		return

	_stateMachine.entity.velocity = (
		(target.position - _stateMachine.entity.position).normalized()
		* move_speed
	)
