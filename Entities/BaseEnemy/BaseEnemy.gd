extends "res://Entities/BaseEntity/BaseEntity.gd"

# Public Variables
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
