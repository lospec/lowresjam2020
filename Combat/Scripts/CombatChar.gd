extends Node

class_name CombatChar

signal HpChanged(newHp)

export var hp = 100 setget HpSetter
func HpSetter(val):
	hp = val
	emit_signal("HpChanged", hp)

func GetDamage():
	return randi() % 6 + 15
