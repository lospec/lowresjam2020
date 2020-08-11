extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed_factor = 1
export (float) var update_time = 1

var _timer: float
var _current_velocity: Vector2


func set_new_velocity(_stateMachine):
	var _move = Vector2(rand_range(-1, 1), rand_range(-1, 1))
	_current_velocity = .set_move(_stateMachine, _move, move_speed_factor)


func _on_start(_stateMachine):
	_timer = update_time
	_stateMachine.connect_to_signal("on_collision", self, "set_new_velocity")


func perform(_stateMachine, _delta, _interrupt):
	_timer += _delta
	if _timer >= update_time:
		_timer = 0
		return

	.set_velocity(_stateMachine, _current_velocity)
