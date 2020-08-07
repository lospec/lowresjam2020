extends Label

const FLOAT_SPEED = 2 # in px per seconds
const FADE_SPEED = 2 # seconds until invisible

export(float) var fade_delay = 0

onready var a = 1 + fade_delay / FADE_SPEED

func _process(delta):
	rect_position.y -= delta * FLOAT_SPEED
	a = max(0, a - delta / FADE_SPEED)
	modulate.a = a
	
	if modulate.a <= 0:
		self.queue_free()
