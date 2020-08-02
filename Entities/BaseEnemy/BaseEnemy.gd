extends "res://Entities/BaseEntity/BaseEntity.gd"

# Public Variables
export(String) var in_game_name: String
var race: String
var spawn_prob: int
var can_respawn: bool
var level: int
var weakness: int
var resistance: int
var coin_drop_amount: int
var drop_table: Dictionary
var max_items_dropped: int
var attack_pool: String
var quick_damage: int
var quick_damage_type: int
var quick_status_effect: int
var quick_effect_chance: float
var heavy_damage: int
var heavy_damage_type: int
var heavy_status_effect: int
var heavy_effect_chance: float
var counter_damage: int
var counter_damage_type: int
var counter_status_effect: int
var counter_effect_chance: float

# Onready Variables
onready var stateMachine = $StateMachine

func _ready():
	if in_game_name == "":
		return
	
	var enemy_stats = Enemies.enemies_stats[in_game_name]
	for property in enemy_stats:
		var value = enemy_stats[property]
		if get(property) == null:
			push_error("No corresponding variable found for %s" % property)
		set(property, value)

func is_in_allowed_tile() -> bool:
	var is_spawn_safe_area2d = $IsSpawnSafeArea2D
	if not is_spawn_safe_area2d.get_overlapping_bodies():
		return true
	is_spawn_safe_area2d.queue_free() # The Area2D is now useless
	return false
