extends Object

# Public Variables
var item_name: String
var buy_value: int
var sell_value: int

func _ready():
	if item_name == "":
		return
	
	var item_stats = Items.all_items[item_name]
	for property in item_stats:
		var value = item_stats[property]
		if get(property) == null:
			push_error("No corresponding variable found for %s" % property)
		set(property, value)
