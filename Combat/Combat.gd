extends CanvasLayer

# Signals
# Currently using bool, should've used enum to check how the combat ended
signal combat_done(player_win, enemy_instance)

# Constants
const COMBAT_ANIM_UTIL = preload("res://Utility/combat_anim_util.gd")

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var player_instance
var enemy_instance

# Onready Variables
onready var combat_menu = $CombatMenu
onready var player_combat: CombatChar = $PlayerCombat
onready var enemy_combat: CombatChar = $EnemyCombat

func setup_combat(player, enemy):
	player_combat.char_instance = player
	enemy_combat.char_instance = enemy
	
	combat_menu.reset_ui()
	combat_menu.current_menu = combat_menu.MENU_SELECTED.MAIN
	
	player_instance = player
	enemy_instance = enemy
	
	combat_menu.set_player_health_value(player_instance.max_health,
			player_instance.health)
	combat_menu.set_enemy_health_value(enemy_instance.max_health,
			enemy_instance.health)
	
	combat_menu.enemy_image.texture.atlas = enemy_instance.battle_texture
	combat_menu.enemy_image.texture.region = Rect2(
		COMBAT_ANIM_UTIL.Anim_State_Region_Pos_X[COMBAT_ANIM_UTIL.Anim_States.NORMAL],
		COMBAT_ANIM_UTIL.BATTLE_TEXTURE_POS_Y,
		COMBAT_ANIM_UTIL.BATTLE_TEXTURE_WIDTH, COMBAT_ANIM_UTIL.BATTLE_TEXTURE_HEIGHT)

func end_combat(player_win):
	emit_signal("combat_done", player_win, enemy_instance)

func TakeTurn(playerAction):
	var enemyAction = enemy_combat.get_action()#playerAction#
	combat_menu.set_buttons_visible(false)
	
	# Show the player and the enemies choices
	yield(combat_menu.show_turn_result(playerAction, enemyAction), "completed")
	
	var timer = get_tree().create_timer(1.5)
	
	if playerAction == combat_util.Combat_Action.FLEE:
		var flee = yield(PlayerFlee(enemyAction), "completed")
		if flee:
			combat_menu.combat_label.visible = true
			end_combat(false)
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
	
	if timer.time_left > 0:
		yield(timer, "timeout")
	
	combat_menu.hide_turn_result()
	
	# PLACEHOLDER END CONDITIONS
	if player_instance.health <= 0:
		yield(combat_menu.show_combat_label("YOU DIED", 2), "completed")
		yield(combat_menu.show_combat_label("GAME OVER", 2), "completed")
		combat_menu.combat_label.visible = true
		end_combat(false)
		return
	
	if enemy_instance.health <= 0:
		yield(combat_menu.show_combat_label("YOU WON", 2), "completed")
		yield(combat_menu.show_combat_label("CONGRATULATION", 2), "completed")
		combat_menu.combat_label.visible = true
		end_combat(true)
		return
	
	combat_menu.reset_ui()

#### to differentiate between different kinds of outcome in the game other than just the color
#### i left some suggestion comment in each of the cases
func PlayerFlee(enemyAction): # Should be replaced with CharFlee so the enemy can have a chance to flee to
	var enemyDmg = enemy_combat.get_base_damage(enemyAction);
	var success = false
	
	match enemyAction:
		combat_util.Combat_Action.COUNTER:
			yield(combat_menu.show_combat_label("Got away safely", 2), "completed")
			success = true
		
		combat_util.Combat_Action.QUICK:
			yield(combat_menu.show_combat_label("Failed to flee", 2), "completed")
			
			player_combat.take_damage(enemyDmg)
			combat_menu.animate_player_hurt(enemyDmg)
		
		combat_util.Combat_Action.HEAVY:
			yield(combat_menu.show_combat_label("Failed to flee", 2), "completed")
			
			enemyDmg *= 2
			player_combat.take_damage(enemyDmg)
			combat_menu.animate_player_hurt(enemyDmg)
	
	return success

func PlayerWin(playerAction):
	var playerDmg = player_combat.get_base_damage(playerAction);
	player_combat.hit_combo += 1
	
	# probably can just merge the repeated lines, but i think i'll keep it for now
	# just in case there will be different things happening in each action 
	match (playerAction):
		combat_util.Combat_Action.QUICK:
			# Player QUICK vs Enemy HEAVY
			# Already do-able: Just show the player attack the enemy immediately
			#combat_menu.show_combat_label("Attack hit!")
			yield(combat_menu.animate_player_attack(player_combat, playerAction), "completed")
			
			enemy_combat.take_damage(playerDmg)
			combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
		
		combat_util.Combat_Action.COUNTER:
			# Player COUNTER vs Enemy QUICK
			# Show the player blocking THEN show the attack effect
			#combat_menu.show_combat_label("Countered!")
			yield(combat_menu.animate_player_attack(player_combat, playerAction), "completed")
			
			enemy_combat.take_damage(playerDmg)
			combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
		
		combat_util.Combat_Action.HEAVY:
			# Player HEAVY vs Enemy COUNTER
			# Show the player charging THEN show the attack effect
			#combat_menu.show_combat_label("Attack hit!")
			yield(combat_menu.animate_player_attack(player_combat, playerAction), "completed")
			
			enemy_combat.take_damage(playerDmg)
			combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on PlayerWin()", 2), "completed")

func EnemyWin(enemyAction):
	#yield(combat_menu.show_combat_label("Player win case is still in progress", 2), "completed")
	var enemyDmg = enemy_combat.get_base_damage(enemyAction);
	
	match (enemyAction):
		combat_util.Combat_Action.QUICK:
			# Player HEAVY vs Enemy QUICK
			# Show the player charge up maybe about half way THEN show
			# the player take damage
			#combat_menu.show_combat_label("The Enemy attacked first!")
			
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.animate_player_hurt(enemyDmg), "completed")
		
		combat_util.Combat_Action.COUNTER:
			# Player QUICK vs Enemy COUNTER
			# if there's enough time, maybe make enemy block sprite to show that
			# the enemy is indeed blocking the players attack THEN show
			# the player take damage
			#combat_menu.show_combat_label("Enemy countered!")
			
			enemyDmg /= 2
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.animate_player_hurt(enemyDmg, true), "completed")
		
		combat_util.Combat_Action.HEAVY:
			# Player COUNTER vs Enemy HEAVY
			# maybe show the player blocking(same as in the PlayerWin counter state)
			# but instead of showing the player attack, show the player take damage instead
			#combat_menu.show_combat_label("The Enemy broke your counter!")
			
			player_combat.take_damage(enemyDmg)
			yield(combat_menu.animate_player_hurt(enemyDmg), "completed")
		
		_:
			combat_menu.hide_turn_result()
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on EnemyWin()", 2), "completed")

func Tie(action):
	var enemyDmg = enemy_combat.get_base_damage(action);
	var playerDmg = player_combat.get_base_damage(action);
	
	match (action):
		combat_util.Combat_Action.QUICK:
			# Player QUICK vs Enemy QUICK
			# Already doable: Show the player attack and the player take damage
			# at the same time
			#combat_menu.show_combat_label("The enemy attacked!")
			yield(combat_menu.animate_player_attack(player_combat, action), "completed")
			
			enemy_combat.take_damage(playerDmg)
			combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
			
			player_combat.take_damage(enemyDmg)
			combat_menu.animate_player_hurt(enemyDmg)
		
		combat_util.Combat_Action.COUNTER:
			# Player COUNTER vs Enemy COUNTER
			# Show both the enemy and the player blocking, but nothing happen after that
			#yield(combat_menu.show_combat_label("Nothing happened", 2), "completed")
			yield(get_tree().create_timer(1.5), "timeout")
		
		combat_util.Combat_Action.HEAVY:
			# Player HEAVY vs Enemy HEAVY
			# Show the player charge THEN show the player attack and the player
			# take damage at the same time
			#combat_menu.show_combat_label("The enemy used heavy attack!")
			yield(combat_menu.animate_player_attack(player_combat, action), "completed")
			
			playerDmg /= 2
			enemy_combat.take_damage(playerDmg)
			combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
			
			enemyDmg /= 2
			player_combat.take_damage(enemyDmg)
			combat_menu.animate_player_hurt(enemyDmg)
		
		_:
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on Tie()", 2), "completed")


func _on_Player_enemy_detected(player, enemy):
	get_tree().paused = true
	player.hud_margin.visible = false
	setup_combat(player, enemy)
	combat_menu.visible = true

# maybe these health changed signals should be moved to CombatMenu.gd, so this script is more cleaner?
# i don't know, your call Mariothedog
func _on_Player_health_changed(_old_health, new_health):
	combat_menu.update_player_health_value(new_health)

func _on_Enemy_health_changed(_old_health, new_health):
	combat_menu.update_enemy_health_value(new_health)

func _on_Counter_pressed():
	TakeTurn(combat_util.Combat_Action.COUNTER)

func _on_Quick_pressed():
	TakeTurn(combat_util.Combat_Action.QUICK)

func _on_Heavy_pressed():
	TakeTurn(combat_util.Combat_Action.HEAVY)

func _on_Flee_pressed():
	TakeTurn(combat_util.Combat_Action.FLEE)
