extends Node2D

class_name EnemySpawnData

export(float) var spawn_probability setget , get_spawn_prob

export(bool) var respawns
export(float) var respawn_time
export(Array, String) var allowed_tilenames

func is_in_allowed_tile():
	var area2D = get_parent().get_node("Area2D")
	if area2D.get_overlapping_bodies().size() == 0:
		return true
	# The Area2D is now useless
	area2D.queue_free()
	return false
	
func get_spawn_prob():
	return spawn_probability
