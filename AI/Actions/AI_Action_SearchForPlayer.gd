extends "res://AI/Actions/AI_State_Action.gd"

export (float) var search_range = 1
export (bool) var found_target = false

var _initialized = false

func _on_body_entered(body):
	if body.is_in_group("PlayerGroup"):
		found_target = body as BaseEntity
		
func _on_body_exited(body):
	if body.is_in_group("PlayerGroup"):
		found_target = null

func perform(_stateMachine, _delta, _interrupt):
	if not _initialized:
		_stateMachine._circle_area.connect("body_entered", self, "_on_body_entered")
		_initialized = true

	_stateMachine._collision_circle_size = search_range
	if found_target:
		_stateMachine.target = found_target
