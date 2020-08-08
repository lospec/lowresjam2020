extends ShakableControl

class_name CombatMenu

# Signals
signal action_selected

# Constants
enum MENU_SELECTED {
	MAIN,
	ATTACK,
}

# Exported Variables
# should probably put this in somewhere else
# maybe the weapon/weaponUtil script or something
export(Texture) var blunt_attack_anim
export(Texture) var counter_anim

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var utility = preload("res://Utility/Utility.gd")
var current_menu = MENU_SELECTED.MAIN
var DamageLabel = preload("res://Combat/Effects/DamageLabel.tscn")

# Onready Variables
onready var buttons = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons
onready var main_buttons_menu = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/MainButtonsMenu
onready var attack_buttons_menu = $VBoxContainer/PlayerHUD/ChoiceHUD/Buttons/AttackButtonsMenu
onready var combat_label = $VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel
onready var combat_turn_result = $VBoxContainer/PlayerHUD/ChoiceHUD/CombatTurnResult
onready var player_health_label = $VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/Health
onready var player_health_icon = $VBoxContainer/PlayerHUD/HealthHUD/MarginContainer/HBoxContainer/MarginContainer/HealthIcon
onready var enemy_health_bar = $VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/EnemyHealthBar
onready var enemy_health_bar_tween = $VBoxContainer/EnemyHUD/VBoxContainer/MarginContainer/MarginContainer/Tween
onready var enemy_image = $VBoxContainer/EnemyHUD/VBoxContainer/Enemy
onready var attack_effect = $EffectsContainer/EffectTexture
onready var damage_spawn_area = $EffectsContainer/DamageSpawnArea

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
	hide_turn_result()
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

func hide_turn_result():
	combat_turn_result.win_container.visible = false
	combat_turn_result.compare_container.visible = false

func show_turn_result(playerAction, enemyAction):
	combat_turn_result.visible = true
	yield(combat_turn_result.show_turn_compare(playerAction, enemyAction, 1.5), "completed")
	combat_turn_result.show_win_result(playerAction, enemyAction)

func show_combat_label(text, duration = 0):
	combat_label.text = text
	combat_label.visible = true
	
	if duration > 0:
		yield(get_tree().create_timer(duration), "timeout")
		combat_label.visible = false

# still needed to implement argument to get what animation to play
func animate_player_attack(action: int):
	if action == combat_util.Combat_Action.COUNTER:
		attack_effect.play(counter_anim, combat_util.GetActionColor(action))
		yield(attack_effect, "effect_done")
	
	attack_effect.play(blunt_attack_anim, combat_util.GetActionColor(action))
	yield(attack_effect, "effect_done")

func animate_player_hurt(damage, enemyCountered: bool = false):
	if enemyCountered:
		attack_effect.play(counter_anim, combat_util.GetActionColor(combat_util.Combat_Action.HEAVY))
		yield(attack_effect, "effect_done")
	
	shake(1, 20, 1)
	yield(get_tree().create_timer(1.5), "timeout")
	#yield(show_combat_label("You take %s dmg" % damage, 2), "completed")
	# how do i blink the player health icon?
	# i want to shake the player health icon but don't know how

func animate_enemy_hurt(enemy_instance, damage):
	spawn_enemy_damage_label(damage)
	enemy_image.shake(1, 15, 1)
	
	enemy_image.texture = enemy_instance.battle_texture_hurt
	yield(get_tree().create_timer(1), "timeout")
	enemy_image.texture = enemy_instance.battle_texture_normal

func spawn_enemy_damage_label(damage):
	var damage_label = DamageLabel.instance()
	damage_spawn_area.add_child(damage_label)
	
	var x = (utility.randomRange(0, damage_spawn_area.rect_size.x)
		- damage_label.rect_size.x / 2)
	var y = (utility.randomRange(0, damage_spawn_area.rect_size.y)
		- damage_label.rect_size.y / 2)
	
	damage_label.rect_position = Vector2(x, y)
	damage_label.text = str(damage)


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

# These 3 signal emmiters can be used to make the player even more modular
# the combat system can be made to not wait for this input
# but they will wait for PlayerCombat to choose an action
# and the PlayerCombat is set to listen to these signals
# but i'm(A&) not confident enough to pull this off on time
# if there's enough time maybe this could even be made into multiplayer or ai driven strategy or something
func _on_Counter_pressed():
	emit_signal("action_selected", combat_util.Combat_Action.COUNTER)

func _on_Quick_pressed():
	emit_signal("action_selected", combat_util.Combat_Action.QUICK)

func _on_Heavy_pressed():
	emit_signal("action_selected", combat_util.Combat_Action.HEAVY)
