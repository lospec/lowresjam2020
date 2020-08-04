extends Control

# Onready Variables
onready var buttons = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons
onready var main_buttons = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/MainButtons
onready var attack_buttons = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/AttackButtons
onready var combat_label = $VBoxContainer/PlayerHUD/ChoiceHUD/CenterContainer/CombatLabel

func _ready():
	reset_ui();

func UpdateEnemyHp(newHp):
	pass

func reset_ui():
	buttons.visible = true
	main_buttons.visible = true
	attack_buttons.visible = false
	combat_label.visible = false

func ShowCombatLabel():
	buttons.visible = false
	combat_label.visible = true

func OpenAttackMenu():
	main_buttons.visible = false
	attack_buttons.visible = true

func _on_Attack_pressed():
	OpenAttackMenu()

func _on_Combat_EnemyHpChanged():
	pass
