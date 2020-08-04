extends Node

# Signal
signal enemy_instanced(enemy)

func _ready():
	yield(get_parent(), "ready")
	
	for chunk in get_children():
		for enemy_spawn in chunk.enemy_spawns.get_children():
			var enemy_scene = Utility.rand_element(enemy_spawn.enemies)
			var enemy = enemy_scene.instance()
			enemy.position = enemy_spawn.position
			emit_signal("enemy_instanced", enemy)
