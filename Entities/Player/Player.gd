extends "res://Entities/BaseEntity/BaseEntity.gd"

# Signals
signal enemy_detected(player, enemy)

# Public Variables
var combat_util := preload("res://Combat/CombatUtil.gd")
var coins = 0
var inventory := ["Stick", "Gem", "Hotdog", "Stick"]
var equipped_weapon := "Stick"
#var equipped_armor := "Loin Cloth"

# Onready Variables
onready var equipped_weapon_data: Dictionary = Data.item_data["Stick"]
#onready var equipped_armor_data: Dictionary = Data.item_data["Loin Cloth"]


func _physics_process(_delta):
	var input_vel := Vector2()
	input_vel.x = Input.get_action_strength("player_move_right") - Input.get_action_strength("player_move_left")
	input_vel.y = Input.get_action_strength("player_move_down") - Input.get_action_strength("player_move_up")
	velocity = input_vel.normalized() * SPEED


func _on_EntityDetector_body_entered(body):
	if body.is_in_group("enemies"):
		emit_signal("enemy_detected", self, body)


func get_base_damage(action):
	var damage
	match action:
		combat_util.Combat_Action.QUICK:
			damage = equipped_weapon_data.quick_damage
		
		combat_util.Combat_Action.HEAVY:
			damage = equipped_weapon_data.heavy_damage
		
		combat_util.Combat_Action.COUNTER:
			damage = equipped_weapon_data.counter_damage
	return damage
