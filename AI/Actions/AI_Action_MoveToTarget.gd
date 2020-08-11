extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed_factor = 1


func perform(_stateMachine, _delta, _interrupt):
	var target = _stateMachine.target
	if not target:
		return
	var move = (target.position - _stateMachine.entity.position).normalized()
	.set_move(_stateMachine, move, move_speed_factor)
