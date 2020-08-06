extends MarginContainer

# Signals
signal detailed_item_info_menu_appeared
signal detailed_item_info_menu_disappeared
signal equipped_weapon_changed
signal equipped_armor_changed
signal yes_or_no_pressed(yes_pressed)

# Constants
const DAMAGE_TYPE_ICON = {
	"none": preload("res://UI/inventory/item_info/type_icons/none_type_icon.png"),
	"Pierce": preload("res://UI/inventory/item_info/type_icons/pierce_type_icon.png"),
	"Blunt": preload("res://UI/inventory/item_info/type_icons/blunt_type_icon.png"),
	"Fire": preload("res://UI/inventory/item_info/type_icons/fire_type_icon.png"),
	"Water": preload("res://UI/inventory/item_info/type_icons/water_type_icon.png"),
	"Electricity": preload("res://UI/inventory/item_info/type_icons/electricity_type_icon.png"),
}
const STATUS_ICON = {
	"none": preload("res://UI/inventory/item_info/status_icons/none_status_icon.png"),
	"Asleep": preload("res://UI/inventory/item_info/status_icons/asleep_status_icon.png"),
	"Burning": preload("res://UI/inventory/item_info/type_icons/fire_type_icon.png"),
	"Frozen": preload("res://UI/inventory/item_info/status_icons/frozen_status_icon.png"),
	"Weak": preload("res://UI/inventory/item_info/status_icons/weak_status_icon.png"),
	"Confused": preload("res://UI/inventory/item_info/status_icons/confused_status_icon.png"),
	"Poison": preload("res://UI/inventory/item_info/status_icons/poison_status_icon.png")
}

# Public Variables
var selected_item
var player_instance

# Onready Variables
onready var margin = $MarginContainer
onready var vbox = margin.get_node("VBoxContainer")

onready var top_hbox = vbox.get_node("Top")
onready var name_margin = top_hbox.get_node("NameMargin")
onready var name_label_margin = name_margin.get_node("NameLabelMargin")
onready var name_label = name_label_margin.get_node("Name")

onready var second_row_hbox = vbox.get_node("SecondRow")
onready var item_margin = second_row_hbox.get_node("ItemMargin")
onready var icon_margin = item_margin.get_node("IconMargin")
onready var item_icon = icon_margin.get_node("ItemIcon")

onready var button_margin = second_row_hbox.get_node("ButtonMargin")
onready var equip_button = button_margin.get_node("Equip")
onready var use_button = button_margin.get_node("Use")

onready var damage_values = vbox.get_node("DamageValues")

onready var quick_vbox = damage_values.get_node("Quick")
onready var quick_damage_label = quick_vbox.get_node("DamageValue")
onready var quick_icon_vbox = quick_vbox.get_node("IconVBoxContainer")
onready var quick_type_icon = quick_icon_vbox.get_node("TypeIcon")
onready var quick_status_icon = quick_icon_vbox.get_node("StatusIcon")
onready var quick_status_effect_chance_label = quick_icon_vbox.get_node("StatusEffectChance")

onready var heavy_vbox = damage_values.get_node("Heavy")
onready var heavy_damage_label = heavy_vbox.get_node("DamageValue")
onready var heavy_icon_vbox = heavy_vbox.get_node("IconVBoxContainer")
onready var heavy_type_icon = heavy_icon_vbox.get_node("TypeIcon")
onready var heavy_status_icon = heavy_icon_vbox.get_node("StatusIcon")
onready var heavy_status_effect_chance_label = heavy_icon_vbox.get_node("StatusEffectChance")

onready var counter_vbox = damage_values.get_node("Counter")
onready var counter_damage_label = counter_vbox.get_node("DamageValue")
onready var counter_icon_vbox = counter_vbox.get_node("IconVBoxContainer")
onready var counter_type_icon = counter_icon_vbox.get_node("TypeIcon")
onready var counter_status_icon = counter_icon_vbox.get_node("StatusIcon")
onready var counter_status_effect_chance_label = counter_icon_vbox.get_node("StatusEffectChance")

onready var health_gained_hbox = vbox.get_node("HealthGainedHBox")
onready var health_gained_value_label = health_gained_hbox.get_node("HealthGainedValueLabel")

onready var confirm_use_popup = margin.get_node("ConfirmUsePopup")


func _ready():
	confirm_use_popup.visible = false


func _on_Item_pressed(item_name, player):
	selected_item = item_name
	player_instance = player
	
	visible = true
	emit_signal("detailed_item_info_menu_appeared")
	
	name_label.text = item_name
	item_icon.texture = load("res://items/inventory_sprites/%s.png" % item_name.to_lower().replace(" ", "_"))
	
	var item_data = Data.item_data[item_name]
	var item_type = item_data.type
	match item_type:
		"item":
			damage_values.visible = false
			equip_button.visible = false
			use_button.visible = false
			health_gained_hbox.visible = false
		"weapon":
			damage_values.visible = true
			equip_button.visible = player_instance.equipped_weapon != item_name
			use_button.visible = false
			health_gained_hbox.visible = false
			
			quick_damage_label.text = str(item_data.quick_damage)
			heavy_damage_label.text = str(item_data.heavy_damage)
			counter_damage_label.text = str(item_data.counter_damage)
			
			quick_type_icon.texture = DAMAGE_TYPE_ICON[item_data.quick_damage_type]
			heavy_type_icon.texture = DAMAGE_TYPE_ICON[item_data.heavy_damage_type]
			counter_type_icon.texture = DAMAGE_TYPE_ICON[item_data.counter_damage_type]
			
			quick_status_icon.texture = STATUS_ICON[item_data.quick_status_effect]
			if item_data.quick_status_effect == "none":
				quick_status_effect_chance_label.text = ""
			else:
				quick_status_effect_chance_label.text = str(item_data.quick_effect_chance * 100)
			
			heavy_status_icon.texture = STATUS_ICON[item_data.heavy_status_effect]
			if item_data.heavy_status_effect == "none":
				heavy_status_effect_chance_label.text = ""
			else:
				heavy_status_effect_chance_label.text = str(item_data.heavy_effect_chance * 100)
			
			counter_status_icon.texture = STATUS_ICON[item_data.counter_status_effect]
			if item_data.counter_status_effect == "none":
				counter_status_effect_chance_label.text = ""
			else:
				counter_status_effect_chance_label.text = str(item_data.counter_effect_chance * 100)
		"usable":
			damage_values.visible = false
			equip_button.visible = false
			use_button.visible = true
			health_gained_hbox.visible = true
			
			health_gained_value_label.text = str(item_data.health_gained)
		"armor":
			damage_values.visible = false
			equip_button.visible = true
			use_button.visible = false
			health_gained_hbox.visible = true
			equip_button.visible = player_instance.equipped_armor != item_name
			
			health_gained_value_label.text = str(item_data.health_added)


func _on_Back_pressed():
	visible = false
	emit_signal("detailed_item_info_menu_disappeared")


func _on_Equip_pressed():
	var selected_item_data = Data.item_data[selected_item]
	var selected_item_type = selected_item_data.type
	match selected_item_type:
		"weapon":
			player_instance.equipped_weapon = selected_item
			emit_signal("equipped_weapon_changed")
			equip_button.visible = false
		"armor":
			player_instance.equipped_armor = selected_item
			player_instance.max_health = 10 + selected_item_data.health_added
			player_instance.health += selected_item_data.health_added
			emit_signal("equipped_armor_changed")
			equip_button.visible = false
		_:
			push_error("The equip button was pressed for a non-equippable item.")


func _on_Use_pressed():
	var selected_item_data = Data.item_data[selected_item]
	var selected_item_type = selected_item_data.type
	if selected_item_type == "usable":
		if player_instance.health >= player_instance.max_health:
			confirm_use_popup.visible = true
			var yes_pressed = yield(self, "yes_or_no_pressed")
			confirm_use_popup.visible = false
			if not yes_pressed:
				return
		else:
			player_instance.health = min(player_instance.health + selected_item_data.health_gained,
					player_instance.max_health)
		player_instance.inventory.erase(selected_item)
		get_parent().get_parent().get_parent().update_inventory() # Should use a signal but this is fine for now
		visible = false
	else:
		push_error("The use button was pressed for a non-usable item.")


func _on_Yes_pressed():
	emit_signal("yes_or_no_pressed", true)


func _on_No_pressed():
	emit_signal("yes_or_no_pressed", false)
