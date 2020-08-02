extends AI_State_Action
class_name AI_Action_WaitTime

export (float) var wait_period
export (float) var active_period

var _timer := 0.0


func perform(_stateMachine, delta: float, _interrupt: bool):
	_timer += delta
	if _timer >= wait_period && _timer > 0:
		_timer = -active_period
		return
	_interrupt = true
