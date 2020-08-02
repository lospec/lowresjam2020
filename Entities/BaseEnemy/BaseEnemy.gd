extends "res://Entities/BaseEntity/BaseEntity.gd"

# Public Variables
var level: int
var weakness
var resistance
var coin_drop_amount: int
var drop_table: Dictionary
var max_items_dropped: int
var attack_pool: String
var quick_damage: int
var quick_type
var quick_status_effect
var quick_effect_chance: float
var heavy_damage: int
var heavy_type
var heavy_status_effect
var heavy_effect_chance: float
var counter_damage: int
var counter_type
var counter_status_effect
var counter_effect_chance: float

# Onready Variables
onready var stateMachine = $StateMachine
