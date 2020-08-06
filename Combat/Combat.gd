extends CanvasLayer

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var player_instance
var enemy_instance

# Onready Variables
onready var combat_menu = $CombatMenu
onready var combat_label = $CombatMenu/VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel
onready var enemy_image = $CombatMenu/VBoxContainer/EnemyHUD/VBoxContainer/Enemy

func _on_Player_enemy_detected(player, enemy):
	var tree = get_tree()
	tree.paused = true
	
	player_instance = player
	enemy_instance = enemy
	
	player.hud_margin.visible = tree.paused
	
	combat_menu.set_player_health_value(player_instance.max_health,
			player_instance.health)
	combat_menu.set_enemy_health_value(enemy_instance.max_health,
			enemy_instance.health)
	
	enemy_image.texture = enemy_instance.battle_texture_normal
	
	combat_menu.visible = true


func _on_Player_health_changed(_old_health, new_health):
	combat_menu.update_player_health_value(new_health)


func _on_Enemy_health_changed(_old_health, new_health):
	combat_menu.update_enemy_health_value(new_health)


func GetActionWeakness(action):
	match (action):
		combat_util.Combat_Action.QUICK:
			return combat_util.COMBAT_ACTION.COUNTER
		combat_util.Combat_Action.COUNTER:
			return combat_util.COMBAT_ACTION.HEAVY
		combat_util.Combat_Action.HEAVY:
			return combat_util.COMBAT_ACTION.QUICK
	return combat_util.Combat_Action.INVALID


func ActionCompare(action1, action2):
	if action1 == action2:
		return 0
	elif GetActionWeakness(action2) == action1:
		return 1
	elif GetActionWeakness(action1) == action2:
		return 2
	return -1


func TakeTurn(playerAction):
	var enemyAction = playerAction#enemy_combat.GetAction()
	var win = ActionCompare(playerAction, enemyAction)
	
	combat_menu.ShowCombatLabel()
	
	match win:
		0:
			yield(Tie(playerAction), "completed")
		
		1:
			yield(PlayerWin(playerAction), "completed")
		
		2:
			yield(EnemyWin(playerAction), "completed")
	
	combat_menu.reset_ui()


func PlayerWin(playerAction):
	combat_label.text = "Player win case is still in progress"
	yield(get_tree().create_timer(1.5), "timeout")


func EnemyWin(enemyAction):
	combat_label.text = "Enemy win case is still in progress"
	yield(get_tree().create_timer(1.5), "timeout")


func Tie(action):
	var enemyDmg = enemy_instance.get_base_damage(action);
	var playerDmg = player_instance.get_base_damage(action);
	
	match (action):
		combat_util.Combat_Action.QUICK:
			combat_label.text = "Both of you attack"
			yield(get_tree().create_timer(1.5), "timeout")
			player_instance.health -= playerDmg
			combat_label.text = "The Enemy takes %s dmg" % playerDmg
			yield(get_tree().create_timer(1.5), "timeout")
			player_instance.health -= enemyDmg
			combat_label.text = "You take %s dmg" % enemyDmg
			yield(get_tree().create_timer(1.5), "timeout")
		
		combat_util.Combat_Action.COUNTER:
			combat_label.text = "You prepare to counter"
			yield(get_tree().create_timer(1.5), "timeout")
			combat_label.text = "But nothing happened"
			yield(get_tree().create_timer(1.5), "timeout")
		
		combat_util.Combat_Action.HEAVY:
			combat_label.text = "You charge up your attack"
			yield(get_tree().create_timer(1.5), "timeout")
			combat_label.text = "The enemy also charges up!"
			yield(get_tree().create_timer(1.5), "timeout")
			playerDmg /= 2
			enemy_instance.health -= playerDmg
			combat_label.text = "The Enemy takes %s dmg" % playerDmg
			yield(get_tree().create_timer(1.5), "timeout")
			enemyDmg /= 2
			player_instance.health -= enemyDmg
			combat_label.text = "You take %s dmg" % enemyDmg
			yield(get_tree().create_timer(1.5), "timeout")
		
		_:
			yield(get_tree(), "idle_frame")


func _on_Counter_pressed():
	TakeTurn(combat_util.Combat_Action.COUNTER)


func _on_Quick_pressed():
	TakeTurn(combat_util.Combat_Action.QUICK)


func _on_Heavy_pressed():
	TakeTurn(combat_util.Combat_Action.HEAVY)
