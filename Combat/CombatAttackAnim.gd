extends TextureRect

class_name CombatAttackAnim

signal effect_done

var playing = false

func play(texture, color, min_duration = 0):
	var timer: SceneTreeTimer
	if min_duration > 0:
		timer = get_tree().create_timer(min_duration)
	
	self.texture = texture
	self.texture.current_frame = 0
	modulate = color
	visible = true
	playing = true
	yield(get_tree().create_timer(texture.frames / texture.fps), "timeout")
	playing = false
	visible = false
	
	if min_duration > 0 and timer.time_left > 0:
		print(timer.time_left)
		yield(timer, "timeout")
		emit_signal("effect_done")
	else:
		emit_signal("effect_done")

