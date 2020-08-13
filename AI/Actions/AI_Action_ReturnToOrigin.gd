extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed_factor = 1


func perform(_stateMachine, _delta, _interrupt):
	var origin = _stateMachine.origin_position
	var move = (origin - _stateMachine.entity.position).normalized()
	.set_move(_stateMachine, move, move_speed_factor)
