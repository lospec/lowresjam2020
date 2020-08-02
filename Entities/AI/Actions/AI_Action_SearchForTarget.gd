extends AI_State_Action
class_name AI_Action_SearchForTarget

export (PackedScene) var target
export (float) var search_range


func perform(_stateMachine, _delta, _interrupt):
	if _stateMachine.target:
		if _stateMachine.distance_to_target <= search_range:
			return
		else:
			_stateMachine.target = null
			return


	for body in _stateMachine.find_bodies_in_range(search_range):
		if body.is_in_group("PlayerGroup"):
			_stateMachine.target = body as BaseEntity
			return

	
