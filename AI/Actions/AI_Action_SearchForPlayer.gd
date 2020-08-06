extends "res://AI/Actions/AI_State_Action.gd"

export (float) var search_range = 1

var _found_target: BaseEntity

var _initialized = false

func _on_body_entered(body):
	print("body entered")
	if body.is_in_group("PlayerGroup"):
		_found_target = body as BaseEntity
		
func _on_body_exited(body):
	if body.is_in_group("PlayerGroup"):
		_found_target = null

func perform(_stateMachine, _delta, _interrupt):
	if not _initialized:
		_stateMachine._circle_area.connect("body_entered", self, "_on_body_entered")
		_initialized = true

	_stateMachine._collision_circle_size = search_range
	if _found_target:
		print("found target")
		_stateMachine.target = _found_target
