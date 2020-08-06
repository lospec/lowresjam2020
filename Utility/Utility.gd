extends Node


func _ready():
	randomize()


static func randomRange(minim, maxim):
	return randi() % (int(maxim - minim)) + int(minim)


static func rand_element(arr):
	return arr[randi() % arr.size()]


static func rand_bool() -> bool:
	return bool(randi() % 2)


static func merge_dictionaries(dictionaries: Array) -> Dictionary:
	var new_dict = dictionaries[0]
	for dict in dictionaries:
		if new_dict == dict:
			continue
		
		for key in dict:
			new_dict[key] = dict[key]
	return new_dict


static func free_all_children(node: Node) -> void:
	for child in node.get_children():
		child.queue_free()


static func get_inventory_item_resource(item_name: String) -> Resource:
	return load("res://items/inventory_sprites/%s.png" % item_name.to_lower().replace(" ", "_"))
