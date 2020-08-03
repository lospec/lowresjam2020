extends Node

# Constants
const BASIC_ITEM_DATA_PATH = "res://Data/ItemData.csv"
const USABLE_ITEM_DATA_PATH = "res://Data/UsableItemData.csv"
const WEAPON_DATA_PATH = "res://Data/WeaponData.csv"

# Public Variables
var all_items = {}
var basic_items = {}
var usable_items = {}
var weapons = {}

func _ready():
	parse_data(BASIC_ITEM_DATA_PATH, [basic_items, all_items])
	parse_data(USABLE_ITEM_DATA_PATH, [usable_items, all_items])
	parse_data(WEAPON_DATA_PATH, [weapons, all_items])


func parse_data(csv_path, dict_collections):
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
			var item = {}
			var item_name
			for i in csv.size():
				var value = csv[i]
				var property = property_names[i]
				
				if property == "item_name":
					item_name = value
					continue
				
				item[property] = value
			for dict in dict_collections:
				dict[item_name] = item
		line_num += 1
	file.close()

func get_item(item_name: String):
	return load("res://Items/item_scenes/%s.tscn" % item_name)
