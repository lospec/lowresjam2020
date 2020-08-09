extends Node

# Public Variables
var currently_opened_chest = null
var guild_interface_open = false

# Onready Variables
onready var chest_gui = $ChestGUI
onready var player = $YSort/Player
onready var pause_menu = $PauseMenu
onready var guild_interface = $GuildInterface


func _on_DoorDetection_body_entered(body):
	if not body.is_in_group("enemies"):
		Transitions.change_scene_double_transition("res://World/World.tscn",
			Transitions.Transition_Type.SHRINKING_CIRCLE,
			0.3)


func _on_Player_open_chest_input_received(chest):
	if currently_opened_chest != null or chest.animated_sprite.playing:
		return
	
	chest.animated_sprite.play("open")
	yield(chest.animated_sprite, "animation_finished")
	chest.animated_sprite.stop()
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
