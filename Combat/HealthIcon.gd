extends TextureRect

signal stopped

var is_blinking = false
var _duration: float = 0
var _time_left: float = 0
var _freq: int

func _t() -> float: return _duration - _time_left

func _process(delta):
	if !is_blinking:
		return
	
	if _time_left <= 0:
		stop_blink()
		return
	
	if int(floor(_t() * _freq)) % 2 == 0:
		modulate.a = 0
	else:
		modulate.a = 1
	
	_time_left -= delta

func stop_blink():
	is_blinking = false
	_time_left = 0
	modulate.a = 1
	emit_signal("stopped")
	#print("STOP")

func blink(duration, frequency):
	if _time_left > 0:
		stop_blink()
	
	is_blinking = true
	_duration = duration
	_time_left = duration
	_freq = frequency

