extends AI_State_Action
class_name AI_Action_MoveToTarget

export (float) var move_speed


func perform(_stateMachine, _delta, _interrupt):
	var target = _stateMachine.target
	if not target:
		return

	_stateMachine.entity.velocity = (
		(target.position - _stateMachine.entity.position).normalized()
		* move_speed
	)
