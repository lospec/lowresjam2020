extends TextureRect

class_name CombatAttackAnim

signal effect_done

var playing = false

func play(texture, color):
	self.texture = texture
	self.texture.current_frame = 0
	modulate = color
	visible = true
	playing = true
	yield(get_tree().create_timer(texture.frames / texture.fps), "timeout")
	playing = false
	visible = false
	emit_signal("effect_done")

