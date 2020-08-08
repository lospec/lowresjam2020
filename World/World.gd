extends Node

# Onready Variables
onready var entities_and_static_objects = $EntitiesAndStaticObjects
onready var combat = $Combat
onready var pause_menu = $PauseMenu
onready var player = entities_and_static_objects.get_node("Player")
onready var chunks_collection = $Chunks


func _ready():
	player.position = SaveData.world_position


func _on_Chunks_enemy_instanced(enemy):
	entities_and_static_objects.add_child(enemy)
	enemy.connect("health_changed", combat, "_on_Enemy_health_changed")


func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		pause_menu.toggle_pause(player)


func _on_DoorDetection_body_entered(body):
	if not body.is_in_group("enemies"):
		var offset = Vector2(0, 5) # So the player doesn't immediately enter the guild hall again when they return
		SaveData.world_position = player.position + offset
		Transitions.change_scene_double_transition("res://guild_hall/guild_hall.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.3)
