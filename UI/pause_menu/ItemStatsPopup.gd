extends MarginContainer

# Constants
const ITEM_TYPE_TO_BACKGROUND := {
	"basic": preload("res://UI/inventory/item_stats_popup.png"),
	"weapon": preload("res://UI/inventory/weapon_stats_popup.png"),
	"usable": preload("res://UI/inventory/usable_armor_stats_popup.png"),
	"armor": preload("res://UI/inventory/usable_armor_stats_popup.png"),
}
const TOP_POS_Y = -19
const BOTTOM_POS_Y = 0
const ANIM_DURATION = 0.15

# Public Variables
var popup_to_be_visible = false

# Onready Variables
onready var tween = $Tween
onready var popup_disappear_delay_timer = $PopupDisappearDelay

onready var background := $Background
onready var text_margin := $TextMargin
onready var text_vbox := text_margin.get_node("VBoxContainer")
onready var item_name_label := text_vbox.get_node("Name")

onready var damage_margin := text_vbox.get_node("DamageMargin")
onready var damage_values_hbox := damage_margin.get_node("DamageValues")
onready var quick_damage_label := damage_values_hbox.get_node("QuickDamage")
onready var heavy_damage_label := damage_values_hbox.get_node("HeavyDamage")
onready var counter_damage_label := damage_values_hbox.get_node("CounterDamage")

onready var health_gained_margin := text_vbox.get_node("HealthGainedMargin")
onready var health_gained_label := health_gained_margin.get_node("HealthGained")


func _on_Item_mouse_entered(item_name):
	var item_data = Data.item_data[item_name]
	var item_type = item_data.type
	
	item_name_label.text = item_name
	
	background.texture = ITEM_TYPE_TO_BACKGROUND[item_type]
	match item_data.type:
		"basic":
			damage_margin.visible = false
			health_gained_margin.visible = false
		"weapon":
			damage_margin.visible = true
			health_gained_margin.visible = false
			
			quick_damage_label.text = str(item_data.quick_damage)
			heavy_damage_label.text = str(item_data.heavy_damage)
			counter_damage_label.text = str(item_data.counter_damage)
		"usable", "armor":
			damage_margin.visible = false
			health_gained_margin.visible = true
			
			health_gained_label.text = str(item_data.health_gained)
	
	tween.interpolate_property(self, "rect_position", rect_position,
			Vector2(rect_position.x, BOTTOM_POS_Y), ANIM_DURATION,
			Tween.TRANS_CUBIC, Tween.EASE_OUT)
	tween.start()
	popup_to_be_visible = true


func _on_Item_mouse_exited():
	popup_to_be_visible = false
	popup_disappear_delay_timer.start()


func _on_PopupDisappearDelay_timeout():
	if not popup_to_be_visible:
		tween.interpolate_property(self, "rect_position", rect_position,
			Vector2(rect_position.x, TOP_POS_Y), ANIM_DURATION,
			Tween.TRANS_CUBIC, Tween.EASE_OUT)
		tween.start()
