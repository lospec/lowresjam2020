extends HBoxContainer

# Signals
signal volume_value_updated

# Constants
const INCREMENT_AMOUNT = 10
const DECREMENT_AMOUNT = 10
const STARTING_VOLUME = 100
const MAX_VOLUME = 100
const MIN_VOLUME = 0

# Public Variables
var volume_value = STARTING_VOLUME

# Onready Variables
onready var value_label = $Value

func _on_Increase_Value_pressed():
	if volume_value >= MAX_VOLUME:
		return
	volume_value += INCREMENT_AMOUNT
	update_volume_value()

func _on_Decrease_Value_pressed():
	if volume_value <= MIN_VOLUME:
		return
	volume_value -= DECREMENT_AMOUNT
	update_volume_value()

func update_volume_value():
	value_label.text = str(volume_value)
	emit_signal("volume_value_updated")
