extends Node

# Onready Variables
onready var entities_and_static_objects = $EntitiesAndStaticObjects
onready var combat = $Combat
onready var combat_menu = combat.get_node("CombatMenu")
onready var pause_menu = $PauseMenu
onready var player = entities_and_static_objects.get_node("Player")
onready var chunks_collection = $Chunks
onready var dropped_items_gui = $DroppedItems


func _ready():
	player.position = SaveData.world_position
	
	if AudioSystem.currently_playing_music == AudioSystem.Music.NONE:
		AudioSystem.play_music(AudioSystem.Music.OVERWORLD, -30)


func _on_Chunks_enemy_instanced(enemy):
	entities_and_static_objects.add_child(enemy)
	enemy.connect("health_changed", combat, "_on_Enemy_health_changed")


func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel") and \
			not combat_menu.visible:
		if dropped_items_gui.margin.visible:
			dropped_items_gui.close()
		else:
			pause_menu.toggle_pause(player)


func _on_DoorDetection_body_entered(body):
	if not body.is_in_group("enemies"):
		var offset = Vector2(0, 5) # So the player doesn't immediately enter the guild hall again when they return
		SaveData.world_position = player.position + offset
		Transitions.change_scene_double_transition("res://guild_hall/guild_hall.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.3)


func _on_Combat_combat_done(player_win, enemy_instance):
	if not player_win:
		return
	
	combat_menu.visible = false
	get_tree().paused = false
	
	enemy_instance.die()
	dropped_items_gui.drop_items(enemy_instance.enemy_name, player)
