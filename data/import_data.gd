extends Node

# Constants
const ITEM_DATA_PATH := "res://data/files/item_data.json"
const ENEMY_DATA_PATH := "res://data/files/enemy_data.json"

# Public Variables
var item_data: Dictionary
var enemy_data: Dictionary


func _ready():
	# Parse item data
	item_data = parse_data(ITEM_DATA_PATH)
	
	# Parse enemy data
	enemy_data = parse_data(ENEMY_DATA_PATH)


func parse_data(path):
	var file = File.new()
	file.open(path, File.READ)
	
	var data = JSON.parse(file.get_as_text())
	file.close()
	
	return data.result