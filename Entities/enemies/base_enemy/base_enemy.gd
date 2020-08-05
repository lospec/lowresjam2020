extends "res://Entities/BaseEntity/BaseEntity.gd"

# Exported Variables
export(String) var enemy_name

# Public Variables
var battle_texture_normal: StreamTexture
var battle_texture_hurt: StreamTexture
var race: String
var level: int
var weakness: int
var resistance: int
var coin_drop_amount: int
var item_drop_1: String
var item_drop_1_chance: float
var item_drop_2: String
var item_drop_2_chance: float
var item_drop_3: String
var item_drop_3_chance: float
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


func load_enemy(enemy_data_name):
	if not Data.enemy_data.has(enemy_data_name):
		push_error("Enemy data for %s not found" % enemy_data_name)
		return
	
	var enemy_stats = Data.enemy_data[enemy_data_name]
	for property in enemy_stats:
		var value = enemy_stats[property]
		if get(property) == null:
			push_error("No corresponding variable found for %s" % property)
		set(property, value)
	
	if sprite == null:
		yield(self, "ready")
	var enemy_name_lower = enemy_data_name.to_lower()
	sprite.texture = load("res://Entities/enemies/overworld_sprites/%s_overworld.png" % enemy_name_lower)
	battle_texture_normal = load("res://Entities/enemies/battle_sprites/%s_battle_normal.png" % enemy_name_lower)
	battle_texture_hurt = load("res://Entities/enemies/battle_sprites/%s_battle_hurt.png" % enemy_name_lower)


func is_in_allowed_tile() -> bool:
	var is_spawn_safe_area2d = $IsSpawnSafeArea2D
	if not is_spawn_safe_area2d.get_overlapping_bodies():
		return true
	is_spawn_safe_area2d.queue_free() # The Area2D is now useless
	return false
