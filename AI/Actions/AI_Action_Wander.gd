extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed_factor = 1
export (float) var update_time = 1

var _timer: float
var _current_velocity: Vector2


func _on_start():
	_timer = update_time


func perform(_stateMachine, _delta, _interrupt):
	_timer += _delta
	if _timer >= update_time:
		_timer = 0
		var _move = Vector2(rand_range(-1, 1), rand_range(-1, 1))
		_current_velocity = .set_move(_stateMachine, _move, move_speed_factor)
		return

	.set_velocity(_stateMachine, _current_velocity)
