extends Node2D

class_name EnemySpawnData

export(float) var spawn_probability setget , get_spawn_prob

export(bool) var respawns
export(float) var respawn_time

func get_spawn_prob():
	return spawn_probability
