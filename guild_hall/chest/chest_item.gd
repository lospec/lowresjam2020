extends MarginContainer

# Signals
signal left_clicked(chest_item_instance)

# Public Variables
var item_name: String setget set_item

# Onready Variables
onready var item_center = $ItemCenter


func set_item(value):
	item_name = value
	if item_name == "":
		$ItemCenter/Item.texture = null
	else:
		$ItemCenter/Item.texture = Utility.get_inventory_item_resource(item_name)


func _on_Item_gui_input(event):
	if event is InputEventMouseButton and event.button_index == BUTTON_LEFT and \
			event.is_pressed():
		emit_signal("left_clicked", self)
