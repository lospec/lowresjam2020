extends Node

export (Array) var states
export (bool) var active

export (float) var timeToNewState = 2

export (Resource) var currentState

onready var entity = get_parent()

var _timer = timeToNewState;

func _update_current_state():
	if not active:
		return
	currentState.update_state(self)


func _process(_delta):
	_timer += _delta
	if _timer >= timeToNewState:
		_update_current_state()
		_timer = 0
