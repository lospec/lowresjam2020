extends Control

# Constants
enum MENU_SELECTED {
	MAIN,
	ATTACK,
}

# Public Variables
var current_menu = MENU_SELECTED.MAIN

# Onready Variables
onready var buttons = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons
onready var main_buttons_menu = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/MainButtonsMenu
onready var attack_buttons_menu = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/AttackButtonsMenu
onready var combat_label = $VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel
onready var player_health_label = $VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/Health
onready var enemy_health_bar = $VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/EnemyHealthBar
onready var enemy_health_bar_tween = $VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/Tween


func _ready():
	reset_ui();


func set_player_health_value(_max_health, current_health):
	player_health_label.text = str(current_health)


func set_enemy_health_value(max_health, current_health):
	enemy_health_bar.max_value = max_health
	enemy_health_bar.value = current_health


func update_player_health_value(new_health):
	player_health_label.text = str(new_health)


func update_enemy_health_value(new_health):
	enemy_health_bar_tween.interpolate_property(enemy_health_bar, "value",
			enemy_health_bar.value, new_health, 1.0,
			Tween.TRANS_CUBIC, Tween.EASE_OUT)
	enemy_health_bar_tween.start()


func reset_ui():
	buttons.visible = true
	match current_menu:
		MENU_SELECTED.MAIN:
			main_buttons_menu.visible = true
			attack_buttons_menu.visible = false
		MENU_SELECTED.ATTACK:
			main_buttons_menu.visible = false
			attack_buttons_menu.visible = true
	combat_label.visible = false

func set_buttons_visible(visible = true):
	buttons.visible = visible

func show_combat_label(text, time):
	combat_label.text = text
	combat_label.visible = true
	yield(get_tree().create_timer(time), "timeout")
	combat_label.visible = false

func open_main_menu():
	current_menu = MENU_SELECTED.MAIN
	reset_ui()

func open_attack_menu():
	current_menu = MENU_SELECTED.ATTACK
	reset_ui()

func _on_Attack_pressed():
	open_attack_menu()

func _on_CombatMenu_gui_input(_event):
	if Input.is_action_just_pressed("ui_cancel") and current_menu == MENU_SELECTED.ATTACK:
		open_main_menu()

func _on_Back_pressed():
	open_main_menu()

func _on_Combat_EnemyHpChanged():
	pass
