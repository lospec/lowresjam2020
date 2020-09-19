extends CanvasLayer

# Signals
# Currently using bool, should've used enum to check how the combat ended
signal combat_done(player_win, enemy_instance)
signal bag_opened(player_instance)

# Constants
const COMBAT_ANIM_UTIL = preload("res://Utility/combat_anim_util.gd")
const BACKGROUNDS = [
	preload("res://Combat/ScenicBackgrounds/Rocks.png"),
	preload("res://Combat/ScenicBackgrounds/beach.png"),
	preload("res://Combat/ScenicBackgrounds/forest.png"),
	preload("res://Combat/ScenicBackgrounds/lakes.png"),
	preload("res://Combat/ScenicBackgrounds/mountainlittle.png"),
	preload("res://Combat/ScenicBackgrounds/path.png"),
	preload("res://Combat/ScenicBackgrounds/plain.png"),
	preload("res://Combat/ScenicBackgrounds/plateausmall.png"),
]

# Public Variables
var combat_util = preload("res://Combat/CombatUtil.gd")
var player_instance: BaseEntity
var enemy_instance: BaseEntity

# Onready Variables
onready var combat_menu = $CombatMenu
onready var player_combat: CombatChar = $PlayerCombat
onready var enemy_combat: CombatChar = $EnemyCombat

# TEMP HACK
func _on_enemy_take_damage(damage, damage_type):
	combat_menu.update_particle(enemy_instance)
	combat_menu.animate_enemy_hurt(enemy_instance, damage)

func setup_combat(player, enemy):
	$Background.texture = Utility.rand_element(BACKGROUNDS)
	
	AudioSystem.stop_music()
	
	player_combat.char_instance = player
	enemy_combat.char_instance = enemy
	
	combat_menu.current_menu = combat_menu.MENU_SELECTED.MAIN
	combat_menu.reset_ui()
	
	player_instance = player
	enemy_instance = enemy
	
	if !player_instance.is_connected("health_changed", combat_menu, "update_player_health_value"):
# warning-ignore:return_value_discarded
		player_instance.connect("health_changed", combat_menu, "update_player_health_value")
		
	if !enemy_instance.is_connected("health_changed", combat_menu, "update_enemy_health_value"):
# warning-ignore:return_value_discarded
		enemy_instance.connect("health_changed", combat_menu, "update_enemy_health_value")
	
	if !enemy_combat.is_connected("damage_taken", self, "_on_enemy_take_damage"):
		# warning-ignore:return_value_discarded
		enemy_combat.connect("damage_taken", self, "_on_enemy_take_damage")
	
	combat_menu.set_player_health_value(player_instance.max_health,
			player_instance.health)
	combat_menu.set_enemy_health_value(enemy_instance.max_health,
			enemy_instance.health)
	
	var wpn_name: String = player_instance.equipped_weapon
	var wpn_texture = load("res://Combat/WeaponSprites/%s.png" % wpn_name.to_lower())
	
	if wpn_texture == null:
		print("Weapon Battle sprite for %s not found" % wpn_name)
	
	combat_menu.player_weapon.texture = wpn_texture
	
	combat_menu.enemy_image.texture.atlas = enemy_instance.battle_texture
	combat_menu.enemy_image.texture.region = Rect2(
		COMBAT_ANIM_UTIL.Anim_State_Region_Pos_X[COMBAT_ANIM_UTIL.Anim_States.NORMAL],
		COMBAT_ANIM_UTIL.BATTLE_TEXTURE_POS_Y,
		COMBAT_ANIM_UTIL.BATTLE_TEXTURE_WIDTH, COMBAT_ANIM_UTIL.BATTLE_TEXTURE_HEIGHT)
	
	start_combat()

func start_combat():
	var sfx_player = AudioSystem.play_sfx(AudioSystem.SFX.BATTLE_INTRO,
			null, -20)
	sfx_player.connect("finished", self, "play_battle_music")
	
	var combat = true
# warning-ignore:unused_variable
	var turn_count = 0
	while combat:
		turn_count += 1
		#print("Turn %s: START, %s" % [turn_count, combat])
		
		combat_menu.update_particle(enemy_instance)
		
#		var e: StatusEffect = enemy_instance.status_effects.get("OnFire", null)
#		if e != null:
#			print("OnFire last for %s more turn" % e.duration)
		
		var turn = TakeTurn()
		
		if turn.is_valid():
			turn = yield(turn, "completed")
		
		combat = turn
		
		if combat:
			# PLAYER STATUS EFFECTS on_turn_end
			for key in player_instance.status_effects.keys():
				var se : StatusEffect = player_instance.status_effects[key]
				se.on_turn_end(player_combat)
				
				if se.expired:
					player_instance.status_effects.erase(key)
					
			# ENEMY STATUS EFFECTS on_turn_end
			for key in enemy_instance.status_effects.keys():
				var se : StatusEffect = enemy_instance.status_effects[key]
				se.on_turn_end(enemy_combat)
				
				if se.expired:
					enemy_instance.status_effects.erase(key)
			
			if check_combat_end():
				if player_instance.health <= 0:
					yield(combat_menu.show_combat_label("YOU DIED", 2), "completed")
					yield(combat_menu.show_combat_label("GAME OVER", 2), "completed")
					combat_menu.combat_label.visible = true
					end_combat(CombatUtil.Outcome.COMBAT_LOSE)
				
				elif enemy_instance.health <= 0:
					yield(combat_menu.show_combat_label("YOU WON", 2), "completed")
					yield(combat_menu.show_combat_label("CONGRATULATION", 2), "completed")
					combat_menu.combat_label.visible = true
					end_combat(CombatUtil.Outcome.COMBAT_WIN)
				
				combat = false
		
		if combat:
			combat_menu.reset_ui()
		#print("Turn %s: END, %s" % [turn_count, combat])


func play_battle_music():
	var enemy_data = Data.enemy_data[enemy_instance.enemy_name]
	var enemy_race = enemy_data.race
	
	var enemy_race_music = {
		"Robot": AudioSystem.Music.BATTLE_ROBOT,
		"Beast": AudioSystem.Music.BATTLE_BEAST,
		"Demon": AudioSystem.Music.BATTLE_DEMON,
		"Human": AudioSystem.Music.BATTLE_HUMAN,
		"Gnome": AudioSystem.Music.BATTLE_GNOME,
		"Flora": AudioSystem.Music.BATTLE_FLORA,
		"Slime": AudioSystem.Music.BATTLE_SLIME,
	}
	
	AudioSystem.play_music(enemy_race_music[enemy_race], -25)


func end_combat(outcome):
	player_instance.disconnect("health_changed", combat_menu, "update_player_health_value")
	enemy_instance.disconnect("health_changed", combat_menu, "update_enemy_health_value")
	emit_signal("combat_done", outcome, enemy_instance)


func TakeTurn() -> bool:
	var playerAction = player_combat.get_action()
	var enemyAction = enemy_combat.get_action()
	
	if playerAction is GDScriptFunctionState and playerAction.is_valid():
		#print("wait for player action")
		playerAction = yield(playerAction, "completed")
	#print("player action: %s" % combat_util.GetActionName(playerAction))
	
	if enemyAction is GDScriptFunctionState and enemyAction.is_valid():
		#print("wait for enemy action")
		enemyAction = yield(enemyAction, "completed")
	#print("enemy action: %s" % combat_util.GetActionName(enemyAction))
	
	combat_menu.set_buttons_visible(false)
	# Show the player and the enemies choices
	yield(combat_menu.show_turn_result(playerAction, enemyAction), "completed")
	
	var timer = get_tree().create_timer(1.5)
	
	if playerAction == combat_util.Combat_Action.FLEE:
		var flee = yield(PlayerFlee(enemyAction), "completed")
		if flee:
			combat_menu.combat_label.visible = true
			end_combat(CombatUtil.Outcome.PLAYER_FLEE)
			return false
	
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
	
	if check_combat_end():
		if player_instance.health <= 0:
			yield(combat_menu.show_combat_label("YOU DIED", 2), "completed")
			yield(combat_menu.show_combat_label("GAME OVER", 2), "completed")
			combat_menu.combat_label.visible = true
			end_combat(CombatUtil.Outcome.COMBAT_LOSE)
		
		elif enemy_instance.health <= 0:
			yield(combat_menu.show_combat_label("YOU WON", 2), "completed")
			yield(combat_menu.show_combat_label("CONGRATULATION", 2), "completed")
			combat_menu.combat_label.visible = true
			end_combat(CombatUtil.Outcome.COMBAT_WIN)
		
		return false
	
	return true

func check_combat_end() -> bool:
	return player_instance.health <= 0 or enemy_instance.health <= 0

#### to differentiate between different kinds of outcome in the game other than just the color
#### i left some suggestion comment in each of the cases
func PlayerFlee(enemyAction): # Should be replaced with CharFlee so the enemy can have a chance to flee to
	var rule = CombatUtil.FleeRule.new(enemyAction)
	var enemyDmg = enemy_combat.get_base_damage(enemyAction)
	var outcome = rule.roll()
	match outcome:
		rule.Outcome.SUCCESS:
			yield(combat_menu.show_combat_label("Got away safely", 2), "completed")
			return true
		rule.Outcome.SUCCESS_DMG:
			enemy_combat.attack(player_combat, enemyAction, enemyDmg * rule._dmg_modifier,
					enemy_instance, player_instance)
			combat_menu.animate_player_hurt(enemyDmg)
			yield(combat_menu.show_combat_label("Got away not so safely", 2), "completed")
			return true
		rule.Outcome.FAIL:
			yield(combat_menu.show_combat_label("Failed to flee", 2), "completed")
			enemy_combat.attack(player_combat, enemyAction, enemyDmg * rule._dmg_modifier,
					enemy_instance, player_instance)
			combat_menu.animate_player_hurt(enemyDmg)
			return false

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
			
			player_combat.attack(enemy_combat, playerAction, playerDmg, player_instance, enemy_instance)
			#combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
		
		combat_util.Combat_Action.COUNTER:
			# Player COUNTER vs Enemy QUICK
			# Show the player blocking THEN show the attack effect
			#combat_menu.show_combat_label("Countered!")
			yield(combat_menu.animate_player_attack(player_combat, playerAction), "completed")
			
			player_combat.attack(enemy_combat, playerAction, playerDmg, player_instance, enemy_instance)
			#combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
		
		combat_util.Combat_Action.HEAVY:
			# Player HEAVY vs Enemy COUNTER
			# Show the player charging THEN show the attack effect
			#combat_menu.show_combat_label("Attack hit!")
			yield(combat_menu.animate_player_attack(player_combat, playerAction), "completed")
			
			player_combat.attack(enemy_combat, playerAction, playerDmg, player_instance, enemy_instance)
			#combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
		
		_:
			combat_menu.hide_turn_result()
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
			
			enemy_combat.attack(player_combat, enemyAction, enemyDmg, enemy_instance, player_instance)
			yield(combat_menu.animate_player_hurt(enemyDmg), "completed")
		
		combat_util.Combat_Action.COUNTER:
			# Player QUICK vs Enemy COUNTER
			# if there's enough time, maybe make enemy block sprite to show that
			# the enemy is indeed blocking the players attack THEN show
			# the player take damage
			#combat_menu.show_combat_label("Enemy countered!")
			
			enemyDmg /= 2
			enemy_combat.attack(player_combat, enemyAction, enemyDmg, enemy_instance, player_instance)
			yield(combat_menu.animate_player_hurt(enemyDmg, true), "completed")
		
		combat_util.Combat_Action.HEAVY:
			# Player COUNTER vs Enemy HEAVY
			# maybe show the player blocking(same as in the PlayerWin counter state)
			# but instead of showing the player attack, show the player take damage instead
			#combat_menu.show_combat_label("The Enemy broke your counter!")
			
			enemy_combat.attack(player_combat, enemyAction, enemyDmg, enemy_instance, player_instance)
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
			
			player_combat.attack(enemy_combat, action, playerDmg, player_instance, enemy_instance)
			#combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
			
			enemy_combat.attack(player_combat, action, enemyDmg, enemy_instance, player_instance)
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
			player_combat.attack(enemy_combat, action, playerDmg, player_instance, enemy_instance)
			#combat_menu.animate_enemy_hurt(enemy_instance, playerDmg)
			
			enemyDmg /= 2
			enemy_combat.attack(player_combat, action, enemyDmg, enemy_instance, player_instance)
			combat_menu.animate_player_hurt(enemyDmg)
		
		_:
			combat_menu.hide_turn_result()
			yield(combat_menu.show_combat_label("ERROR. Unknown Action on Tie()", 2), "completed")


func _on_Player_enemy_detected(player, enemy):
	get_tree().paused = true
	player.hud_margin.visible = false
	setup_combat(player, enemy)
	combat_menu.visible = true

func _on_Counter_pressed():
	pass
	#TakeTurn(combat_util.Combat_Action.COUNTER)

func _on_Quick_pressed():
	pass
	#TakeTurn(combat_util.Combat_Action.QUICK)

func _on_Heavy_pressed():
	pass
	#TakeTurn(combat_util.Combat_Action.HEAVY)

func _on_Flee_pressed():
	pass
	#TakeTurn(combat_util.Combat_Action.FLEE)

func _on_CombatMenu_bag_opened():
	emit_signal("bag_opened", player_instance)


func _process(_delta):
	# I'm sorry for this terrible code
	$Background.visible = combat_menu.visible
