extends Node

# Constants
enum COMBAT_ACTION {
	INVALID = -1,
	QUICK,
	COUNTER,
	HEAVY 
}

# Onready Variables
onready var combat_menu = $CombatMenu
onready var enemy_combat = $EnemyCombat
onready var player_combat = $PlayerCombat
onready var combat_label = $CombatMenu/VBoxContainer/PlayerHUD/ChoiceHUD/CombatLabelPadding/CombatLabel

func GetActionWeakness(action):
	match (action):
		COMBAT_ACTION.QUICK:
			return COMBAT_ACTION.COUNTER
		COMBAT_ACTION.COUNTER:
			return COMBAT_ACTION.HEAVY
		COMBAT_ACTION.HEAVY:
			return COMBAT_ACTION.QUICK
	return COMBAT_ACTION.INVALID


func ActionCompare(action1, action2):
	if (action1 == action2):
		return 0
	elif (GetActionWeakness(action2) == action1):
		return 1
	elif (GetActionWeakness(action1) == action2):
		return 2
	return -1


func TakeTurn(playerAction):
	var enemyAction = playerAction#enemyCombat.GetAction()
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
	var enemyDmg = enemy_combat.GetDamage();
	var playerDmg = player_combat.GetDamage();
	
	match (action):
		COMBAT_ACTION.QUICK:
			combat_label.text = "Both of you attack"
			yield(get_tree().create_timer(1.5), "timeout")
			enemy_combat.hp -= playerDmg
			combat_label.text = "The Enemy takes %s dmg" % playerDmg
			yield(get_tree().create_timer(1.5), "timeout")
			player_combat.hp -= enemyDmg
			combat_label.text = "You take %s dmg" % enemyDmg
			yield(get_tree().create_timer(1.5), "timeout")
		
		COMBAT_ACTION.COUNTER:
			combat_label.text = "You prepare to counter"
			yield(get_tree().create_timer(1.5), "timeout")
			combat_label.text = "But nothing happened"
			yield(get_tree().create_timer(1.5), "timeout")
		
		COMBAT_ACTION.HEAVY:
			combat_label.text = "You charge up your attack"
			yield(get_tree().create_timer(1.5), "timeout")
			combat_label.text = "The enemy also charges up!"
			yield(get_tree().create_timer(1.5), "timeout")
			playerDmg = playerDmg / 2
			enemy_combat.hp -= playerDmg
			combat_label.text = "The Enemy takes %s dmg" % playerDmg
			yield(get_tree().create_timer(1.5), "timeout")
			enemyDmg = enemyDmg / 2
			player_combat.hp -= enemyDmg
			combat_label.text = "You take %s dmg" % enemyDmg
			yield(get_tree().create_timer(1.5), "timeout")
		
		_:
			yield(get_tree(), "idle_frame")


func _on_Counter_pressed():
	TakeTurn(COMBAT_ACTION.COUNTER)


func _on_Quick_pressed():
	TakeTurn(COMBAT_ACTION.QUICK)


func _on_Heavy_pressed():
	TakeTurn(COMBAT_ACTION.HEAVY)
