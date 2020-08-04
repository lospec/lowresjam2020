extends Node

# Onready Variables
onready var entities = $Entities

func _on_Chunks_enemy_instanced(enemy):
	entities.add_child(enemy)
