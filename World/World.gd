extends Node

# Onready Variables
onready var entities = $Entities
onready var combat = $Combat

func _on_Chunks_enemy_instanced(enemy):
	entities.add_child(enemy)
	enemy.connect("health_changed", combat, "_on_Enemy_health_changed")
