extends CanvasLayer

# Constants
const MAX_COIN_DROP = 1.2
const MIN_COIN_DROP = 0.8

# Private Variables
var _player_instance

# Onready Variables
onready var margin = $MarginContainer
onready var coin_amount_label = $MarginContainer/MarginContainer/VBoxContainer/CoinsDropped/CoinAmount
onready var items_dropped_grid = $MarginContainer/MarginContainer/VBoxContainer/ItemsDropped
onready var auto_close_timer = $AutoClose


func drop_items(enemy_name, player_instance):
	get_tree().paused = true
	
	_player_instance = player_instance
	
	var enemy_data = Data.enemy_data[enemy_name]
	
	# Coins Dropped
	var coins_dropped = Utility.randomRange(
			round(enemy_data.coin_drop_amount * MIN_COIN_DROP),
			round(enemy_data.coin_drop_amount * MAX_COIN_DROP) + 1)
	
	# Guarantee at least 1 coin to drop if the enemy drops coins
	if coins_dropped <= 0 and enemy_data.coin_drop_amount > 0:
		coins_dropped = 1
	
	coin_amount_label.text = str(coins_dropped)
	_player_instance.coins += coins_dropped
	
	# Items Drops
	Utility.queue_free_all_children(items_dropped_grid)
	
	var items = {}
	
	if enemy_data.has("item_drop_1"):
		items[enemy_data.item_drop_1] = enemy_data.item_drop_1_chance
	if enemy_data.has("item_drop_2"):
		items[enemy_data.item_drop_2] = enemy_data.item_drop_2_chance
	if enemy_data.has("item_drop_3"):
		items[enemy_data.item_drop_3] = enemy_data.item_drop_3_chance
	
	for i in enemy_data.max_items_dropped:
		var chance = randf()
		for item in items:
			if chance < items[item]:
				# Item Dropped
				var item_texture_rect = TextureRect.new()
				item_texture_rect.texture = Utility.get_inventory_item_resource(
					item)
				items_dropped_grid.add_child(item_texture_rect)
				
				player_instance.inventory.append(item)
				continue
	
	_player_instance.hud_margin.visible = false
	margin.visible = true
	auto_close_timer.start()


func close():
	margin.visible = false
	_player_instance.hud_margin.visible = true
	get_tree().paused = false


func _on_AutoClose_timeout():
	close()
