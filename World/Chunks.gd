extends Node

# Signal
signal enemy_instanced(enemy)

# Public Variables
var base_enemy_scene = load("res://Entities/enemies/base_enemy/base_enemy.tscn")


func _ready():
	yield(get_parent(), "ready")
	
	for chunk in get_children():
		for enemy_spawn in chunk.enemy_spawns.get_children():
			if enemy_spawn.enemies.size() == 0:
				continue
			
			var enemy = base_enemy_scene.instance()
			var enemy_name = Utility.rand_element(enemy_spawn.enemies)
			enemy.load_enemy(enemy_name)
			enemy.position = enemy_spawn.global_position
			emit_signal("enemy_instanced", enemy)
