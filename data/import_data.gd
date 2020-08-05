extends Node

# Constants
const BASIC_ITEM_DATA_PATH := "res://data/files/basic_item_data.json"
const WEAPON_DATA_PATH := "res://data/files/weapon_data.json"

const ENEMY_DATA_PATH := "res://data/files/enemy_data.json"

# Public Variables
var all_item_data: Dictionary
var basic_item_data: Dictionary
var weapon_data: Dictionary

var enemy_data: Dictionary


func _ready():
	# Parse item data
	basic_item_data = parse_data(BASIC_ITEM_DATA_PATH)
	weapon_data = parse_data(WEAPON_DATA_PATH)
	all_item_data = Utility.merge_dictionaries([basic_item_data, weapon_data])
	
	# Parse enemy data
	enemy_data = parse_data(ENEMY_DATA_PATH)


func parse_data(path):
	var file = File.new()
	file.open(path, File.READ)
	
	var data = JSON.parse(file.get_as_text())
	file.close()
	
	return data.result
