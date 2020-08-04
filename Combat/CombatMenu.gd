extends Control

# Constants
enum MENU_SELECTED {
	MAIN,
	ATTACK
}

# Public Variables
var current_menu = MENU_SELECTED.MAIN

# Onready Variables
onready var buttons = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons
onready var main_buttons_menu = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/MainButtonsMenu
onready var attack_buttons_menu = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/AttackButtonsMenu
onready var combat_label = $VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel

func _ready():
	reset_ui();

func UpdateEnemyHp(newHp):
	pass

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

func ShowCombatLabel():
	buttons.visible = false
	combat_label.visible = true

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
