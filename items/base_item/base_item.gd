extends Node

# Public Variables
var item_name: String
var buy_value: int
var sell_value: int


func _ready():
	if item_name == "":
		return
	
	var item_stats = Data.all_item_data[item_name]
	for property in item_stats:
		var value = item_stats[property]
		if get(property) == null:
			push_error("No corresponding variable found for %s" % property)
		set(property, value)
