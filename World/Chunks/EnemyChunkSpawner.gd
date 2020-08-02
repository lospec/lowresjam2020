extends Node2D

class_name EnemySpawner

export(Array, PackedScene) var enemies
export(Vector2) var chunk_size
export(float) var n_enemies

var enemy_data = []

# Called when the node enters the scene tree for the first time.
func _ready():
	# Number of enemies spawned until now
	var spawned_enemies = 0
	# Spawn pos
	var spawn_pos
	# Spawn probability
	var spawn_prob
	# Sorting enemies by spawn probability
	enemies.sort_custom(self, "sortEnemies")
	
	# Getting the spawn data
	for i in range (0, enemies.size()):
		enemy_data.append(enemies[i].get_node("SpawnData").get_script())
	
	# Spawning enemies
	while spawned_enemies < n_enemies:
		# Generating a random position
		spawn_pos = Vector2(randi() % chunk_size.x, randi() % chunk_size.y)
		# Generating a random probability
		spawn_prob = randi() % 100
		spawned_enemies += 1


func getMinProbEnemy(prob):
	for i in range(0, enemy_data.size()):
		if (enemy_data.spawn_probability > prob):
			return enemy_data

func sortEnemies(a, b):
	var a_object = a.instance()
	var b_object = b.instance()
	
	var a_prob = a_object.get_node("SpawnData").get_script().get_spawn_prob()
	var b_prob = b_object.get_node("SpawnData").get_script().get_spawn_prob()
	
	print(a_prob)
	
	if a_prob < b_prob:
		a.queue_free()
		b.queue_free()
		return true
	
	a.queue_free()
	b.queue_free()
	
	return false
