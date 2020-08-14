extends Node

# Public Variables
var base_enemy_scene = load("res://Entities/enemies/base_enemy/base_enemy.tscn")

# Onready Variables
onready var map = $Map
onready var enemy_spawns = $EnemySpawns
onready var combat = $Combat
onready var combat_menu = combat.get_node("CombatMenu")
onready var pause_menu = $PauseMenu
onready var player = map.get_node("Player")
onready var dropped_items_gui = $DroppedItems


func _ready():
	player.position = SaveData.world_position
	
	if AudioSystem.currently_playing_music == AudioSystem.Music.NONE:
		AudioSystem.play_music(AudioSystem.Music.OVERWORLD, -30)
	
	spawn_enemies()


func spawn_enemies():
	for enemy_spawn in enemy_spawns.get_children():
		spawn_enemy(enemy_spawn)


func spawn_enemy(enemy_spawn):
	if enemy_spawn.enemies.size() == 0:
		push_error("%s enemy spawn has no enemy names attached." %
				enemy_spawn)
		return
	
	if enemy_spawn.num_enemies <= 0:
		push_error("%s enemy spawn's num_enemies not greater than 0" %
				enemy_spawn)
		return
	
	for _i in enemy_spawn.num_enemies:
		var enemy = null
		var safe = false
		
		enemy = base_enemy_scene.instance()
		var enemy_name = Utility.rand_element(enemy_spawn.enemies)
		if enemy_name == null:
			push_error("%s enemy spawn has a null enemy attached."
					% enemy_spawn.name)
			return
		enemy.load_enemy(enemy_name)
		
		while enemy == null or not safe:
			enemy.position = enemy_spawn.get_random_global_pos()
			map.add_child(enemy)
			
			safe = enemy.is_in_allowed_tile()
			
			if not safe:
				enemy.queue_free()
		
		enemy.connect("died", self, "_on_Enemy_death", [enemy_spawn])
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


func _on_Enemy_death(_enemy_instance, enemy_spawn_instance):
	var timer = Timer.new()
	add_child(timer)
	timer.start(rand_range(5, 15))
	timer.connect("timeout", self, "spawn_enemy", [enemy_spawn_instance])
	timer.connect("timeout", timer, "queue_free")
