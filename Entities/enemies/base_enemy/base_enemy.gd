extends "res://Entities/BaseEntity/BaseEntity.gd"

const MAX_SPEED = 40
const MIN_SPEED = 5

# Signals
signal stats_loaded
signal died(enemy)

# Exported Variables
export (String) var enemy_name

# Public Variables
var battle_texture: AtlasTexture
var race: String
var level: int
var weakness: String
var resistance: String
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
var quick_damage_type: String
var quick_status_effect: String
var quick_effect_chance: float
var heavy_damage: int
var heavy_damage_type: String
var heavy_status_effect: String
var heavy_effect_chance: float
var counter_damage: int
var counter_damage_type: String
var counter_status_effect: String
var counter_effect_chance: float

# Onready Variables
onready var stateMachine = $StateMachine


func _ready():
	if race == "" and enemy_name != "":
		load_enemy(enemy_name)
		if max_health <= 0:
			return
		health = max_health


func load_enemy(enemy_data_name):
	# Check for data
	if not Data.enemy_data.has(enemy_data_name):
		push_error("Enemy data for %s not found" % enemy_data_name)
		return
  
	enemy_name = enemy_data_name

	# Set Properties
	var enemy_stats = Data.enemy_data[enemy_data_name]
	for property in enemy_stats:
		var value = enemy_stats[property]
		if get(property) == null and not property in ["ai_type"]:
			push_error("No corresponding variable found for %s" % property)
		# Interpolate Speed Stat
		if property == "move_speed":
			value = Data.get_lerped_speed_stat(value, MIN_SPEED, MAX_SPEED)
		set(property, value)

	# Set Battle Textures
	battle_texture = AtlasTexture.new()
	battle_texture.atlas = load(
		"res://Entities/enemies/sprites/%s_Battle.png" % enemy_data_name
	)

	# Wait for onready variables to be set
	if sprite == null or stateMachine == null:
		yield(self, "ready")

	# Set Sprite Texture
	sprite.texture = load(
		"res://Entities/enemies/sprites/%s_Overworld.png" % enemy_data_name
	)

	# Set AI Resource
	stateMachine.behaviour = load(
		"res://AI/Resources/%s_behaviour.tres" % enemy_stats.ai_type.to_lower()
	)

	emit_signal("stats_loaded")


func is_in_allowed_tile() -> bool:
	var is_spawn_safe_area2d = $IsSpawnSafeArea2D
	if not is_spawn_safe_area2d.get_overlapping_bodies():
		return true
	is_spawn_safe_area2d.queue_free()  # The Area2D is now useless
	return false


func die():
	emit_signal("died", [self])
	queue_free()
