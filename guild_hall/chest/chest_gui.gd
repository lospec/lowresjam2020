extends CanvasLayer

# Private Variables
var _chest_item_resource := preload("res://guild_hall/chest/chest_item.tscn")

var _player_instance
var _chest_instance
var _opened_chest_id := -1

var _held_chest_item_instance
var _held_inventory_item_instance

# Onready Variables
onready var margin = $MarginContainer
onready var inventory_contents = $MarginContainer/MarginContainer/HBoxContainer/InventoryScroll/InventoryContents
onready var chest_contents = $MarginContainer/MarginContainer/HBoxContainer/ChestContents
onready var draggable_item = $DraggableItem


func _ready():
	margin.visible = false
	
	for chest_item in chest_contents.get_children():
		chest_item.connect("left_clicked", self, "_on_chest_item_left_clicked")


func update_chest_ui():
	if not _chest_instance:
		push_error("Chest instance not found.")
		return
	
	for i in _chest_instance.contents:
		var item_name = _chest_instance.contents[i]
		chest_contents.get_child(i).item_name = item_name


func update_inventory_ui():
	if not _chest_instance:
		push_error("Chest instance not found.")
		return
	
	var movable_inv = _player_instance.inventory.duplicate()
	movable_inv.erase(_player_instance.equipped_weapon)
	movable_inv.erase(_player_instance.equipped_armor)
	
	Utility.queue_free_all_children(inventory_contents)
	if inventory_contents.get_child_count():
		# Wait for last child to be freed
		yield(inventory_contents.get_child(inventory_contents.get_child_count() - 1),
				"tree_exited")
	
	for item_name in movable_inv:
		var inventory_item = _chest_item_resource.instance()
		inventory_item.item_name = item_name
		inventory_contents.add_child(inventory_item)
	
	# Fill in inventory boxes so there's always 8 empty boxes
	for i in 8:
		var inventory_item = _chest_item_resource.instance()
		inventory_contents.add_child(inventory_item)
	
	for inventory_item in inventory_contents.get_children():
		inventory_item.connect("left_clicked", self, "_on_inventory_item_left_clicked")


func open(player_instance, chest_instance):
	_player_instance = player_instance
	_chest_instance = chest_instance
	
	_opened_chest_id = _chest_instance.chest_id
	_chest_instance.contents = SaveData.chest_contents[_opened_chest_id]
	
	get_tree().paused = true
	
	update_chest_ui()
	update_inventory_ui()
	
	margin.visible = true


func close():
	margin.visible = false
	
	SaveData.chest_contents[_opened_chest_id] = _chest_instance.contents
	
	_opened_chest_id = -1
	_chest_instance = null
	
	get_tree().paused = false


func _on_inventory_item_left_clicked(inventory_item_instance):
	_held_inventory_item_instance = inventory_item_instance
	
	_held_inventory_item_instance.item_center.visible = false
	
	draggable_item.texture = Utility.get_inventory_item_resource(
			_held_inventory_item_instance.item_name)


func _on_chest_item_left_clicked(chest_item_instance):
	_held_chest_item_instance = chest_item_instance
	
	_held_chest_item_instance.item_center.visible = false
	
	draggable_item.texture = Utility.get_inventory_item_resource(
			_held_chest_item_instance.item_name)


func _process(_delta):
	if _held_chest_item_instance:
		draggable_item.rect_global_position = draggable_item.get_global_mouse_position() \
				- _held_chest_item_instance.item_center.rect_size / 2
	
	if _held_inventory_item_instance:
		draggable_item.rect_global_position = draggable_item.get_global_mouse_position() \
				- _held_inventory_item_instance.item_center.rect_size / 2


func _unhandled_input(event):
	if event is InputEventMouseButton and event.button_index == BUTTON_LEFT \
			and not event.is_pressed():
		var mouse_pos = draggable_item.get_global_mouse_position()
		if _held_chest_item_instance and _held_inventory_item_instance:
			push_error("Found both held chest item instance and held inventory item instance")
		elif _held_chest_item_instance:
			stop_dragging_from_chest(mouse_pos)
		elif _held_inventory_item_instance:
			stop_dragging_from_inv(mouse_pos)


# There's a bit of overlap between the two stop_dragging methods
func stop_dragging_from_chest(stop_position: Vector2):
	draggable_item.texture = null
	
	# Find out which chest slot it was placed in
	var chest_slot = get_empty_slot_at_pos(stop_position, chest_contents)
	
	if chest_slot:
		# If it was placed in a chest slot
		
		# Add new item
		var new_slot_index = chest_slot.get_index()
		_chest_instance.contents[new_slot_index] = _held_chest_item_instance.item_name
		
		# Remove old item
		var slot_index = _held_chest_item_instance.get_index()
		_chest_instance.contents[slot_index] = ""
		
		update_chest_ui()
	else:
		var inventory_slot = get_empty_slot_at_pos(stop_position, inventory_contents)
		
		if inventory_slot:
			# If it was placed in an inventory slot
			
			# Add new item
			_player_instance.inventory.append(_held_chest_item_instance.item_name)
			update_inventory_ui()
			
			# Remove old item
			var slot_index = _held_chest_item_instance.get_index()
			_chest_instance.contents[slot_index] = ""
			update_chest_ui()
	
	_held_chest_item_instance.item_center.visible = true
	_held_chest_item_instance = null


func stop_dragging_from_inv(stop_position: Vector2):
	draggable_item.texture = null
	
	# Find out which chest slot it was placed in
	var chest_slot = get_empty_slot_at_pos(stop_position, chest_contents)
	if chest_slot:
		# If it was placed in a chest slot
		
		# Add new item
		var new_slot_index = chest_slot.get_index()
		_chest_instance.contents[new_slot_index] = _held_inventory_item_instance.item_name
		update_chest_ui()
		
		# Remove old item
		_player_instance.inventory.erase(_held_inventory_item_instance.item_name)
		update_inventory_ui()
	
	_held_inventory_item_instance.item_center.visible = true
	_held_inventory_item_instance = null


func get_empty_slot_at_pos(position: Vector2, grid):
	for slot in grid.get_children():
		if slot.get_global_rect().has_point(position) and \
				slot.item_name == "":
			return slot
