extends Resource

var _is_init = false

func _on_start():
	pass

func perform(_stateMachine, _delta, _interrupt):
	if not _is_init:
		_is_init = true
		_on_start()