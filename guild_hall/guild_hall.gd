extends Node

# Constants
const CHEST_RESOURCE = preload("res://guild_hall/chest/chest.tscn")
const CHEST_GAP_X = 10
const CHEST_GAP_Y = 0
const TOP_POS_Y = -8
const BOTTOM_POS_Y = 6
const BLACK_TILES_POS_Y = 7
const PLANK_START_POS_Y = -2
enum Tiles {
	EMPTY = -1,
	PLANK_ODD,
	PLANK_EVEN,
	BLACK,
	DOOR,
	WALL
}

# Public Variables
var currently_opened_chest = null
var guild_interface_open = false

# Onready Variables
onready var chest_gui = $ChestGUI
onready var objects_ysort = $Objects
onready var player = objects_ysort.get_node("Player")
onready var pause_menu = $PauseMenu
onready var guild_interface = $GuildInterface
onready var extended_tilemap = $Extended
onready var first_chest_position = $FirstChestPosition.position
onready var right_border = $Borders/Right


func _ready():
	player.birds_system.visible = false
	player.clouds_system.visible = false
	
	update_guild_from_level()


func _on_DoorDetection_body_entered(body):
	if not body.is_in_group("enemies"):
		# Transition to world scene & play sound
		AudioSystem.play_sfx(AudioSystem.SFX.DOOR_OPEN, null, -30)
		Transitions.change_scene_double_transition("res://World/World.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.3)


func _on_Player_open_chest_input_received(chest):
	if currently_opened_chest != null or chest.animated_sprite.playing:
		return
	
	# Play chest open animation and sound then wait until the animation is finished
	AudioSystem.play_sfx(AudioSystem.SFX.CHEST_OPEN, null, -25)
	chest.animated_sprite.play("open")
	yield(chest.animated_sprite, "animation_finished")
	chest.animated_sprite.stop()
	
	# Open chest GUI
	currently_opened_chest = chest
	chest_gui.open(player, chest)


func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		if guild_interface_open:
			guild_interface_open = false
			guild_interface.toggle(player)
		else:
			pause_menu.toggle_pause(player)
	
	if Input.is_action_just_pressed("close_chest") and currently_opened_chest != null \
			and not currently_opened_chest.animated_sprite.playing:
		chest_gui.close()
		currently_opened_chest.animated_sprite.play("close")
		yield(currently_opened_chest.animated_sprite, "animation_finished")
		currently_opened_chest.animated_sprite.stop()
		currently_opened_chest = null


func _on_Player_guild_hall_desk_input_received(_desk):
	guild_interface_open = true
	guild_interface.toggle(player)


func _on_GuildInterface_guild_hall_level_up():
	update_guild_from_level()


func update_guild_from_level():
	# Clear extended tilemap
	for tile in extended_tilemap.get_used_cells():
		extended_tilemap.set_cellv(tile, Tiles.EMPTY)
	
	# Free all chests
	for chest in get_tree().get_nodes_in_group("Chest"):
		chest.queue_free()
	
	for i in SaveData.guild_level:
		# Append chest contents array
		if i >= SaveData.chest_contents.size():
			SaveData.chest_contents.append({
				0: "",
				1: "",
				2: "",
				3: "",
				4: "",
				5: "",
				6: "",
				7: "",
			})
		
		# Place chest
		var pos = first_chest_position + Vector2(CHEST_GAP_X * i, CHEST_GAP_Y * i)
		
		var chest_instance = CHEST_RESOURCE.instance()
		chest_instance.position = pos
		chest_instance.chest_id = i
		
		objects_ysort.add_child(chest_instance)
		
		# Place tiles
		var x = i
		
		extended_tilemap.set_cell(x, BLACK_TILES_POS_Y, Tiles.BLACK)
		
		for y in range(TOP_POS_Y, BOTTOM_POS_Y + 1):
			x = i
			
			if y % 2 == 0:
				x += 1
			
			var tile = Tiles.WALL
			
			if y >= PLANK_START_POS_Y:
				if y % 2 == 0:
					tile = Tiles.PLANK_EVEN
				else:
					tile = Tiles.PLANK_ODD
			
			extended_tilemap.set_cell(x, y, tile)
	
	# Set right border position
	right_border.position.x = 38 + CHEST_GAP_X * (SaveData.guild_level - 1)
	
	# Set camera right limit
	player.camera.limit_right = 32 + CHEST_GAP_X * (SaveData.guild_level - 1)
