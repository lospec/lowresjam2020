extends Resource

var _is_init = false


func set_move(stateMachine, direction: Vector2, speed_factor: float):
	var velocity = direction * (stateMachine.entity.move_speed * speed_factor)
	stateMachine.entity.velocity = velocity
	return velocity

func set_velocity(stateMachine, velocity: Vector2):
	stateMachine.entity.velocity = velocity


func _on_start():
	pass


func perform(_stateMachine, _delta, _interrupt):
	if not _is_init:
		_is_init = true
		_on_start()
