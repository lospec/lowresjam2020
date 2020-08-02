extends "res://Entities/BaseEntity/BaseEntity.gd"

# Public Variables
var in_game_name: String
var level: int
var weakness: int
var resistance: int
var coin_drop_amount: int
var drop_1 # Will be replaced with a drop table once the spreadsheet is updated
var drop_1_chance
var max_items_dropped: int
var attack_pool: String
var quick_damage: int
var heavy_damage: int
var counter_damage: int
var battle_texture: StreamTexture

func set_enemy(enemy_name):
	in_game_name = enemy_name
	var enemy_stats = EnemyStats.enemies_stats[enemy_name]
	for property in enemy_stats.keys():
		var value = enemy_stats[property]
		if not get(property):
			print_debug("The %s enemy property does not have a corresponding variable." % property)
		set(property, value)
