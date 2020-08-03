extends Control

onready var attackButtons = $AttackButtons
onready var mainButtons = $MainButtons
onready var combatLabel = $Label

func _ready():
	ResetUI();

func UpdateEnemyHp(newHp):
	pass

func ResetUI():
	mainButtons.visible = true
	attackButtons.visible = false
	combatLabel.visible = false

func ShowCombatLabel():
	mainButtons.visible = false
	attackButtons.visible = false
	combatLabel.visible = true

func OpenAttackMenu():
	mainButtons.visible = false
	attackButtons.visible = true

func _on_AttackButton_pressed():
	OpenAttackMenu()

func _on_Combat_EnemyHpChanged():
	pass
