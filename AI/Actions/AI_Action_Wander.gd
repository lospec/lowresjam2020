extends "res://AI/Actions/AI_State_Action.gd"

export (float) var move_speed = 1
export (float) var update_time = 1

var _timer: float
var _move: Vector2


func _on_start():
	_timer = update_time


func perform(_stateMachine, _delta, _interrupt):
	if _timer >= update_time:
		_timer = 0
		_move = Vector2(rand_range(-1, 1), rand_range(-1, 1)) * move_speed
	_stateMachine.entity.velocity = _move
	_timer += _delta
