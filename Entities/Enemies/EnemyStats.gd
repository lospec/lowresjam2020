extends Node

# Public Variables
var property_names = []
var enemies_stats = {}

func _init():
	parse_enemy_data()

func parse_enemy_data():
	var file = File.new()
	file.open("res://Data/EnemyData.csv", file.READ)
	
	var line_num = 0
	while !file.eof_reached():
		var csv = file.get_csv_line()
		if line_num == 0:
			for value in csv:
				property_names.append(value)
		else:
			var enemy_stats = {}
			var enemy_name
			for i in csv.size():
				var value = csv[i]
				var property = property_names[i]
				
				var separated_value = value.split(",")
				if property in ["drop_1", "drop_1_chance"]:
					value = separated_value
				
				if property == "name":
					enemy_name = value
				
				enemy_stats[property] = value
			
			enemies_stats[enemy_name] = enemy_stats
			
			if enemy_stats["drop_1"].size() != enemy_stats["drop_1_chance"].size():
				push_error("The length of the item drop list for the \"%s\" enemy is not equal to the length of the item drop chance list." % enemy_name)
		
		line_num += 1
	
	file.close()
