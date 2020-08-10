extends MarginContainer

# This really should be refactored
# - Merged with inventory_item
# - Utilise setget to automatically set the item texture after modifying
# item name
# * But it works for now™

# Public Variables
var item_name: String

# Onready Variables
onready var hover_signifier := $HBoxContainer/ItemMargin/HoverSignifier
onready var item_texture_button := $HBoxContainer/ItemMargin/IconMargin/Item


func _ready():
	hover_signifier.visible = false


func _on_Item_mouse_entered():
	hover_signifier.visible = true


func _on_Item_mouse_exited():
	hover_signifier.visible = false
