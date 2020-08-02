extends Node

# Constants
const BASIC_ITEM_DATA_PATH = "res://Data/ItemData.csv"
const WEAPON_DATA_PATH = "res://Data/WeaponData.csv"

# Public Variables
var BaseItemScript = preload("res://Items/base_item/BaseItem.gd")
var BaseWeaponScript = preload("res://Items/base_weapon/BaseWeapon.gd")
var all_items = {}
var basic_items = {}
var weapons = {}

func _ready():
	parse_data(BASIC_ITEM_DATA_PATH, BaseItemScript, [basic_items, all_items])
	parse_data(WEAPON_DATA_PATH, BaseWeaponScript, [weapons, all_items])


func parse_data(csv_path, class_script, dict_collections):
	var file = File.new()
	file.open(csv_path, file.READ)
	
	var property_names = []
	
	var line_num = 0
	while !file.eof_reached():
		var csv = file.get_csv_line()
		if line_num == 0:
			for value in csv:
				property_names.append(value)
		else:
			var item = class_script.new()
			for i in csv.size():
				var value = csv[i]
				var property = property_names[i]
				if item.get(property) == null:
					print_debug("Corresponding variable not found for the %s property" % property)
				item.set(property, value)
			for dict in dict_collections:
				dict[item.item_name] = item
		line_num += 1
	file.close()


func get_item(item_name):
	if all_items.has(item_name):
		return all_items[item_name]
	else:
		push_error("Item %s not found." % item_name)
