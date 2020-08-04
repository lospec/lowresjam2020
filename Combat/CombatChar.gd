extends Node

class_name CombatChar

# Signals
signal HpChanged(newHp)

# Exported Variables
export var hp = 100 setget HpSetter

func HpSetter(val):
	hp = val
	emit_signal("HpChanged", hp)

func GetDamage():
	return Utility.randomRange(15, 21)
