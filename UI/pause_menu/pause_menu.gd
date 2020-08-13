extends CanvasLayer

# Constants
enum Menu {
	SETTINGS,
	INFO,
	INVENTORY,
}
const LIGHT_GREEN = Color("35AF35")
const DARK_GREEN = Color("1B651B")
const LIGHT_RED = Color("BC3535")
const DARK_RED = Color("661E25")
const LIGHT_BLUE = Color("3535C9")
const DARK_BLUE = Color("1B1B65")
const MENU_HEADER_DECOR_COLOR = {
	Menu.SETTINGS: LIGHT_GREEN,
	Menu.INFO: LIGHT_RED,
	Menu.INVENTORY: LIGHT_BLUE,
}
const MENU_BUTTONS_NORMAL_COLOR = {
	Menu.SETTINGS: DARK_GREEN,
	Menu.INFO: DARK_RED,
	Menu.INVENTORY: DARK_BLUE,
}
const MENU_BUTTONS_PRESSED_COLOR = {
	Menu.SETTINGS: LIGHT_GREEN,
	Menu.INFO: LIGHT_RED,
	Menu.INVENTORY: LIGHT_BLUE,
}
const MENU_COIN_DECOR_COLOR = {
	Menu.INFO: DARK_RED,
	Menu.INVENTORY: DARK_BLUE,
}
const MENU_BACKGROUND = {
	Menu.SETTINGS: preload("res://UI/settings/background.png"),
	Menu.INFO: preload("res://UI/information/background.png"),
	Menu.INVENTORY: preload("res://UI/inventory/background.png"),
}

# Public Variables
var inventory_item_scene := preload("res://UI/inventory/InventoryItem.tscn")
var current_menu = Menu.INVENTORY
var player_instance

# Onready Variables
onready var tree = get_tree()

onready var pause_menu_control = $PauseMenuControl
onready var pause_menu_margin = pause_menu_control.get_node("PauseMenuMargin")
onready var pause_menu_vbox = pause_menu_margin.get_node("VBoxContainer")
onready var inventory_items_margin = pause_menu_vbox.get_node("InventoryItemsMargin")
onready var inventory_items_grid = inventory_items_margin.get_node("InventoryItemsScrollContainer/InventoryItems")
onready var top_hbox = pause_menu_vbox.get_node("Top")
onready var buttons = top_hbox.get_node("Buttons")
onready var settings_button = buttons.get_node("Settings")
onready var info_button = buttons.get_node("Info")
onready var inventory_button = buttons.get_node("Inventory")
onready var header_margin = top_hbox.get_node("MarginContainer")
onready var header_label = header_margin.get_node("Header")
onready var header_decor = header_margin.get_node("HeaderDecor")
onready var second_row_hbox = pause_menu_vbox.get_node("SecondRow")
onready var coins_margin = second_row_hbox.get_node("Coins")
onready var coins_label = coins_margin.get_node("CoinAmount")
onready var coins_decor = coins_margin.get_node("CoinsDecor")
onready var background = pause_menu_margin.get_node("Background")

onready var equipped_items_margin = second_row_hbox.get_node("EquippedMargin")
onready var equipped_items_hbox = equipped_items_margin.get_node("Equipped")
onready var equipped_weapon = equipped_items_hbox.get_node("EquippedWeapon")
onready var equipped_armor = equipped_items_hbox.get_node("EquippedArmor")

onready var item_stats_popup = pause_menu_control.get_node("ItemStatsPopup")

onready var item_info_menu = pause_menu_margin.get_node("ItemInfoMenu")


func _ready():
	pause_menu_control.visible = false
	
	item_stats_popup.rect_position.y = -19
	
	for button in buttons.get_children():
		button.connect("button_down", self, "_on_Button_button_down", [button])
		button.connect("button_up", self, "_on_Button_button_up", [button])


func _on_Player_inventory_button_pressed(player):
	toggle_pause(player)


func toggle_pause(player):
	player_instance = player
	tree.paused = not tree.paused
	pause_menu_control.visible = tree.paused
	player_instance.hud_margin.visible = not tree.paused
	if tree.paused:
		update_inventory()
		update_equipped_items()
		update_menu_state()
		coins_label.text = "%s:COIN" % player_instance.coins


func update_inventory():
	Utility.queue_free_all_children(inventory_items_grid)
	for item in player_instance.inventory:
		var inventory_item = inventory_item_scene.instance()
		inventory_item.item_name = item
		var item_texture_button = inventory_item.get_node("MarginContainer/Item")
		item_texture_button.texture_normal = Utility.get_inventory_item_resource(item)
		inventory_item.connect("mouse_entered", item_stats_popup, "_on_Item_mouse_entered", [item])
		inventory_item.connect("mouse_exited", item_stats_popup, "_on_Item_mouse_exited")
		item_texture_button.connect("pressed", item_info_menu, "_on_Item_pressed", [inventory_item, player_instance])
		inventory_items_grid.add_child(inventory_item)


func update_equipped_items():
	if player_instance.equipped_weapon == null:
		return
	
	equipped_weapon.item_name = player_instance.equipped_weapon
	if player_instance.equipped_weapon != "":
		var equipped_weapon_texture_button = equipped_weapon.get_node("MarginContainer/Item")
		equipped_weapon_texture_button.texture_normal = Utility.get_inventory_item_resource(player_instance.equipped_weapon)
	
	equipped_armor.item_name = player_instance.equipped_armor
	if player_instance.equipped_armor != "":
		var equipped_armor_texture_button = equipped_armor.get_node("MarginContainer/Item")
		equipped_armor_texture_button.texture_normal = Utility.get_inventory_item_resource(player_instance.equipped_armor)
	
	for equipped_item in equipped_items_hbox.get_children():
		var item_button = equipped_item.get_node("MarginContainer/Item")
		
		equipped_item.visible = equipped_item.item_name != ""
		
		if equipped_item.is_connected("mouse_entered", item_stats_popup, "_on_Item_mouse_entered"):
			equipped_item.disconnect("mouse_entered", item_stats_popup, "_on_Item_mouse_entered")
		equipped_item.connect("mouse_entered", item_stats_popup, "_on_Item_mouse_entered", [equipped_item.item_name])
		
		if equipped_item.is_connected("mouse_exited", item_stats_popup, "_on_Item_mouse_exited"):
			equipped_item.disconnect("mouse_exited", item_stats_popup, "_on_Item_mouse_exited")
		equipped_item.connect("mouse_exited", item_stats_popup, "_on_Item_mouse_exited")
		
		if item_button.is_connected("pressed", item_info_menu, "_on_Item_pressed"):
			 item_button.disconnect("pressed", item_info_menu, "_on_Item_pressed")
		item_button.connect("pressed", item_info_menu, "_on_Item_pressed", [equipped_item.item_name, player_instance])

func update_menu_state():
	match current_menu:
		Menu.SETTINGS:
			settings_button.pressed = true
			info_button.pressed = false
			inventory_button.pressed = false
			header_label.text = "Settings"
			coins_margin.visible = false
			inventory_items_margin.visible = false
			equipped_items_margin.visible = false
			item_stats_popup.visible = false
		Menu.INFO:
			settings_button.pressed = false
			info_button.pressed = true
			inventory_button.pressed = false
			header_label.text = "Info"
			coins_margin.visible = true
			inventory_items_margin.visible = false
			equipped_items_margin.visible = false
			item_stats_popup.visible = false
		Menu.INVENTORY:
			settings_button.pressed = false
			info_button.pressed = false
			inventory_button.pressed = true
			header_label.text = "Inventory"
			coins_margin.visible = false
			inventory_items_margin.visible = true
			equipped_items_margin.visible = true
			item_stats_popup.visible = true
	
	background.texture = MENU_BACKGROUND[current_menu]
	
	for button in buttons.get_children():
		if button.pressed:
			button.modulate = MENU_BUTTONS_PRESSED_COLOR[current_menu]
		else:
			button.modulate = MENU_BUTTONS_NORMAL_COLOR[current_menu]
	
	header_decor.modulate = MENU_HEADER_DECOR_COLOR[current_menu]
	if MENU_COIN_DECOR_COLOR.has(current_menu):
		coins_decor.modulate = MENU_COIN_DECOR_COLOR[current_menu]


func _on_Button_button_down(button):
	match button:
		settings_button:
			current_menu = Menu.SETTINGS
		info_button:
			current_menu = Menu.INFO
		inventory_button:
			current_menu = Menu.INVENTORY
	
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
			button.rect_global_position, -15)
	
	update_menu_state()


func _on_Button_button_up(button):
	button.pressed = true


func _on_ItemInfoMenu_equipped_armor_changed():
	update_equipped_items()


func _on_ItemInfoMenu_equipped_weapon_changed():
	update_equipped_items()
