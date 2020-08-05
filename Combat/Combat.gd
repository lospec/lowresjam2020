extends CanvasLayer

# Signals
# Currently using bool, should've used enum to check how the combat ended
signal combat_done(player_win)

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var player_instance
var enemy_instance

# Onready Variables
onready var combat_menu = $CombatMenu
onready var enemy_image = $CombatMenu/VBoxContainer/EnemyHUD/VBoxContainer/Enemy
onready var player_combat : CombatChar = $PlayerCombat
onready var enemy_combat : CombatChar = $EnemyCombat

func _on_Player_enemy_detected(player, enemy):
	get_tree().paused = true
	setup_combat(player, enemy)
	combat_menu.visible = true

# maybe these health changed signals should be moved to CombatMenu.gd, so this script is more cleaner?
# i don't know, your call Mariothedog 
func _on_Player_health_changed(_old_health, new_health):
	combat_menu.update_player_health_value(new_health)

func _on_Enemy_health_changed(_old_health, new_health):
	combat_menu.update_enemy_health_value(new_health)


func setup_combat(player, enemy):
	player_combat.char_instance = player
	enemy_combat.char_instance = enemy
	player_instance = player
	enemy_instance = enemy
	
	combat_menu.set_player_health_value(player_instance.max_health,
			player_instance.health)
	combat_menu.set_enemy_health_value(enemy_instance.max_health,
			enemy_instance.health)
	
	enemy_image.texture = enemy_instance.battle_texture_normal

func TakeTurn(playerAction):
	var enemyAction = enemy_combat.get_action()#playerAction#
	combat_menu.set_buttons_visible(false)
	
	if playerAction == combat_util.Combat_Action.FLEE:
		var flee = yield(PlayerFlee(enemyAction), "completed")
		if flee:
			combat_menu.combat_label.visible = true
			emit_signal("combat_done", true)
			return
	
	else:
		var win = combat_util.ActionCompare(playerAction, enemyAction)
		
		match win:
			0:
				yield(Tie(playerAction), "completed")
			
			1:
				yield(PlayerWin(playerAction), "completed")
			
			2:
				yield(EnemyWin(enemyAction), "completed")
			
			_:
				yield(combat_menu.show_combat_label("ERROR: Invalid win check", 2), "completed")
	
	# PLACEHOLDER END CONDITIONS
	if player_instance.health <= 0:
		yield(combat_menu.show_combat_label("YOU DIED", 2), "completed")
		yield(combat_menu.show_combat_label("GAME OVER", 2), "completed")
		combat_menu.combat_label.visible = true
		emit_signal("combat_done", false)
		return
	
	if enemy_instance.health <= 0:
		yield(combat_menu.show_combat_label("YOU WON", 2), "completed")
		yield(combat_menu.show_combat_label("CONGRATULATION", 2), "completed")
		combat_menu.combat_label.visible = true
		emit_signal("combat_done", true)
		return
		
	combat_menu.reset_ui()


func PlayerFlee(enemyAction):
	var enemyDmg = enemy_combat.get_base_damage(enemyAction);
	var success = false
	
	match enemyAction:
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Got away safely", 2), "completed")
			success = true
		
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("The Enemy attacked", 2), "completed")
			yield(combat_menu.show_combat_label("Failed to flee", 2), "completed")
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("The Enemy charges up", 2), "completed")
			yield(combat_menu.show_combat_label("Failed to flee", 2), "completed")
			enemyDmg *= 2
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
	
	return success

func PlayerWin(playerAction):
	#yield(combat_menu.show_combat_label("Player win case is still in progress", 2), "completed")
	var playerDmg = player_combat.get_base_damage(playerAction);
	player_combat.hit_combo += 1
	
	match (playerAction):
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("Attack hit!", 2), "completed")
			
			enemy_combat.take_damage(playerDmg)
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
		
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Countered!", 2), "completed")
			
			playerDmg /= 2
			enemy_combat.take_damage(playerDmg)
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("Enemy counter broken!", 2), "completed")
			
			enemy_combat.take_damage(playerDmg)
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on PlayerWin()", 2), "completed")

func EnemyWin(enemyAction):
	#yield(combat_menu.show_combat_label("Player win case is still in progress", 2), "completed")
	var enemyDmg = enemy_combat.get_base_damage(enemyAction);
	
	match (enemyAction):
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("The Enemy attacked first!", 2), "completed")
			
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Enemy countered!", 2), "completed")
			
			enemyDmg /= 2
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("The Enemy broke your counter!", 2), "completed")
			
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on EnemyWin()", 2), "completed")

func Tie(action):
	var enemyDmg = enemy_combat.get_base_damage(action);
	var playerDmg = player_combat.get_base_damage(action);
	
	match (action):
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("Attack hit!", 2), "completed")
			
			enemy_combat.take_damage(playerDmg)
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
			
			yield(combat_menu.show_combat_label("The enemy attacked!", 2), "completed")
			
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Nothing happened", 2), "completed")
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("The enemy also charges up!", 2), "completed")
			
			playerDmg /= 2
			enemy_combat.take_damage(playerDmg)
			yield(combat_menu.show_combat_label("The Enemy takes %s dmg" % playerDmg, 2), "completed")
			
			enemyDmg /= 2
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.show_combat_label("You take %s dmg" % enemyDmg, 2), "completed")
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on Tie()", 2), "completed")


func _on_Counter_pressed():
	TakeTurn(combat_util.Combat_Action.COUNTER)


func _on_Quick_pressed():
	TakeTurn(combat_util.Combat_Action.QUICK)


func _on_Heavy_pressed():
	TakeTurn(combat_util.Combat_Action.HEAVY)

func _on_Flee_pressed():
	TakeTurn(combat_util.Combat_Action.FLEE)
