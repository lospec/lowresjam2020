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
	get_tree().paused = true
	
	player_instance = player
	enemy_instance = enemy
	
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


func ActionCompare(action1, action2):
	if action1 == action2:
		return 0
	elif combat_util.GetActionWeakness(action2) == action1:
		return 1
	elif combat_util.GetActionWeakness(action1) == action2:
		return 2
	return -1


func TakeTurn(playerAction):
	var enemyAction = enemy_instance.GetAction()#playerAction#
	var win = ActionCompare(playerAction, enemyAction)
	
	combat_menu.set_buttons_visible(false)
	
	match win:
		0:
			yield(Tie(playerAction), "completed")
		
		1:
			yield(PlayerWin(playerAction), "completed")
		
		2:
			yield(EnemyWin(playerAction), "completed")
	
	combat_menu.reset_ui()


func PlayerWin(playerAction):
	#yield(combat_menu.show_combat_label("Player win case is still in progress", 2), "completed")
	var playerDmg = player_instance.get_base_damage(playerAction);
	
	match (playerAction):
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("Attack hit!", 2), "completed")
			
			enemy_instance.health -= playerDmg
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
		
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Countered!", 2), "completed")
			
			playerDmg /= 2
			enemy_instance.health -= playerDmg
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("Enemy counter broken!", 2), "completed")
			
			enemy_instance.health -= playerDmg
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on PlayerWin()", 2), "completed")


func EnemyWin(enemyAction):
	#yield(combat_menu.show_combat_label("Player win case is still in progress", 2), "completed")
	var enemyDmg = enemy_instance.get_base_damage(enemyAction);
	
	match (enemyAction):
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("The Enemy attacked first!", 2), "completed")
			
			player_instance.health -= enemyDmg
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Enemy countered!", 2), "completed")
			
			enemyDmg /= 2
			player_instance.health -= enemyDmg
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("The Enemy broke your counter!", 2), "completed")
			
			player_instance.health -= enemyDmg
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on EnemyWin()", 2), "completed")


func Tie(action):
	var enemyDmg = enemy_instance.get_base_damage(action);
	var playerDmg = player_instance.get_base_damage(action);
	
	match (action):
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("Both of you attack", 2), "completed")
			
			enemy_instance.health -= playerDmg
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
			
			player_instance.health -= enemyDmg
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("You prepare to counter", 2), "completed")
			yield(combat_menu.show_combat_label("But nothing happened", 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("You charge up your attack", 2), "completed")
			yield(combat_menu.show_combat_label("The enemy also charges up!", 2), "completed")
			
			playerDmg /= 2
			enemy_instance.health -= playerDmg
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
			
			enemyDmg /= 2
			player_instance.health -= enemyDmg
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on Tie()", 2), "completed")


func _on_Counter_pressed():
	TakeTurn(combat_util.Combat_Action.COUNTER)


func _on_Quick_pressed():
	TakeTurn(combat_util.Combat_Action.QUICK)


func _on_Heavy_pressed():
	TakeTurn(combat_util.Combat_Action.HEAVY)
