extends Node2D

class_name EnemySpawner

const Utility = preload("../../Utility/Utility.gd")

export(Array, PackedScene) var enemies
export(Vector2) var horizontal_bounds
export(Vector2) var vertical_bounds
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

	# Spawning enemies
	while spawned_enemies < n_enemies:
		randomize()
		# Generating a random position
		var x = Utility.randomRange(horizontal_bounds.x, horizontal_bounds.y)
		var y = Utility.randomRange(vertical_bounds.x, vertical_bounds.y)
		
		print("Coord: ", Vector2(x, y))
		
		# Converting it to global position
		spawn_pos = get_parent().get_node("TileMap").map_to_world(Vector2(x,y)) / 16

		# Generating a random probability
		spawn_prob = randi() % 100
		
		print(spawn_pos)
		
		# Instantiating the enemy
		var enemy = getMinProbEnemy(spawn_prob).instance()
		enemy.set_position(spawn_pos)
		add_child(enemy)

		spawned_enemies += 1


func getMinProbEnemy(prob):
	for i in range(0, enemies.size()):
		var current_enemy = enemies[i].instance()
		
		if (current_enemy.get_node("SpawnData").get_spawn_prob() > prob):
			current_enemy.queue_free()
			return enemies[i]
		current_enemy.queue_free()
	return enemies[enemies.size() - 1]

func sortEnemies(a, b):
	var a_object = a.instance()
	var b_object = b.instance()
	
	var a_prob = a_object.get_node("SpawnData").get_spawn_prob()
	var b_prob = b_object.get_node("SpawnData").get_spawn_prob()

	if a_prob < b_prob:
		a_object.queue_free()
		b_object.queue_free()
		return true
	
	a_object.queue_free()
	b_object.queue_free()
	
	return false
