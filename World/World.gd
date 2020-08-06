extends Node

# Onready Variables
onready var entities_and_static_objects = $EntitiesAndStaticObjects
onready var combat = $Combat
onready var pause_menu = $PauseMenu
onready var player = entities_and_static_objects.get_node("Player")


func _on_Chunks_enemy_instanced(enemy):
	entities_and_static_objects.add_child(enemy)
	enemy.connect("health_changed", combat, "_on_Enemy_health_changed")


func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		pause_menu.toggle_pause(player)
