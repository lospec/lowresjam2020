extends StatusEffect

var duration = 2

func _init():
	name = "Confused"

func on_turn_end(combat_char: CombatChar):
	duration -= 1
	if duration <= 0:
		expired = true
