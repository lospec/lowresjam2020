extends Label

export(float) var float_speed = 2 # in px per seconds
export(float) var fade_duration = 2 # seconds until invisible
export(float) var fade_delay = 0 # seconds until starts fading

onready var a = 1 + fade_delay / fade_duration

func _process(delta):
	rect_position.y -= delta * float_speed
	a = max(0, a - delta / fade_duration)
	modulate.a = a
	
	if modulate.a <= 0:
		self.queue_free()
