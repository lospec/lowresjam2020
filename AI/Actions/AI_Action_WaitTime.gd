extends "res://AI/Actions/AI_State_Action.gd"

export (float) var wait_period = 1
export (float) var active_period = 1

var _timer := 0.0


func perform(_stateMachine, delta: float, _interrupt: bool):
	_timer += delta
	if _timer >= wait_period && _timer > 0:
		_timer = -active_period
		return
	_interrupt = true
