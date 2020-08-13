extends StatusEffect

var duration = 3

func _init():
	name = "Frozen"

func on_turn_end(combat_char: CombatChar):
	duration -= 1
	if duration <= 0:
		expired = true
