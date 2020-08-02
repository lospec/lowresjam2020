extends CanvasLayer

# Onready Variables
onready var margin_container = $MarginContainer

func _unhandled_input(_event):
	if Input.is_action_just_pressed("ui_cancel"):
		pause_game()

func pause_game():
	var tree = get_tree()
	tree.paused = not tree.paused
	margin_container.visible = not margin_container.visible
