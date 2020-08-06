extends MarginContainer

# Public Variables
var item_name: String

# Onready Variables
onready var hover_signifier := $HoverSignifier


func _ready():
	hover_signifier.visible = false


func _on_InventoryItem_mouse_entered():
	hover_signifier.visible = true


func _on_InventoryItem_mouse_exited():
	hover_signifier.visible = false

