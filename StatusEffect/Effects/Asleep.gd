extends StatusEffect

var duration = 1

func _init():
	name = "Asleep"

func on_turn_end(combat_char: CombatChar):
	duration -= 1
	if duration <= 0:
		expired = true
