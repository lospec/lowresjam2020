extends Node

# Constants
const ENEMY_DATA_PATH = "res://Data/EnemyData.csv"

# Public Variables
var BaseEnemyScript = preload("res://Entities/BaseEnemy/BaseEnemy.gd")
var property_names = []
var enemies = []

func _ready():
	parse_enemy_data()


func parse_enemy_data():
	var file = File.new()
	file.open(ENEMY_DATA_PATH, file.READ)
	
	var line_num = 0
	while !file.eof_reached():
		var csv = file.get_csv_line()
		if line_num == 0:
			for value in csv:
				property_names.append(value)
		else:
			var enemy = BaseEnemyScript.new()
			var drop_table = {}
			var drop_table_item = null
			var drop_table_chance = null
			for i in csv.size():
				var value = csv[i]
				var property = property_names[i]
				
				if "item_drop" in property:
					if "chance" in property:
						drop_table_chance = value
					elif value != "":
						drop_table_item = Items.get_item(value)
					
					if drop_table_item != null and drop_table_chance != null:
						drop_table[drop_table_item] = drop_table_chance
						drop_table_item = null
						drop_table_chance = null
					continue
				elif enemy.get(property) == null:
					print_debug("Corresponding variable not found for the %s property." % property)
				
				enemy.set(property, value)
			enemy.set("drop_table", drop_table)
			
			enemies.append(enemy)
		line_num += 1
	file.close()
