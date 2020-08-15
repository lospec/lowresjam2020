extends "res://Entities/BaseEntity/BaseEntity.gd"

# Signals
signal enemy_detected(player, enemy)
signal inventory_button_pressed(player)
signal open_chest_input_received(chest)
signal guild_hall_desk_input_received(desk)

# Public Variables
var coins: int
var inventory: Array
var equipped_weapon: String
var equipped_armor: String

# Onready Variables
onready var hud_margin = $HUD/MarginContainer
onready var hud_health_label = $HUD/MarginContainer/HealthMargin/MarginContainer/HBoxContainer/Health
onready var chest_detector = $ChestDetector
onready var desk_detector = $DeskDetector
onready var camera = $Camera2D
onready var collision_detector = $CollisionDetector


func _ready():
	coins = SaveData.coins
	inventory = SaveData.inventory
	equipped_weapon = SaveData.equipped_weapon
	equipped_armor = SaveData.equipped_armor 
	max_health = SaveData.max_health
	health = SaveData.health
	
	hud_health_label.text = "{health}/{max_health}".format(
			{"health": health, "max_health": max_health})
	
	var texture = load("res://Entities/Player/spritesheets/%s_Overworld.png"
			% SaveData.character_name)
	sprite.texture = texture


func _on_Player_tree_exiting():
	SaveData.coins = coins
	SaveData.inventory = inventory
	SaveData.equipped_weapon = equipped_weapon
	SaveData.equipped_armor = equipped_armor
	SaveData.max_health = max_health
	SaveData.health = health


func _physics_process(_delta):
	var input_vel := Vector2()
	input_vel.x = Input.get_action_strength("player_move_right") - Input.get_action_strength("player_move_left")
	input_vel.y = Input.get_action_strength("player_move_down") - Input.get_action_strength("player_move_up")
	velocity = input_vel.normalized() * move_speed


# It would be better if the health was updated based on a signal but this works fine for now
func _process(_delta):
	hud_health_label.text = "{health}/{max_health}".format(
			{"health": health, "max_health": max_health})


func _on_EntityDetector_body_entered(body):
	if body.is_in_group("enemies"):
		emit_signal("enemy_detected", self, body)


func _on_Inventory_pressed():
	emit_signal("inventory_button_pressed", self)


func _unhandled_input(_event):
	if Input.is_action_just_pressed("open_chest"):
		var chests = chest_detector.get_overlapping_bodies()
		if chests:
			var closest_chest
			var closest_dist = -1
			for chest in chests:
				var dist = position.distance_to(chest.position)
				if dist < closest_dist or closest_dist == -1:
					closest_dist = dist
					closest_chest = chest
			
			emit_signal("open_chest_input_received", closest_chest)
	
	if Input.is_action_just_pressed("guild_hall_desk_interact"):
		var desks = desk_detector.get_overlapping_bodies()
		if not desks:
			return
		
		var desk = desks[0]
		
		emit_signal("guild_hall_desk_input_received", desk)

# Animation : play footstep sound
func anim_play_footstep (sfx_offset: int):
	AudioSystem.play_sfx(AudioSystem.SFX.FOOTSTEP_1 + sfx_offset, null, -22)
