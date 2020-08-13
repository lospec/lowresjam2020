extends StatusEffect

var duration = 4
var delay = 2
var damage = 4

var _next_damage = 0

func _init():
	name = "OnFire"

func on_turn_end(combat_char: CombatChar):
	_next_damage -= 1
	print("NEXT FIRE DAMAGE: %s" % _next_damage)
	if _next_damage <= 0:
		print("APPLY FIRE DAMAGE TO: %s" % combat_char.name)
		combat_char.take_damage(damage, "Fire")
		_next_damage = delay + 1
	
	duration -= 1
	if duration <= 0:
		expired = true
