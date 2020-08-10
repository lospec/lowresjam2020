extends CanvasLayer

# Signals
signal guild_hall_level_up

# Constants
const POPUP_ITEM_SCENE := preload("res://UI/inventory/InventoryItem.tscn")

enum Menu {
	DEPOSIT,
	SELL,
	MARKET,
}
const MENU_BACKGROUND = {
	Menu.DEPOSIT: preload("res://guild_hall/guild_interface/deposit/background.png"),
	Menu.SELL: preload("res://guild_hall/guild_interface/sell/background.png"),
	Menu.MARKET: preload("res://guild_hall/guild_interface/market/background.png"),
}
const LIGHT_ORANGE = Color("C97F35")
const DARK_ORANGE = Color("661E25")
const LIGHT_CYAN = Color("35B78F")
const DARK_CYAN = Color("1B6E83")
const LIGHT_PURPLE = Color("7F35C9")
const DARK_PURPLE = Color("553361")
const MENU_BUTTONS_NORMAL_COLOR = {
	Menu.DEPOSIT: DARK_ORANGE,
	Menu.SELL: DARK_CYAN,
	Menu.MARKET: DARK_PURPLE,
}
const MENU_BUTTONS_PRESSED_COLOR = {
	Menu.DEPOSIT: LIGHT_ORANGE,
	Menu.SELL: LIGHT_CYAN,
	Menu.MARKET: LIGHT_PURPLE,
}
const MENU_HEADER_DECOR_COLOR = {
	Menu.DEPOSIT: LIGHT_ORANGE,
	Menu.SELL: LIGHT_CYAN,
	Menu.MARKET: LIGHT_PURPLE,
}
const MENU_COIN_DECOR_COLOR = {
	Menu.DEPOSIT: LIGHT_ORANGE,
	Menu.SELL: LIGHT_CYAN,
	Menu.MARKET: LIGHT_PURPLE,
}
const DEPOSIT_AMOUNTS = [
	1, 2, 5,
	10, 20, 50,
	100, 200, 500,
	1000, 2000, 5000,
	10000, 20000, 50000
	]
const MAX_MARKET_ITEMS = 12
const MAX_MARKET_PRICE_LEVEL_MULTIPLIER = 25

# Public Variables
var current_menu = Menu.MARKET
var selected_item
var selected_market_item

# Private Variables
var _player_instance
var _deposit_amount_index := 0

# Onready Variables
onready var tree = get_tree()

onready var margin = $Margin
onready var background = margin.get_node("Background")
onready var vbox = margin.get_node("VBoxContainer")

onready var top = vbox.get_node("Top")
onready var buttons = top.get_node("Buttons")
onready var deposit_tab_button = buttons.get_node("Deposit")
onready var sell_tab_button = buttons.get_node("Sell")
onready var shop_tab_button = buttons.get_node("Market")

onready var header_margin = top.get_node("HeaderMargin")
onready var header_decor = header_margin.get_node("HeaderDecor")
onready var header_label = header_margin.get_node("Header")

onready var second_row = vbox.get_node("SecondRow")
onready var coins_margin = second_row.get_node("CoinsMargin")
onready var coins_decor = coins_margin.get_node("CoinsDecor")
onready var coins_label = coins_margin.get_node("CoinAmount")

onready var inventory_margin = vbox.get_node("InventoryMargin")
onready var inventory_scroll = inventory_margin.get_node("InventoryScrollContainer")
onready var inventory_grid = inventory_scroll.get_node("InventoryItems")

onready var item_price_margin = $ItemPriceMargin
onready var market_item_price_margin = $MarketItemPriceMargin

onready var deposit_margin = vbox.get_node("DepositMargin")
onready var deposit_vbox = deposit_margin.get_node("DepositVBox")
onready var deposit_hbox = deposit_vbox.get_node("DepositHBox")
onready var deposit_button_margin = deposit_hbox.get_node("DepositButtonMargin")
onready var deposit_button_vbox = deposit_button_margin.get_node("DepositButtonVBox")
onready var deposit_button = deposit_button_vbox.get_node("Deposit")

onready var deposit_progress_vbox = deposit_vbox.get_node("DepositProgressVBox")
onready var next_level_label = deposit_progress_vbox.get_node("NextLevel")
onready var next_level_progress_text = deposit_progress_vbox.get_node("ProgressText")
onready var whole_progress_bar_margin = deposit_progress_vbox.get_node("WholeProgressBarMargin")
onready var progress_bar_margin = whole_progress_bar_margin.get_node("ProgressBarMargin")
onready var deposit_progress_bar = progress_bar_margin.get_node("Progress")

onready var market_margin = vbox.get_node("MarketMargin")
onready var market_items_grid = market_margin.get_node("MarketItems")


func _ready():
	margin.visible = false
	item_price_margin.visible = false
	
	for button in buttons.get_children():
		button.connect("button_down", self, "_on_Button_button_down", [button])
		button.connect("button_up", self, "_on_Button_button_up", [button])


func _on_Button_button_down(button):
	match button:
		deposit_tab_button:
			current_menu = Menu.DEPOSIT
		sell_tab_button:
			current_menu = Menu.SELL
		shop_tab_button:
			current_menu = Menu.MARKET
	
	update_menu_state()


func _on_Button_button_up(button):
	button.pressed = true


func update_menu_state():
	match current_menu:
		Menu.DEPOSIT:
			shop_tab_button.pressed = false
			sell_tab_button.pressed = false
			deposit_tab_button.pressed = true
			header_label.text = "Deposit"
			
			inventory_margin.visible = false
			deposit_margin.visible = true
			market_margin.visible = false
		Menu.SELL:
			shop_tab_button.pressed = false
			sell_tab_button.pressed = true
			deposit_tab_button.pressed = false
			header_label.text = "Sell"
			
			inventory_margin.visible = true
			deposit_margin.visible = false
			market_margin.visible = false
		Menu.MARKET:
			shop_tab_button.pressed = true
			sell_tab_button.pressed = false
			deposit_tab_button.pressed = false
			header_label.text = "Market"
			
			inventory_margin.visible = false
			deposit_margin.visible = false
			market_margin.visible = true
	
	background.texture = MENU_BACKGROUND[current_menu]
	
	for button in buttons.get_children():
		if button.pressed:
			button.modulate = MENU_BUTTONS_PRESSED_COLOR[current_menu]
		else:
			button.modulate = MENU_BUTTONS_NORMAL_COLOR[current_menu]
	
	header_decor.modulate = MENU_HEADER_DECOR_COLOR[current_menu]
	coins_decor.modulate = MENU_COIN_DECOR_COLOR[current_menu]


func toggle(player):
	_player_instance = player
	tree.paused = not tree.paused
	margin.visible = tree.paused
	
	item_price_margin.visible = false
	market_item_price_margin.visible = false
	
	_player_instance.hud_margin.visible = not tree.paused
	if tree.paused:
		update_inventory()
		update_menu_state()
		update_coins()
		update_guild_level()
		update_progress_bar_instantly()
		update_market()


func update_inventory():
	Utility.queue_free_all_children(inventory_grid)
	
	var sellable_items = _player_instance.inventory.duplicate()
	sellable_items.erase(_player_instance.equipped_weapon)
	sellable_items.erase(_player_instance.equipped_armor)
	
	for item_name in sellable_items:
		var sellable_item = POPUP_ITEM_SCENE.instance()
		inventory_grid.add_child(sellable_item)
		sellable_item.item_name = item_name
		sellable_item.item_texture_button.texture_normal =\
				Utility.get_inventory_item_resource(item_name)
		
		sellable_item.connect("mouse_entered", self, "_on_InventoryItem_mouse_entered",
				[sellable_item])
		sellable_item.connect("mouse_exited", self, "_on_InventoryItem_mouse_exited")
		
		sellable_item.item_texture_button.connect("pressed", self,
				"_on_InventoryItem_pressed", [sellable_item])


func update_coins():
	coins_label.text = "%s:COIN" % _player_instance.coins


func _on_InventoryItem_mouse_entered(item):
	selected_item = item
	
	item_price_margin.rect_position = item.rect_global_position + \
			Vector2(item.rect_size.x, 0)
	if item_price_margin.rect_position.x >= 32:
		item_price_margin.rect_position.x -= item_price_margin.rect_size.x + \
				item.rect_size.x
	
	var item_name = item.item_name
	var item_data = Data.item_data[item_name]
	var item_sell_value = item_data.sell_value
	
	var item_price_label = item_price_margin.get_node("ItemPriceTextMargin/ItemPrice")
	item_price_label.text = "%s:c" % item_sell_value
	
	item_price_margin.visible = true


func _on_InventoryItem_mouse_exited():
	selected_item = null
	item_price_margin.visible = false


func _on_InventoryItem_pressed(item):
	if selected_item == item:
		selected_item = null
		item_price_margin.visible = false
	
	sell_item(item)


func sell_item(item):
	var item_name = item.item_name
	var item_data = Data.item_data[item_name]
	var item_sell_value = item_data.sell_value
	
	_player_instance.inventory.remove(item_name)
	
	_player_instance.coins += item_sell_value
	
	update_inventory()
	update_coins()


func _on_IncreaseAmount_pressed():
	_deposit_amount_index = (_deposit_amount_index + 1) % DEPOSIT_AMOUNTS.size()
	
	update_deposit_amount()


func _on_DecreaseAmount_pressed():
	_deposit_amount_index = (_deposit_amount_index - 1) % DEPOSIT_AMOUNTS.size()
	
	update_deposit_amount()


func update_deposit_amount():
	deposit_button.text = str(DEPOSIT_AMOUNTS[_deposit_amount_index])


func _on_Deposit_pressed():
	var deposit_amount = DEPOSIT_AMOUNTS[_deposit_amount_index]
	
	if _player_instance.coins == 0:
		return
	
	_player_instance.coins = max(_player_instance.coins - deposit_amount, 0)
	
	var total_coins_for_next_level = SaveData.guild_level * 250
	deposit_progress_bar.max_value = total_coins_for_next_level
	
	SaveData.coins_deposited += deposit_amount
	
	while SaveData.coins_deposited > total_coins_for_next_level:
		SaveData.coins_deposited -= total_coins_for_next_level
		SaveData.guild_level += 1
		
		emit_signal("guild_hall_level_up")
	
	update_coins()
	update_guild_level()
	update_progress_bar_instantly()


func update_guild_level():
	next_level_label.text = "To Level %s:" % (SaveData.guild_level + 1)
	
	var total_coins_for_next_level = SaveData.guild_level * 250
	next_level_progress_text.text = "{current_coins}/{total_coins}".format({
		"current_coins": SaveData.coins_deposited,
		"total_coins": total_coins_for_next_level,
	})


func update_progress_bar_instantly():
	var total_coins_for_next_level = SaveData.guild_level * 250
	deposit_progress_bar.max_value = total_coins_for_next_level
	
	deposit_progress_bar.value = SaveData.coins_deposited


func update_market():
	Utility.queue_free_all_children(market_items_grid)
	if market_items_grid.get_child_count():
		# Wait for last child to be freed
		yield(market_items_grid.get_child(market_items_grid.get_child_count() - 1),
				"tree_exited")
	
	var buyable_items = []
	for item_name in Data.item_data:
		var item_data = Data.item_data[item_name]
		if item_data.has("buy_value") and item_data.buy_value > 0 and \
				item_data.buy_value <= SaveData.guild_level * MAX_MARKET_PRICE_LEVEL_MULTIPLIER:
			buyable_items.append(item_name)
	
	while market_items_grid.get_child_count() < MAX_MARKET_ITEMS:
		var market_item = POPUP_ITEM_SCENE.instance()
		market_items_grid.add_child(market_item)
		
		var item_name = Utility.rand_element(buyable_items)
		market_item.item_name = item_name
		market_item.item_texture_button.texture_normal = \
				Utility.get_inventory_item_resource(item_name)
		
		market_item.connect("mouse_entered", self, "_on_MarketItem_mouse_entered",
				[market_item])
		market_item.connect("mouse_exited", self, "_on_MarketItem_mouse_exited")
		
		market_item.item_texture_button.connect("pressed", self,
				"_on_MarketItem_pressed", [market_item])


func _on_MarketItem_mouse_entered(item):
	selected_market_item = item
	
	market_item_price_margin.rect_position = item.rect_global_position + \
			Vector2(item.rect_size.x, 0)
	if market_item_price_margin.rect_position.x >= 32:
		market_item_price_margin.rect_position.x -= \
				market_item_price_margin.rect_size.x + item.rect_size.x
	
	var item_name = item.item_name
	var item_data = Data.item_data[item_name]
	
	var item_price_label = market_item_price_margin.get_node("ItemPriceTextMargin/ItemPrice")
	item_price_label.text = "%s:c" % item_data.buy_value
	
	market_item_price_margin.visible = true


func _on_MarketItem_mouse_exited():
	selected_market_item = null
	market_item_price_margin.visible = false


func _on_MarketItem_pressed(item):
	if buy_item(item) and selected_market_item == item:
		selected_market_item = null
		market_item_price_margin.visible = false


func buy_item(item) -> bool: # Returns true if successful
	var item_name = item.item_name
	var item_data = Data.item_data[item_name]
	
	if _player_instance.coins < item_data.sell_value:
		return false
	
	_player_instance.inventory.append(item_name)
	_player_instance.coins -= item_data.sell_value
	
	update_inventory()
	update_coins()
	
	return true
