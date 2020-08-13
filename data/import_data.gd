extends Node

# Constants
const ITEM_DATA_PATH := "res://data/item_data.json"
const ENEMY_DATA_PATH := "res://data/enemy_data.json"
const CHARACTER_DATA_PATH := "res://data/character_data.json"

# Public Variables
var item_data: Dictionary
var enemy_data: Dictionary
var character_data: Dictionary

var _min_speed_stat = 9223372036854775807
var _max_speed_stat = -9223372036854775808
var _speed_stat_read = false


func _ready():
	item_data = parse_data(ITEM_DATA_PATH)
	enemy_data = parse_data(ENEMY_DATA_PATH)
	character_data = parse_data(CHARACTER_DATA_PATH)
	_set_min_max_speed_stat()


func _set_min_max_speed_stat():
	for enemy_stat in enemy_data.values():
		if not enemy_stat.has("move_speed"):
			continue
		var speed = enemy_stat["move_speed"]
		if speed < _min_speed_stat:
			_min_speed_stat = speed
		if speed > _max_speed_stat:
			_max_speed_stat = speed
	_speed_stat_read = true


func get_lerped_speed_stat(speed_stat: int, min_speed: float, max_speed: float):
	if not _speed_stat_read:
		_set_min_max_speed_stat()
	return range_lerp(speed_stat, _min_speed_stat, _max_speed_stat, min_speed, max_speed)


func parse_data(path):
	var file = File.new()
	if not file.file_exists(path):
		push_error("%s file not found" % path)
		return

	file.open(path, File.READ)

	var data = JSON.parse(file.get_as_text())
	file.close()

	return data.result
