extends AI_State_Action
class_name AI_Action_ReturnToOrigin

export (float) var move_speed

func perform(_stateMachine, _delta, _interrupt):
	var origin = _stateMachine.origin_position

	_stateMachine.entity.velocity = (
		(origin - _stateMachine.entity.position).normalized()
		* move_speed
	)

